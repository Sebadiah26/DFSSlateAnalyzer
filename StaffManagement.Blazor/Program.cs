using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using StaffManagement.Blazor.Services;
using StaffManagement.Core.Extensions;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Core.TestData;
using StaffManagement.Data;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // Site-wide login gate: every Blazor route funnels through MapFallbackToPage("/_Host") below, so
    // authorizing the whole Pages folder means any unauthenticated request gets challenged (redirected
    // to Account/Login) before a Blazor circuit ever starts, instead of today's fully-open-by-default
    // behavior. Account/Login and Account/AccessDenied themselves must stay reachable while
    // unauthenticated - see their own [AllowAnonymous] attributes.
    options.Conventions.AuthorizeFolder("/");
});
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddMudServices();
builder.Services.AddStaffManagementCoreClasses(builder.Configuration);

// Blazor-layer "UI state" services (per-circuit), matching the reference app's
// ContestStateService/UserService pattern. CurrentUserService is registered for both its concrete
// type (pages/components inject it directly for impersonation controls, NeedsReviewCount, etc.) and
// ICurrentUserContext (what Core services/repositories depend on) - same scoped instance either way.
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<FlashMessageService>();
builder.Services.AddScoped<SelectedUnitFilterService>();

// Three auth schemes now coexist:
//   - IISDefaults.AuthenticationScheme: unchanged, still auto-populated by the IIS module's own
//     Negotiate/NTLM handshake when the request is on-network/domain-joined - this is the app's
//     original (and still preferred) login path, requiring zero extra clicks.
//   - Cookie: holds the signed-in identity across requests once established, whichever way it was
//     established. SAML itself is only the initial handshake; without a cookie the user would have to
//     re-authenticate with ClassLink on every request.
//   - "Saml2": drives the ClassLink redirect/callback (ClassLink SSO, see Saml2 config section below).
//
// "Smart" is a policy scheme (AddPolicyScheme) used as the *default* authenticate/challenge scheme: on
// each request it checks whether IIS already produced a successful Windows identity and, if so,
// forwards to that scheme; otherwise it forwards to the Cookie scheme, whose LoginPath sends
// unauthenticated users to our own Account/Login page (which offers the "Login with ClassLink" button)
// rather than an automatic silent redirect straight into the SAML dance.
//
// Known limitation: this can only be fully verified under real IIS (same gap as IIS Express Windows-
// auth testing already noted elsewhere) - under `dotnet run`/Kestrel, IISDefaults.AuthenticationScheme
// never succeeds, so local testing always falls through to the Cookie/Login path. That's expected.
IServiceProvider? serviceProviderForSamlClaims = null;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Smart";
    options.DefaultChallengeScheme = "Smart";
})
.AddPolicyScheme("Smart", "Windows or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // The "Windows" (IISDefaults) scheme handler is only auto-registered when actually hosted
        // in-process behind IIS with Windows auth enabled at the IIS level - under `dotnet run`/Kestrel
        // (all local/test-mode testing) it doesn't exist at all, and AuthenticateAsync against a scheme
        // with no registered handler throws rather than just failing, which would otherwise 500 every
        // single request. Check it's actually registered first and fall through to Cookie if not.
        var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var iisSchemeRegistered = schemeProvider.GetSchemeAsync(IISDefaults.AuthenticationScheme).GetAwaiter().GetResult() != null;

        if (iisSchemeRegistered)
        {
            var iisResult = context.AuthenticateAsync(IISDefaults.AuthenticationScheme).GetAwaiter().GetResult();
            if (iisResult?.Succeeded == true)
            {
                return IISDefaults.AuthenticationScheme;
            }
        }

        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddSaml2(options =>
{
    options.SPOptions.EntityId = new EntityId(builder.Configuration["Saml2:EntityId"]);

    // ClassLink's SAML app config for this district determines what's actually asserted here - the
    // NameID ClassLink's metadata advertises is "persistent"/"unspecified" format, i.e. an opaque
    // ClassLink-internal identifier, NOT the user's email. The email/username we need to validate
    // against ad.User arrives as a separate assertion attribute instead (exact attribute name is
    // unknown until a real login is attempted against a registered ClassLink app - see
    // ResolveIncomingIdentifier below, which checks several likely candidates and can be extended once
    // a real assertion's claims are inspected).
    // ClassLink's entityID *is* its metadata URL (confirmed by fetching Saml2:IdentityProviderMetadataUrl
    // directly), so LoadMetadata alone is enough - Sustainsys fetches metadata from the EntityId itself.
    options.IdentityProviders.Add(new IdentityProvider(
        new EntityId(builder.Configuration["Saml2:ClassLinkEntityId"]), options.SPOptions)
    {
        LoadMetadata = true
    });

    // Runs synchronously right after Sustainsys builds the ClaimsPrincipal from ClassLink's assertion,
    // before the local sign-in cookie is written. Resolves the asserted identity against ad.User and
    // either normalizes the principal's Name claim to the matched SamaccountName (so
    // CurrentUserService's existing Windows-identity resolution logic - strip DOMAIN\, look up by
    // SamaccountName - treats a ClassLink login exactly like a Windows login with zero changes needed
    // there) or tags it as unresolved for the rejection middleware below to catch.
    options.Notifications.AcsCommandResultCreated = (commandResult, _) =>
    {
        var identity = commandResult.Principal!.Identities.Single();
        var incomingIdentifier = ResolveIncomingIdentifier(identity);

        string? samAccountName = null;
        if (!string.IsNullOrWhiteSpace(incomingIdentifier) && serviceProviderForSamlClaims != null)
        {
            using var scope = serviceProviderForSamlClaims.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            samAccountName = userRepository.ResolveSamAccountNameAsync(incomingIdentifier).GetAwaiter().GetResult();
        }

        if (samAccountName != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, samAccountName));
        }
        else
        {
            // Caught by the rejection middleware right after this sign-in completes - see below.
            // Not rejected here directly: this notification isn't well-positioned to issue an HTTP
            // redirect (it fires deep inside SAML response processing, before the final redirect
            // response is constructed), so it just marks the principal and lets a normal request-
            // pipeline middleware do the actual sign-out + redirect to Account/AccessDenied.
            identity.AddClaim(new Claim("StaffManagementUnresolved", "true"));
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
serviceProviderForSamlClaims = app.Services;

// Test mode: in Development, point ConnectionStrings:ActiveDB (via appsettings.Development.json) at
// a local SQL Server instance and this creates the schema + seeds a usable admin account on first
// run, instead of requiring the production DB. See TestDatabaseSeeder for what "test mode" actually
// sets up. No-ops after the first run (checks for existing data before seeding).
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<accountmanagementContext>>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("TestDatabaseSeeder");
    await using var seedDb = await dbFactory.CreateDbContextAsync();
    await TestDatabaseSeeder.EnsureTestDatabaseAsync(seedDb, seedLogger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

// Rejects a ClassLink login whose asserted email/username didn't match any active ad.User row - the
// literal "validate the user against ad.User" gate. Runs on every request, but the marker claim only
// ever exists on a principal freshly minted from an unresolved SAML assertion, and signing out removes
// it, so this only actually fires once, immediately after such a login's callback redirect.
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true &&
        context.User.HasClaim(c => c.Type == "StaffManagementUnresolved"))
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/Account/AccessDenied");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

/// <summary>Tries several likely places ClassLink might assert the user's email or username, in order
/// of preference. NameID is deliberately checked last: ClassLink's metadata for this district
/// advertises "persistent"/"unspecified" NameID formats, not email, so it's likely an opaque
/// ClassLink-internal identifier rather than anything resolvable against ad.User. Extend this once a
/// real ClassLink login has been attempted and the actual assertion's attribute names are known - see
/// this app's memory notes for how to inspect them (log every claim on first attempts).</summary>
static string? ResolveIncomingIdentifier(ClaimsIdentity identity) =>
    identity.FindFirst(ClaimTypes.Email)?.Value
    ?? identity.FindFirst(ClaimTypes.Upn)?.Value
    ?? identity.FindFirst("email")?.Value
    ?? identity.FindFirst("username")?.Value
    ?? identity.FindFirst(ClaimTypes.Name)?.Value
    ?? identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
