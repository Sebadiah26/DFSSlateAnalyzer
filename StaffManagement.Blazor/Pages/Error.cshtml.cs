using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StaffManagement.Blazor.Pages;

// Ports HomeController.Error + Views/Shared/Error.cshtml as a plain Razor Page rather than a Blazor
// component, matching the reference app's own Pages/Error.cshtml - error pages need to work even
// when the Blazor circuit itself is what failed, so they can't depend on it.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
