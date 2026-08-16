using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly IDbContextFactory<accountmanagementContext> _dbFactory;

        public UnitRepository(IDbContextFactory<accountmanagementContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<SelectList> GetUnitsAsSelectlistAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var unitdata = new SelectList(await db.Units
                .ToDictionaryAsync(
                unit => unit.UnitId.ToString(),
                unit => unit.Name), "Key", "Value");

            return unitdata;
        }
    }
}
