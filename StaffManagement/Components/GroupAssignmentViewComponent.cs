using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class GroupAssignmentViewComponent : BaseViewComponent
    {
        private readonly IEmployeeRepository _employeeRepository;
        // private readonly AppUser _appUser;
        //private readonly IHttpContextAccessor _httpContextAccessor;


        public GroupAssignmentViewComponent(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _employeeRepository = employeeRepository;


        }
        public async Task<IViewComponentResult> InvokeAsync(int viewid, int systemid, string page)
        {

            switch (page)
            {
                // case "Default":

                //  var employeelist = await _employeeRepository.GetEmployees(viewid, _appUser.PrimaryUnitId, _appUser.JobCode, _appUser.PermissionRoleId);
                //  return View(page, employeelist);

                case "EmployeeGroups":
                    var grouplist = await _employeeRepository.GetEmployeeGroups(systemid);
                    return View(page, grouplist);

                case "EmployeeStaffGroups":
                    var staffgrouplist = await _employeeRepository.GetEmployeeStaffGroups(systemid);
                    return View(page, staffgrouplist);

                default:
                    return View();

            }
        }



    }
}
