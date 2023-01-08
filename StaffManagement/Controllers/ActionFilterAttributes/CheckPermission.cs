using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using StaffManagement.Models;

namespace StaffManagement.Controllers
{

    public class CheckPermission : ActionFilterAttribute
    {
        private AppUser _appUser = null;




        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var httpcontext = context.HttpContext;

            AppUser appuser = new AppUser(httpcontext);
            appuser.GetAppUserData(httpcontext);
            _appUser = appuser;

            if (_appUser.IsInLevel("Can View System Review Page") == false)
            {
                httpcontext.Session.SetString("SecurityMessage", "You do not have permission to this page.");
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));

            }



            base.OnActionExecuting(context);
        }


    }
}
