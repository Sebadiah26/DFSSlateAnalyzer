using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StaffManagement.Blazor.Pages.Account
{
    /// <summary>Entry point for anyone the "Smart" policy scheme couldn't authenticate via IIS/Windows
    /// auth (Program.cs) - Cookie's LoginPath points here. Plain Razor Page, not a Blazor component:
    /// this has to run before any Blazor circuit starts, since the whole Pages folder (including
    /// _Host.cshtml, which every Blazor route falls back to) requires authorization.</summary>
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect("~/");
            }

            return Page();
        }

        /// <summary>Named handler, reached via "?handler=Challenge" - see Login.cshtml's link. Issues
        /// the actual redirect to ClassLink; ClassLink posts back to the Saml2 scheme's own
        /// assertion-consumer endpoint, which Sustainsys.Saml2.AspNetCore2 wires up automatically.</summary>
        public IActionResult OnGetChallenge()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "~/" }, "Saml2");
        }
    }
}
