using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using StaffManagement.Repositories;
using StaffManagement.Models;
using Microsoft.AspNetCore.Http;
using StaffManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace StaffManagement.Components
{
    public class SiteTextViewComponent : BaseViewComponent
    {

        private readonly accountmanagementContext _db;

        public SiteTextViewComponent(accountmanagementContext context, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {


            _db = context;



        }
        //public async Task<IViewComponentResult>  InvokeAsync(int sitetextid)
        //{
        //    var sitetext = await _db.SiteText
        //        .Where(x => x.SiteTextId == sitetextid)
        //        .FirstOrDefaultAsync();
          

        //    return View(sitetext); 

        //}
    }
}
