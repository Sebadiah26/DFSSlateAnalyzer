using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;

namespace StaffManagement.Components
{
    /// <summary>
    /// blah
    /// </summary>
    public class BreadcrumbViewComponent : BaseViewComponent
    {


        public BreadcrumbViewComponent(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {

        }

        public IViewComponentResult Invoke()


        {




            return View(_appUser);

        }





    }
}
