using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class EmployeeFilterViewComponent : BaseViewComponent
    {
        private readonly IUnitRepository _unitRepository;



        public EmployeeFilterViewComponent(IUnitRepository unitRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _unitRepository = unitRepository;



        }

        public async Task<IViewComponentResult> InvokeAsync(string selectedValue)
        {

            SelectList units = null;

            if (!_cache.TryGetValue("Units", out units))
            {
                units = await _unitRepository.GetUnitsAsSelectlist();
                _cache.Set("Units", units, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });

            }


            var model = new DropDownViewModel()
                .Add(units);

            model.SelectedValue = selectedValue;
            model.DefaultValue = "0";

            ViewBag.Unitdata = model;

            ViewBag.SelectedUnitId = _appUser.SelectedUnitId;

            return View(model);



        }



    }
}

