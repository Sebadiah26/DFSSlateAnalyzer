using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly accountmanagementContext _db = null;


        public UnitRepository(accountmanagementContext context)
        {
            _db = context;
        }


        public async Task<List<UnitModel>> GetUnits()
        {
            return await _db.Units.Select(x => new UnitModel()
            {
                UnitId = x.UnitId,
                Name = x.Name,

            }).ToListAsync();
        }

        public async Task<SelectList> GetUnitsAsSelectlist()
        {



            var unitdata = new SelectList(await _db.Units
                .ToDictionaryAsync(
                unit => unit.UnitId.ToString(),
                unit => unit.Name), "Key", "Value");



            return unitdata;
        }


    }
}