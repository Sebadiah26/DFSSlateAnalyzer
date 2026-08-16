using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    /// <summary>Ported from StaffManagement/Repositories/UserRepository.cs (GetAppUsers) plus the
    /// identity/permission-resolution queries that used to live directly on AppUser (GetEmployeeID,
    /// GetUserLevels, GetEmployee, GetNeedsReviewCount) - moved here so CurrentUserService
    /// (StaffManagement.Blazor) can resolve identity through a repository instead of holding its own
    /// DbContext. Methods are async versions of the originals (all were synchronous EF calls).
    ///
    /// Takes IDbContextFactory rather than accountmanagementContext directly, and creates a fresh
    /// context per method call - see the comment on AddStaffManagementCoreClasses' AddDbContextFactory
    /// call for why a shared scoped context isn't safe here.</summary>
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<accountmanagementContext> _dbFactory;

        public UserRepository(IDbContextFactory<accountmanagementContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<SelectList> GetAppUsersAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            IQueryable<User> query = db.Users
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

        public async Task<int> GetSystemIdByActiveDirectoryNameAsync(string samAccountName)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = await db.Users
                   .Include(user => user.Account)
                   .Where(user => user.SamaccountName == samAccountName)
                   .SingleOrDefaultAsync();

            return query?.Account != null ? query.Account.SystemId : 0;
        }

        public async Task<string?> ResolveSamAccountNameAsync(string emailOrUsername)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var user = await db.Users
                .Where(u => !u.IsDeleted && !u.IsDisabled)
                .Where(u => u.Mail == emailOrUsername
                         || u.UserPrincipalName == emailOrUsername
                         || u.SamaccountName == emailOrUsername)
                .FirstOrDefaultAsync();

            return user?.SamaccountName;
        }

        public async Task<List<string>> GetUserLevelsAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            // PermissionRole.PermissionLevelRole is modeled as a single reference, not a collection -
            // a genuine gap in accountmanagementContext.cs's relationship configuration (see
            // TestDatabaseSeeder's comments on the same gap), even though one role legitimately grants
            // many permission levels (a synthetic unique index enforcing 1:1 had to be dropped in test
            // mode for exactly this reason). Going through that navigation here would silently collapse
            // every role down to at most one resolvable level. Joining on the raw FK columns instead
            // (PermissionRoleID/PermissionLevelID) bypasses the broken navigation and returns every
            // level actually granted to the role, without touching the shared EF model configuration.
            return await (from permission in db.Permission
                          join role in db.PermissionRole on permission.PermissionRoleID equals role.PermissionRoleID
                          join levelRole in db.PermissionLevelRole on role.PermissionRoleID equals levelRole.PermissionRoleID
                          join level in db.PermissionLevel on levelRole.PermissionLevelID equals level.PermissionLevelID
                          where permission.SystemId == systemId
                                && permission.Active == true
                                && role.Active == true
                                && levelRole.Active == true
                                && level.Active == true
                          select level.PermissionLevelName)
                          .ToListAsync();
        }

        public async Task<(int PrimaryUnitId, int PermissionRoleId)?> GetEmployeeSummaryAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = await db.Users
                   .Include(user => user.Account)
                   .ThenInclude(account => account.Permission)
                   .Include(user => user.Account)
                   .ThenInclude(account => account.AccountUnit)
                   .ThenInclude(accountunit => accountunit.Unit)
                   .Where(user => user.Account.SystemId == systemId)
                   .Where(user => user.Account.Permission.SystemId == systemId)
                   .Where(user => user.Account.Permission.Active == true)
                   .FirstOrDefaultAsync();

            if (query?.Account.AccountUnit == null)
            {
                return null;
            }

            return (query.Account.AccountUnit.UnitId, query.Account.Permission.PermissionRoleID);
        }

        public async Task<int> GetNeedsReviewCountAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.JobRecord
                    .Where(jobrecord => jobrecord.IsPHRST == true)
                    .Where(jobrecord => jobrecord.NeedsReview == true)
                    .CountAsync();
        }
    }
}
