using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;

namespace StaffManagement.Components
{
    public abstract class BaseViewComponent : ViewComponent
    {

        protected readonly AppUser _appUser;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMemoryCache _cache;


        public BaseViewComponent(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser)
        {

            _httpContextAccessor = httpContextAccessor;
            _cache = memoryCache;
            _appUser = appUser;

            //AppUser appuser = new AppUser(_httpContextAccessor);

            //_appUser = appuser;

            //_appUser.GetAppUserData(_httpContextAccessor.HttpContext);



        }

    }
}
