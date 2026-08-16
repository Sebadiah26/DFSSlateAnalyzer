using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StaffManagement.Blazor.Pages.Account
{
    /// <summary>Only signs out of the Cookie scheme - there's no real way to "log out" of Windows/IIS
    /// auth from the app side (it's a transparent per-request Negotiate handshake, not a session), so a
    /// domain-joined user hitting this will just get silently re-authenticated by IIS on the very next
    /// request. That's an inherent limitation of Windows auth, not a bug here.</summary>
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("~/Account/Login");
        }
    }
}
