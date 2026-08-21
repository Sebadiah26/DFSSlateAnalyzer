using System.Text;
using ApiCallMonitor.Blazor.Services;
using ApiCallMonitor.Core.Execution;
using ApiCallMonitor.Core.Export;
using ApiCallMonitor.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// SQLite file store under App_Data, next to the app - keeps this tool self-contained with no
// external database server to stand up. Overridable via ConnectionStrings:ApiCallMonitorDb (e.g. to
// point at a different path, or a shared network location) in appsettings/user secrets.
var defaultDbPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "apicallmonitor.db");
Directory.CreateDirectory(Path.GetDirectoryName(defaultDbPath)!);
var configuredConnectionString = builder.Configuration.GetConnectionString("ApiCallMonitorDb");
var connectionString = string.IsNullOrWhiteSpace(configuredConnectionString)
    ? $"Data Source={defaultDbPath}"
    : configuredConnectionString;

builder.Services.AddDbContextFactory<ApiMonitorDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddHttpClient(HttpCallExecutor.HttpClientName, client =>
{
    // Per-call timeouts are enforced explicitly in HttpCallExecutor via a linked
    // CancellationTokenSource, so the shared client just needs a generous outer ceiling instead of
    // the 100s default, so it never cuts off a deliberately long per-call timeout early.
    client.Timeout = TimeSpan.FromMinutes(10);
});

// Singletons: both depend only on other singletons (IHttpClientFactory, IDbContextFactory), and
// registering them this way lets a run started from one Blazor circuit keep executing in the
// background via Task.Run even if that circuit's own DI scope ends first (e.g. the user navigates
// away or disconnects) - see the "Run Now" handler in Collections/Details.razor.
builder.Services.AddSingleton<IHttpCallExecutor, HttpCallExecutor>();
builder.Services.AddSingleton<IRunOrchestrator, RunOrchestrator>();
builder.Services.AddSingleton<RunProgressNotifier>();
builder.Services.AddSingleton<IPowerShellScriptGenerator, PowerShellScriptGenerator>();
builder.Services.AddSingleton<ScriptFileStore>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApiMonitorDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await db.Database.EnsureCreatedAsync();
    await BuiltInConfigurationSeeder.EnsureSeededAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();

// Plain download endpoint rather than Blazor Server JS interop, so a link/button can just point at
// it directly. Always regenerates from the collection's current saved state (independent of
// ScriptFileStore's on-disk copy), so what you download is guaranteed to match what's saved even if
// the on-disk copy somehow lagged behind.
app.MapGet("/api/collections/{collectionId:int}/script.ps1", async (
    int collectionId,
    IDbContextFactory<ApiMonitorDbContext> dbFactory,
    IPowerShellScriptGenerator scriptGenerator) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();

    var collection = await db.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
    if (collection is null)
    {
        return Results.NotFound();
    }

    collection.Calls = await db.Calls
        .Where(c => c.CollectionId == collectionId)
        .OrderBy(c => c.Order)
        .ToListAsync();

    var scriptBytes = Encoding.UTF8.GetBytes(scriptGenerator.Generate(collection));
    var fileName = $"{SanitizeFileName(collection.Name)}.ps1";
    return Results.File(scriptBytes, "application/octet-stream", fileName);
});

app.MapFallbackToPage("/_Host");

app.Run();

static string SanitizeFileName(string name)
{
    var invalidChars = Path.GetInvalidFileNameChars();
    var sanitized = new string(name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray()).Trim();
    return string.IsNullOrWhiteSpace(sanitized) ? "collection" : sanitized;
}
