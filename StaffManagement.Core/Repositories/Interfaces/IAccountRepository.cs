using StaffManagement.Core.Models;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories.Interfaces
{
    /// <summary>
    /// Ported from StaffManagement/Repositories/Interfaces/IAccountRepository.cs, backing the
    /// Employees feature area (AccountController). Per the plan's Phase 8 scoping decision, this
    /// binds pages directly to EF entities (Account, JobRecord) plus flat DTOs
    /// (EmployeeSearchResult/EmployeeSearchMatch) instead of porting the source's AccountModel view-
    /// model graph (which existed purely to support MVC's asp-for/SelectList-caching conventions -
    /// dead weight in Blazor). Dropdown lookups return plain Dictionary&lt;TKey,string&gt; instead of
    /// Microsoft.AspNetCore.Mvc.Rendering.SelectList for the same reason.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>Ports AccountRepository.GetPHRSTMismatchedRecordsByEmployee - runs the
        /// Select_PHRST_Mismatched_Employees stored procedure for the given SystemId (0 = all
        /// employees), used by the Home dashboard's mismatch queue widget.</summary>
        Task<List<Select_PHRST_Mismatched_Employees>> GetPHRSTMismatchedRecordsByEmployeeAsync(int systemId);

        /// <summary>Backs JobRecordController.Create's SystemId dropdown.</summary>
        Task<List<int>> GetAllSystemIdsAsync();

        /// <summary>Ports the search-grid half of AccountRepository.GetAccounts(string,string,...).
        /// Returns the current page plus the total match count (source's PagedList.Core.IPagedList).</summary>
        Task<(List<EmployeeSearchResult> Items, int TotalCount)> GetAccountsAsync(
            string firstName, string lastName, string systemId, int unitId, int titleId, int jobCode,
            string personnelCode, int buildingId, string? sortOrder, int pageNumber, int pageSize);

        /// <summary>Ports AccountRepository.GetContractors.</summary>
        Task<List<EmployeeSearchResult>> GetContractorsAsync();

        /// <summary>Loads an Account with Employee/PrimaryUnit/JobRecords included, for Employees/Edit
        /// and Employees/AssignEmployee. Ports the data-loading half of GetAccountByID (the source's
        /// version additionally built an AccountModel with cached dropdown SelectLists - not needed
        /// here per the Phase 8 scoping decision).</summary>
        Task<Account?> GetAccountByIdAsync(int systemId);

        /// <summary>Ports AccountRepository.AddAccount.</summary>
        Task<int> AddAccountAsync(Account account, string comment);

        /// <summary>Ports AccountRepository.EditAccount, simplified to a direct db.Update of the
        /// already-loaded-and-modified entity (the source's version selectively copied fields onto a
        /// freshly `.Find`-ed tracked instance - unnecessary here since Employees/Edit binds the form
        /// straight to the entity GetAccountByIdAsync returned).</summary>
        Task UpdateAccountAsync(Account account, string comment);

        /// <summary>Ports AccountRepository.EditAccountEmployee (AssignEmployee's save action).</summary>
        Task EditAccountEmployeeAsync(int systemId, int employeeId);

        /// <summary>Ports AccountRepository.SearchEmployeesByName.</summary>
        Task<List<EmployeeSearchMatch>> SearchEmployeesByNameAsync(string firstName, string lastName);

        /// <summary>Ports AccountRepository.GetEmployeeJobs - the live hr.EmployeeJob rows for a PHRST
        /// employee, backing Employees/Edit's PHRST reconciliation comparison card. Unlike the source
        /// (which also produced a parallel SelectList keyed by a fragile "#"-delimited composite
        /// string), this returns one flat DTO list keyed by the row's own JobId.</summary>
        Task<List<EmployeeJobSummary>> GetEmployeeJobsAsync(int employeeId);

        /// <summary>Ports AccountRepository.GetAuditLog(systemId, jobRecordId, objectGuid, name,
        /// employeeId) - the account/employee-scoped audit history shown on Employees/Edit, as opposed
        /// to ISystemReviewRepository.GetAuditLogAsync's global admin log (a different source: this
        /// queries the AuditLogView SQL view directly rather than the Select_AuditLog stored
        /// procedure). Any parameter left null/0/Guid.Empty is simply excluded from the OR filter,
        /// matching the source's conditional predicate-building.</summary>
        Task<List<AuditLogViewModel>> GetAuditLogForAccountAsync(int systemId, int? jobRecordId, int? employeeId, Guid? adObjectGuid, string? name);

        /// <summary>Ports AccountRepository.GetEmployees (SystemId -&gt; Name), used for the
        /// "substitute for" dropdown.</summary>
        Task<Dictionary<int, string>> GetEmployeesAsync();

        /// <summary>Ports AccountRepository.GetActiveEmployees.</summary>
        Task<Dictionary<int, string>> GetActiveEmployeesAsync();

        /// <summary>Ports AccountRepository.GetTitles() (all active titles).</summary>
        Task<Dictionary<int, string>> GetTitlesAsync();

        /// <summary>Ports AccountRepository.GetTitlesByTitleGroup.</summary>
        Task<Dictionary<int, string>> GetTitlesByTitleGroupAsync(int titleGroupId);

        /// <summary>Ports AccountRepository.GetTitlebyTitleId.</summary>
        Task<string> GetTitleTextByIdAsync(int titleId);

        /// <summary>Ports AccountRepository.GetAccountTypes.</summary>
        Task<Dictionary<int, string>> GetAccountTypesAsync();

        /// <summary>Ports AccountRepository.GetBuildings().</summary>
        Task<Dictionary<int, string>> GetBuildingsAsync();

        /// <summary>Ports AccountRepository.GetBuildingsByUnit.</summary>
        Task<Dictionary<int, string>> GetBuildingsByUnitAsync(int unitId);

        /// <summary>Ports AccountRepository.GetfsfBuildings (hr.Building, keyed by FsfBuildingId string).</summary>
        Task<Dictionary<string, string>> GetFsfBuildingsAsync();

        /// <summary>Ports AccountRepository.GetJobCodes.</summary>
        Task<Dictionary<int, string>> GetJobCodesAsync();

        /// <summary>Ports AccountRepository.GetPersonnelCodes.</summary>
        Task<Dictionary<string, string>> GetPersonnelCodesAsync();
    }
}
