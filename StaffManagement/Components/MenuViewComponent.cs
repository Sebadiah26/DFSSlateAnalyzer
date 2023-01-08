using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;

namespace StaffManagement.Components
{
    /// <summary>
    /// Supplies model for templates in "Views/Shared/Components/Menu" through the Invoke method   Used for example in "Views/Shared/_Layout.cshtml" with the syntax
    /// <vc:menu current-user="@ViewBag.CurrentUser"></vc:menu> as part of the basic template for the site.   
    /// </summary>
    public class MenuViewComponent : BaseViewComponent
    {


        public MenuViewComponent(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {

        }

        public IViewComponentResult Invoke(string CurrentUser)


        {
            if ((ViewBag.ImpersonateUser ?? "") != "" && ViewBag.ImpersonateUser != _appUser.CurrentUser)
            {
                _appUser.ImpersonateUser = ViewBag.ImpersonateUser;
                _appUser.SelectedUser = ViewBag.SelectedUser;
            }

            if (ViewBag.SelectedUnitId != null && ViewBag.SelectedUnitId != "")
            { _appUser.SelectedUnitId = ViewBag.SelectedUnitId; }

            // If impersonating, set current AppUser object, ViewBag, Session object to impersonated user.

            ViewBag.ActiveDirectoryName = _appUser.ActiveDirectoryName;
            ViewBag.ImpersonateUser = _appUser.ImpersonateUser;
            ViewBag.SelectedUser = _appUser.SelectedUser;
            ViewBag.CurrentUser = _appUser.CurrentUser;



            return View(_appUser);

        }





    }
}
