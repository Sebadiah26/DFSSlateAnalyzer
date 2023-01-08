
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface IUnitRepository
    {

        Task<SelectList> GetUnitsAsSelectlist();











    }
}
