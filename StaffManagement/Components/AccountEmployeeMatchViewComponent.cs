using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class AccountEmployeeMatchViewComponent : BaseViewComponent
    {
        private readonly IAccountRepository _accountRepository;
        //private readonly AppUser _appUser;
        //private readonly IHttpContextAccessor _httpContextAccessor;


        public AccountEmployeeMatchViewComponent(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _accountRepository = accountRepository;





        }
        public async Task<IViewComponentResult> InvokeAsync(string employeesearchfirstname, string employeesearchlastname, string mode = "Default")
        {
            //use what is entered in first and last name textboxes
            // if mode is "Search", go to Search template rather than default

            var employeesearchdata = await _accountRepository.SearchEmployeesByName(employeesearchfirstname, employeesearchlastname);

            TempData["ResultCount"] = employeesearchdata.Count.ToString();

            switch (mode)
            {

                case "Search":

                    return View("Search", employeesearchdata);

                case "Default":

                    return View(employeesearchdata);

                default:
                    return View(employeesearchdata);


            }

        }
    }
}
