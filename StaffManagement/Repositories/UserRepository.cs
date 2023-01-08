using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Data;
using StaffManagement.Repositories.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly accountmanagementContext _db = null;


        public UserRepository(accountmanagementContext context)
        {
            _db = context;
        }

        public async Task<SelectList> GetAppUsers()
        {

            IQueryable<User> query = _db.Users
                        .Include(user => user.Account).ThenInclude(account => account.Permission).ThenInclude(permission => permission.PermissionRole)
                        .Where(user => user.Account.Permission.Active.Equals(true));


            query = query.Where(user => user.IsDeleted.Equals(false));
            query = query.Where(user => user.IsDisabled.Equals(false));
            query = query.Where(user => user.JobTitle != "Student");
            query = query.OrderBy(user => user.Account.Permission.PermissionRole.PermissionRoleID).ThenBy(user => user.Account.LastName);

            var userdata = new SelectList(await query
                .ToDictionaryAsync(user =>
                user.SamaccountName,
                user => user.Account.FirstName + " "
                         + user.Account.LastName + " ("
                        + user.Account.Permission.PermissionRole.PermissionRoleName + ")"), "Key", "Value");



            return userdata;
        }
    }
}
