using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Models;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    /// <summary>Ported from StaffManagement/Repositories/AccountRepository.cs - see
    /// IAccountRepository's doc comment for the Phase 8 scoping decision (entities+DTOs instead of
    /// AccountModel's full view-model graph).</summary>
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(IDbContextFactory<accountmanagementContext> dbFactory,
                                  IAuditLoggerService log,
                                  Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
                                  ICurrentUserContext currentUser)
            : base(dbFactory, log, memoryCache, currentUser)
        {
        }

        public async Task<List<Select_PHRST_Mismatched_Employees>> GetPHRSTMismatchedRecordsByEmployeeAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Select_PHRST_Mismatched_Employees
                .FromSqlInterpolated($"EXEC Select_PHRST_Mismatched_Employees {systemId}")
                .ToListAsync();
        }

        public async Task<List<int>> GetAllSystemIdsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Accounts.Select(a => a.SystemId).ToListAsync();
        }

        private IQueryable<EmployeeSearchResult> BuildSearchQuery(accountmanagementContext db)
        {
            return from a in db.Accounts
                   join jr in db.JobRecord_Ordered.Where(e => e.Order == 1) on a.SystemId equals jr.SystemId into ajr
                   from jr in ajr.DefaultIfEmpty()
                   join a2 in db.Accounts on jr.SubstituteForId equals a2.SystemId into jra2
                   from a2 in jra2.DefaultIfEmpty()
                   join at in db.AccountTypes on a.AccountTypeId equals at.AccountTypeId into aat
                   from at in aat.DefaultIfEmpty()
                   join t in db.Titles on jr.TitleId equals t.TitleId into jrt
                   from t in jrt.DefaultIfEmpty()
                   join tg in db.TitleGroups on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                   from tg in jrtg.DefaultIfEmpty()
                   join u in db.Units on jr.UnitId equals u.UnitId into jru
                   from u in jru.DefaultIfEmpty()
                   join bg in db.Buildings on jr.BuildingId equals bg.BuildingId into jrbg
                   from bg in jrbg.DefaultIfEmpty()
                   join e in db.Employees on a.EmployeeId equals e.EmployeeId into ae
                   from e in ae.DefaultIfEmpty()
                   join us in db.Users on a.AdobjectGuid equals us.ObjectGuid into aus
                   from us in aus.DefaultIfEmpty()
                   join ou in db.OrganizationalUnits on us.OrganizationalUnitGuid equals ou.ObjectGuid into usou
                   from ou in usou.DefaultIfEmpty()
                   select new EmployeeSearchResult
                   {
                       SystemId = a.SystemId,
                       EmployeeId = a.EmployeeId ?? 0,
                       FirstName = (a.FirstName == null || a.FirstName == "") ? (e.FirstName ?? (us.GivenName ?? "")) : a.FirstName,
                       LastName = (a.LastName == null || a.LastName == "") ? (e.LastName ?? (us.Surname ?? "")) : a.LastName,
                       AccountTypeId = at.AccountTypeId,
                       EmployeeType = at.Description ?? "",
                       JobTitle = (a.JobTitle == null || a.JobTitle == "") ? (e.JobTitle ?? (us.JobTitle ?? "")) : a.JobTitle,
                       Title = t.Text ?? "",
                       TitleGroup = tg.Description ?? "",
                       Department = u.Name ?? "",
                       Building = bg.Name ?? "",
                       AccountLastUpdate = a.LastUpdate,
                       AccountLastUpdateUser = a.LastUpdateUser ?? "",
                       JobRecordLastUpdate = jr.LastUpdate,
                       JobRecordLastUpdateUser = jr.LastUpdateUser ?? "",
                       // e is left-joined (DefaultIfEmpty) - null-shaped for an account with no linked
                       // hr.Employee, and Employee.IsDeleted is a non-nullable bool column; same hazard
                       // as UserIsDisabled/UserIsDeleted below.
                       IsDeleted = e != null && e.IsDeleted,
                       IsActive = a.IsActive ?? true,
                       AlternateFirstName = a.AlternateFirstName ?? "",
                       AlternateLastName = a.AlternateLastName ?? "",
                       UsernameOverride = a.UsernameOverride ?? "",
                       PrimaryUnitId = jr.UnitId ?? 0,
                       TitleId = jr.TitleId ?? 0,
                       Jobcode = e.Jobcode,
                       PersonnelCode = e.PersonnelCode ?? "",
                       BuildingId = jr.BuildingId ?? 0,
                       Source = a.Source ?? "",
                       SubstituteFor = (a2.FirstName ?? "") + " " + (a2.LastName ?? ""),
                       // us comes from a LEFT JOIN (DefaultIfEmpty) - for an account with no linked AD
                       // user (AdobjectGuid null, or no matching ad.User row) every column of us reads
                       // back as SQL NULL, and User.IsDisabled/IsDeleted are non-nullable bool columns,
                       // so reading them directly throws "Nullable object must have a value" the moment
                       // such an account shows up in this query. Genuine pre-existing bug, surfaced by
                       // seeding a second test account with no AD user - not something every production
                       // account happened to trigger, but a real unlinked/contractor account could.
                       UserIsDisabled = us != null && us.IsDisabled,
                       UserIsDeleted = us != null && us.IsDeleted,
                       OrganizationalUnit = ou.OrganizationalUnitName,
                       StartDate = jr.StartDate,
                       EndDate = jr.EndDate,
                       AccountExpires = us.AccountExpires
                   };
        }

        public async Task<(List<EmployeeSearchResult> Items, int TotalCount)> GetAccountsAsync(
            string firstName, string lastName, string systemId, int unitId, int titleId, int jobCode,
            string personnelCode, int buildingId, string? sortOrder, int pageNumber, int pageSize)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = BuildSearchQuery(db);

            if (systemId != "" && int.TryParse(systemId, out var number))
            {
                query = query.Where(x => x.SystemId.Equals(number) || x.EmployeeId.Equals(number));
            }
            else
            {
                if (firstName != "")
                {
                    query = query.Where(x => x.FirstName.Contains(firstName)
                              || x.AlternateFirstName.Contains(firstName)
                              || x.UsernameOverride.Contains(firstName)
                              || db.Soundex(x.FirstName) == db.Soundex(firstName));
                }

                if (lastName != "")
                {
                    query = query.Where(x => x.LastName.Contains(lastName)
                            || x.AlternateLastName.Contains(lastName)
                            || x.UsernameOverride.Contains(lastName)
                            || db.Soundex(x.LastName) == db.Soundex(lastName));
                }

                if (unitId != 0) query = query.Where(x => x.PrimaryUnitId.Equals(unitId));
                if (titleId != 0) query = query.Where(x => x.TitleId.Equals(titleId));
                if (jobCode != 0) query = query.Where(x => x.Jobcode.Equals(jobCode));
                if (personnelCode != "") query = query.Where(x => x.PersonnelCode.Equals(personnelCode));
                if (buildingId != 0) query = query.Where(x => x.BuildingId.Equals(buildingId));
            }

            query = query.Where(x => x.Source != "STU" && x.Source != null);
            query = query.Where(x =>
                        x.OrganizationalUnit == null ||
                        x.OrganizationalUnit == "_Disabled User Accounts" ||
                        x.OrganizationalUnit == "Student Support" ||
                        x.OrganizationalUnit == "Users");

            query = sortOrder switch
            {
                " Jobtitle_Asc_Sort" => query.OrderBy(s => s.JobTitle),
                " Jobtitle_Desc_Sort" => query.OrderByDescending(s => s.JobTitle),
                " TitleGroup_Asc_Sort" => query.OrderBy(s => s.TitleGroup).ThenBy(s => s.Title),
                " TitleGroup_Desc_Sort" => query.OrderByDescending(s => s.TitleGroup).ThenByDescending(s => s.Title),
                " Title_Asc_Sort" => query.OrderBy(s => s.Title),
                " Title_Desc_Sort" => query.OrderByDescending(s => s.Title),
                " AccountType_Asc_Sort" => query.OrderBy(s => s.EmployeeType),
                " AccountType_Desc_Sort" => query.OrderByDescending(s => s.EmployeeType),
                " FirstName_Asc_Sort" => query.OrderBy(s => s.FirstName),
                " FirstName_Desc_Sort" => query.OrderByDescending(s => s.FirstName),
                " LastName_Asc_Sort" => query.OrderBy(s => s.LastName),
                " LastName_Desc_Sort" => query.OrderByDescending(s => s.LastName),
                _ => query.OrderBy(s => s.SystemId),
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<EmployeeSearchResult>> GetContractorsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = BuildSearchQuery(db);

            query = query.Where(x => x.Source != "STU" && x.Source != null);
            query = query.Where(x => (x.AccountExpires ?? DateTime.Now) > Core.Common.Settings.SixMonthsAgo);
            query = query.Where(x => !x.FirstName.StartsWith("BSD"));
            query = query.Where(x =>
                        x.OrganizationalUnit == null ||
                        x.OrganizationalUnit == "Student Support" ||
                        x.OrganizationalUnit == "Users");
            query = query.Where(x =>
                       x.AccountTypeId == (int)EnumAccountTypeDescription.Contractor ||
                       x.AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub ||
                       x.EmployeeId == 0);

            return await query.ToListAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Accounts
                .Include(a => a.Employee)
                .Include(a => a.PrimaryUnit)
                .Include(a => a.JobRecords)
                .FirstOrDefaultAsync(a => a.SystemId == systemId);
        }

        public async Task<int> AddAccountAsync(Account account, string comment)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            comment = "for [SystemId]: [" + account.SystemId + "] " + comment;
            _log.Log((int)ActionTypes.Create, "dbo.Account", account.SystemId.ToString(), comment);

            return account.SystemId;
        }

        public async Task UpdateAccountAsync(Account account, string comment)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            db.Accounts.Update(account);
            await db.SaveChangesAsync();

            _log.Log((int)ActionTypes.Edit, "dbo.Account", account.SystemId.ToString(), comment);
        }

        public async Task EditAccountEmployeeAsync(int systemId, int employeeId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var accountToEdit = await db.Accounts.FindAsync(systemId);
            if (accountToEdit == null) return;

            accountToEdit.EmployeeId = employeeId;
            accountToEdit.LastUpdateUser = _currentUser.CurrentUser;
            accountToEdit.LastUpdate = DateTime.Now;

            db.Update(accountToEdit);
            await db.SaveChangesAsync();
        }

        public async Task<List<EmployeeSearchMatch>> SearchEmployeesByNameAsync(string firstName, string lastName)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return new List<EmployeeSearchMatch>();
            }

            var query = from e in db.Employees
                        join u in db.Units on e.UnitId equals u.UnitId
                        join a in db.Accounts on e.EmployeeId equals a.EmployeeId into grouping
                        from a in grouping.DefaultIfEmpty()
                        select new EmployeeSearchMatch
                        {
                            FirstName = e.FirstName ?? "",
                            LastName = e.LastName ?? "",
                            JobTitle = e.JobTitle ?? "",
                            IsDeleted = e.IsDeleted,
                            Department = u.Name ?? "",
                            SystemId = a.SystemId,
                            EmployeeId = e.EmployeeId,
                            EndDate = e.EndDate,
                            LastUpdate = e.LastUpdate
                        };

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(e => e.FirstName.Contains(firstName) || db.Soundex(e.FirstName) == db.Soundex(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(e => e.LastName.Contains(lastName) || db.Soundex(e.LastName) == db.Soundex(lastName));
            }

            return await query.OrderBy(x => x.LastName).Take(50).ToListAsync();
        }

        public async Task<List<EmployeeJobSummary>> GetEmployeeJobsAsync(int employeeId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await (from ej in db.EmployeeJobs
                          join b in db.Hr_Buildings on ej.FsfBuildingId equals b.FsfBuildingId into ejb
                          from b in ejb.DefaultIfEmpty()
                          join pc in db.PersonnelCodes on ej.PersonnelCode equals pc.Personnel_Code into ejpc
                          from pc in ejpc.DefaultIfEmpty()
                          join jc in db.Jobcodes on ej.Jobcode equals jc.JobcodeId into ejjc
                          from jc in ejjc.DefaultIfEmpty()
                          join u in db.Units on ej.UnitId equals u.UnitId into eju
                          from u in eju.DefaultIfEmpty()
                          where ej.EmployeeId == employeeId
                          select new EmployeeJobSummary
                          {
                              EmployeeId = ej.EmployeeId,
                              JobId = ej.JobId,
                              FsfBuildingId = ej.FsfBuildingId,
                              PersonnelCode = ej.PersonnelCode,
                              Jobcode = ej.Jobcode,
                              UnitId = ej.UnitId,
                              EffectiveDate = ej.EffectiveDate,
                              BuildingDescription = b.Description ?? "",
                              PersonnelCodeDescription = pc.Description ?? "",
                              JobcodeDescription = jc.Description ?? "",
                              UnitName = u.Name ?? ""
                          }).ToListAsync();
        }

        public async Task<List<AuditLogViewModel>> GetAuditLogForAccountAsync(int systemId, int? jobRecordId, int? employeeId, Guid? adObjectGuid, string? name)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var systemIdText = systemId.ToString();
            var jobRecordIdText = jobRecordId?.ToString();
            var employeeIdText = employeeId?.ToString();
            var adObjectGuidText = adObjectGuid?.ToString();
            var hasJobRecordId = jobRecordId.HasValue && jobRecordId.Value != 0;
            var hasEmployeeId = employeeId.HasValue && employeeId.Value != 0;
            var hasAdObjectGuid = adObjectGuid.HasValue && adObjectGuid.Value != Guid.Empty;
            var hasName = !string.IsNullOrEmpty(name);

            var query = db.AuditLogView.Where(a =>
                (a.ReferenceTable == "dbo.Account" && a.ReferenceID == systemIdText) ||
                (hasJobRecordId && a.ReferenceTable == "dbo.JobRecord" && a.ReferenceID == jobRecordIdText) ||
                (hasEmployeeId && (a.ReferenceTable == "hr.Employee" || a.ReferenceTable == "hr.EmployeeJob") && a.ReferenceID == employeeIdText) ||
                (hasAdObjectGuid && a.ReferenceTable == "ad.User" && a.ReferenceID == adObjectGuidText) ||
                (hasName && a.Employee == name));

            var results = await query.OrderByDescending(a => a.LogDate).ToListAsync();

            return results.Select(a => new AuditLogViewModel
            {
                LogId = a.LogId,
                LogDate = a.LogDate,
                ReferenceTable = a.ReferenceTable,
                ReferenceID = a.ReferenceID,
                Employee = a.Employee,
                ActionType = a.ActionType,
                ApplicationUser = a.ApplicationUser,
                ImpersonatedUser = a.ImpersonatedUser,
                Comment = a.Comment
            }).ToList();
        }

        public async Task<Dictionary<int, string>> GetEmployeesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Accounts.Where(a => a.Source != "STU").Where(a => a.LastName != null)
                .OrderBy(a => a.LastName)
                .ToDictionaryAsync(a => a.SystemId, a => a.Name);
        }

        public async Task<Dictionary<int, string>> GetActiveEmployeesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Accounts
                .Where(a => a.EmployeeId != null).Where(a => a.Source != "STU").Where(a => a.LastName != null)
                .Where(a => a.Employee!.IsDeleted == false)
                .OrderBy(a => a.JobTitle)
                .ToDictionaryAsync(a => a.SystemId, a => a.JobTitle + " " + a.PrimaryUnitId + " " + a.Name);
        }

        public async Task<Dictionary<int, string>> GetTitlesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Titles.Where(x => x.Active).OrderBy(x => x.Text).ToDictionaryAsync(m => m.TitleId, m => m.Text);
        }

        public async Task<Dictionary<int, string>> GetTitlesByTitleGroupAsync(int titleGroupId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.TitleGroup_Titles
                .Include(x => x.Title).Include(x => x.TitleGroup)
                .Where(x => x.TitleGroupId == titleGroupId && x.Active && x.Title.Active && x.TitleGroup.Active)
                .OrderBy(x => x.Title.Text)
                .ToDictionaryAsync(m => m.TitleId, m => m.Title.Text);
        }

        public async Task<string> GetTitleTextByIdAsync(int titleId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var title = await db.Titles.FirstOrDefaultAsync(x => x.TitleId == titleId);
            return title?.Text ?? "";
        }

        public async Task<Dictionary<int, string>> GetAccountTypesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.AccountTypes.Where(x => x.IsStaff).OrderBy(x => x.AccountTypeId)
                .ToDictionaryAsync(m => m.AccountTypeId, m => m.Description);
        }

        public async Task<Dictionary<int, string>> GetBuildingsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Buildings.OrderBy(x => x.Name).ToDictionaryAsync(m => m.BuildingId, m => m.Name);
        }

        public async Task<Dictionary<int, string>> GetBuildingsByUnitAsync(int unitId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.UnitBuildings
                .Include(x => x.Building).Include(x => x.Unit)
                .Where(x => x.UnitId == unitId && x.Active)
                .OrderBy(x => x.Building.Name)
                .ToDictionaryAsync(m => m.BuildingId, m => m.Building.Name);
        }

        public async Task<Dictionary<string, string>> GetFsfBuildingsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Hr_Buildings.OrderBy(x => x.Description).ToDictionaryAsync(m => m.FsfBuildingId, m => m.Description);
        }

        public async Task<Dictionary<int, string>> GetJobCodesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Jobcodes.OrderBy(x => x.Description)
                .ToDictionaryAsync(m => m.JobcodeId, m => m.Description + " (" + m.JobcodeId + ")");
        }

        public async Task<Dictionary<string, string>> GetPersonnelCodesAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.PersonnelCodes.OrderBy(x => x.Description)
                .ToDictionaryAsync(m => m.Personnel_Code ?? "", m => m.Description + " (" + m.Personnel_Code + ")");
        }
    }
}
