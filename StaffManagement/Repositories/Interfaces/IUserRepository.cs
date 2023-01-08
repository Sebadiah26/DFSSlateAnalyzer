
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {

        Task<SelectList> GetAppUsers();











    }
}
