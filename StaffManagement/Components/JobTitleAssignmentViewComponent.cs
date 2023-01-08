using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using StaffManagement.Repositories;
using StaffManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Repositories.Interfaces;

namespace StaffManagement.Components
{
    public class JobTitleAssignmentViewComponent : BaseViewComponent
    {
        private readonly IEmployeeRepository _employeeRepository;
       // private readonly AppUser _appUser;
       // private readonly IHttpContextAccessor _httpContextAccessor;


        public JobTitleAssignmentViewComponent(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _employeeRepository = employeeRepository;
          //  _httpContextAccessor = httpContextAccessor;


           // AppUser appuser = new AppUser(_httpContextAccessor);

           // _appUser = appuser;

           // _appUser.GetAppUserData(_httpContextAccessor.HttpContext);




           
        }
        //public async Task<IViewComponentResult>  InvokeAsync(int systemid)
        //{
        //   // var jobtitleassignmentinfo = await _employeeRepository.GetEmployeeJobTitleAssignmentInfo( systemid );



        //    //return View(jobtitleassignmentinfo);
        //    return View();
        //}
    }
}
