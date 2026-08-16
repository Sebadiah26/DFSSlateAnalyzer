using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffManagement.Core.Repositories.Interfaces
{
    public interface IUnitRepository
    {
        /// <summary>UnitId -&gt; Name lookup for the department/unit filter dropdown. Ports
        /// IUnitRepository.GetUnitsAsSelectlist. More Unit-related methods (GetUnits, building lookups,
        /// etc.) get added here as later phases need them.</summary>
        Task<SelectList> GetUnitsAsSelectlistAsync();
    }
}
