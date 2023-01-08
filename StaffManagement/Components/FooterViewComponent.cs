using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;

namespace StaffManagement.Components
{
    /// <summary>
    /// blah
    /// </summary>
    public class FooterViewComponent : BaseViewComponent
    {
        // private readonly UserRepository _userRepository;
        // private readonly IHttpContextAccessor _httpContextAccessor;
        // private AppUser _appUser = null;

        public FooterViewComponent(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {

        }

        public IViewComponentResult Invoke(string CurrentUser)


        {




            return View(_appUser);

        }





    }
}
