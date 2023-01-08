using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class MismatchedEmployeesViewComponent : BaseViewComponent
    {
        private readonly IAccountRepository _accountRepository;



        public MismatchedEmployeesViewComponent(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _accountRepository = accountRepository;





        }
        public async Task<IViewComponentResult> InvokeAsync(int systemid)
        {

            var employeeData = await _accountRepository.GetPHRSTMismatchedRecordsByEmployee(systemid);


            foreach (var employee in employeeData)
            {
                if (employee.Reason == "New")
                {
                    // Save value in session so that employeeid is not sent through URL 
                    _appUser.SaveEncryptedValue(_httpContextAccessor.HttpContext, employee.Slug.ToString(), employee.EmployeeId.ToString());
                }

            }


            return View(employeeData);




        }
    }
}
