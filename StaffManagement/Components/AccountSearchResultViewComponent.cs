using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class AccountSearchResultViewComponent : BaseViewComponent
    {
        private readonly IAccountRepository _accountRepository;
        //private readonly AppUser _appUser;
        //private readonly IHttpContextAccessor _httpContextAccessor;


        public AccountSearchResultViewComponent(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _accountRepository = accountRepository;





        }
        public async Task<IViewComponentResult> InvokeAsync(string accountsearchfirstname, string accountsearchlastname, string mode = "Default")
        {
            //use what is entered in first and last name textboxes
            // if mode is "Select", go to Select template rather than default

            var accountsearchdata = await _accountRepository.GetAccounts(accountsearchfirstname, accountsearchlastname);

            TempData["ResultCount"] = accountsearchdata.Count.ToString();

            switch (mode)
            {

                case "Select":

                    return View("Select", accountsearchdata);

                default:
                    return View(accountsearchdata);


            }
        }
    }
}
