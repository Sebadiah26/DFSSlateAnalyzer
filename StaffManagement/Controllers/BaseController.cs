using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Common;
using StaffManagement.Models;
using StaffManagement.Services;

namespace StaffManagement.Controllers
{
    public class BaseController : Controller
    {


        protected readonly IAuditLoggerService _log = null;
        protected readonly IHttpContextAccessor _httpContextAccessor = null;

        protected readonly URLEncrypter _protector;
        protected AppUser _appUser = null;

        public BaseController(IAuditLoggerService log, IHttpContextAccessor contextAccessor, URLEncrypter protector, AppUser appUser)
        {

            _log = log;
            _httpContextAccessor = contextAccessor;
            _protector = protector;
            _appUser = appUser;




        }

        public SelectList SetSelected(SelectList selectlist, string selecteditemvalue)
        {
            //if (selecteditemvalue != "")
            //{
            foreach (SelectListItem item in selectlist)
            {

                if (item.Value == selecteditemvalue)
                { item.Selected = true; }
                else
                {
                    item.Selected = false;
                }


            }

            //}

            return selectlist;
        }



        public string GetSelected(SelectList selectlist, string selecteditemvalue)
        {
            string selecteditem = "";

            if (selecteditemvalue != "")
            {
                foreach (SelectListItem item in selectlist)
                {

                    if (item.Value == selecteditemvalue)
                    { selecteditem = item.Text; }


                }

            }

            return selecteditem;
        }



    }


}

