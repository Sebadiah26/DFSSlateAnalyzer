using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class ImpersonateViewComponent : BaseViewComponent
    {
        private readonly IUserRepository _userRepository;
        //  private readonly AppUser _appUser;
        // private readonly IHttpContextAccessor _httpContextAccessor;


        public ImpersonateViewComponent(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _userRepository = userRepository;



        }

        public async Task<IViewComponentResult> InvokeAsync(string selectedValue)
        {

            SelectList users = null;

            if (!_cache.TryGetValue("AppUsers", out users))
            {
                users = await _userRepository.GetAppUsers();
                _cache.Set("AppUsers", users, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });

            }


            _appUser.Users = users;


            //  var model = new DropDownViewModel()
            //      .Add(users);

            //  _appUser.Users.SelectedValue = selectedValue;
            //  _appUser.Users.DefaultValue = "All";
            //model.SelectedValue = selectedValue;
            //model.DefaultValue = "All";

            // ViewBag.Userdata = model;
            ViewBag.Userdata = _appUser.Users;
            ViewBag.ActiveDirectoryName = _appUser.ActiveDirectoryName;
            ViewBag.ImpersonateUser = _appUser.ImpersonateUser;



            return View(_appUser);



        }



    }
}

