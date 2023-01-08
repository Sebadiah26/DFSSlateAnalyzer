//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using StaffManagement.Data;
//using StaffManagement.Repositories;
//using StaffManagement.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Extensions.Caching.Memory;
//using StaffManagement.Repositories.Interfaces;

//namespace StaffManagement.Components
//{
//    public class AvailableJobTitlesViewComponent : BaseViewComponent
//    {
//        private readonly IAccountRepository _accountRepository;
//        //private readonly AppUser _appUser;
//        //private readonly IHttpContextAccessor _httpContextAccessor;


//        public AvailableJobTitlesViewComponent(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
//        {
//            _accountRepository = accountRepository;
//           // _httpContextAccessor = httpContextAccessor;


//            //AppUser appuser = new AppUser(_httpContextAccessor);

//            //_appUser = appuser;
           
//            //_appUser.GetAppUserData(_httpContextAccessor.HttpContext);

          

//        }
//        //public async Task<IViewComponentResult>  InvokeAsync(int id)
//        //{
//        //   // var jobtitlelist = await _accountRepository.GetAvailableJobTitles();


          

       

//        //    var model = new DropDownViewModel()

//        //        .Add(await _accountRepository.GetAvailableJobTitles());
            

//        //  //  model.SelectedValue = selectedValue;
//        //  // model.DefaultValue = "All";

//        //    ViewBag.JobTitles = model;
//        //  //  ViewBag.ActiveDirectoryName = _appUser.ActiveDirectoryName;
//        //  //  ViewBag.ImpersonateUser = _appUser.ImpersonateUser;

//        //    return View(model);

//        //}
//    }
//}
