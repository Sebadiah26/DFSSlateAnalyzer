using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaffManagement.Common;
using StaffManagement.Models;
using StaffManagement.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace StaffManagement.Controllers
{

    /// <summary>
    /// Contains "GET" and "POST" action methods  referenced in "Views/Home"
    /// </summary>
    [DisplayName("Home")]
    public class HomeController : BaseController
    {

        /// <summary>
        /// Get AuditLog and HttpContext from base class
        /// </summary>
        /// <param name="log"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="protector"></param>

        public HomeController(IAuditLoggerService log, IHttpContextAccessor contextAccessor, URLEncrypter protector, AppUser appUser) : base(log, contextAccessor, protector, appUser)
        {

        }

        /// <summary>
        /// GET action method from "Views/Home/Index.cshtml" Checks logged in user, saves to session as necessary.
        /// </summary>
        /// <returns>AppUser</returns>
        [DisplayName("Menu")]
        public IActionResult Index()

        {



            if (_httpContextAccessor.HttpContext.Session.GetString("SecurityMessage") != "")
            {
                TempData["SecurityMessage"] = _httpContextAccessor.HttpContext.Session.GetString("SecurityMessage");
                _httpContextAccessor.HttpContext.Session.SetString("SecurityMessage", "");
            }


            if (_appUser.CurrentUser != _appUser.ActiveDirectoryName)
            {
                _appUser.ImpersonateSystemId = _appUser.GetEmployeeID(_appUser.CurrentUser);

                _appUser.UserLevels = _appUser.GetUserLevels(_appUser.ImpersonateSystemId);

                _appUser.GetEmployee(_appUser.ImpersonateSystemId);
            }




            ViewBag.ActiveDirectoryName = _appUser.ActiveDirectoryName;
            ViewBag.SystemID = _appUser.SystemId;
            ViewBag.UserLevels = _appUser.UserLevels;
            ViewBag.CurrentUser = _appUser.CurrentUser;

            _appUser.SetAppUserToSession(_httpContextAccessor.HttpContext);



            return View(_appUser);
        }



        /// <summary>
        /// POST action method for "Views/Home/Index.cshtml" - sets selected unit and logged in user as needed
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DisplayName("Menu")]
        [HttpPost]
        public IActionResult Index(int value)
        {
            // var selectedValue = dropDownViewModel.SelectedValue;

            var selectedUnitValue = Request.Form["Units"].ToString();

            var selectedUserValue = Request.Form["ImpersonateUser"];

            var selectedUserText = Request.Form["linkeduserInput"];


            if (selectedUnitValue != null && selectedUnitValue != "" && Convert.ToInt32(selectedUnitValue) != _appUser.SelectedUnitId)
            {

                _appUser.SelectedUnitId = Convert.ToInt32(selectedUnitValue);
                ViewBag.SelectedUnitId = _appUser.SelectedUnitId;
                _appUser.SetAppUserToSession(_httpContextAccessor.HttpContext);
                TempData["ResultMessage"] = "Your Department filter has been changed.";
                return RedirectToAction(nameof(Index));
            }
            else

            {
                _appUser.ImpersonateUser = selectedUserValue.ToString();
                _appUser.SelectedUser = selectedUserText.ToString();

                ViewBag.ImpersonateUser = _appUser.ImpersonateUser;
                ViewBag.SelectedUser = _appUser.SelectedUser;

                if (_appUser.CurrentUser != _appUser.ActiveDirectoryName)
                {
                    _appUser.ImpersonateSystemId = _appUser.GetEmployeeID(_appUser.CurrentUser);

                    _appUser.UserLevels = _appUser.GetUserLevels(_appUser.ImpersonateSystemId);

                    _appUser.GetEmployee(_appUser.ImpersonateSystemId);
                    _appUser.NeedsReviewCount = _appUser.GetNeedsReviewCount(_appUser.ImpersonateSystemId);
                }
                else
                {
                    if (_appUser.CurrentUser != "")
                    {
                        _appUser.SystemId = _appUser.GetEmployeeID(_appUser.CurrentUser);
                        _appUser.UserLevels = _appUser.GetUserLevels(_appUser.SystemId);

                        _appUser.GetEmployee(_appUser.SystemId);
                        _appUser.NeedsReviewCount = _appUser.GetNeedsReviewCount(_appUser.SystemId);
                    }
                }

                ViewBag.ActiveDirectoryName = _appUser.ActiveDirectoryName;
                ViewBag.SystemID = _appUser.SystemId;
                ViewBag.UserLevels = _appUser.UserLevels;
                ViewBag.CurrentUser = _appUser.CurrentUser;
                ViewBag.NeedsReviewCount = _appUser.NeedsReviewCount;

                _appUser.SetAppUserToSession(_httpContextAccessor.HttpContext);
            }



            return View(_appUser);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
