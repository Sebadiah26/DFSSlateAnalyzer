using StaffManagement.Data;

namespace StaffManagement.Core.Repositories.Interfaces
{
    /// <summary>
    /// Ported from StaffManagement/Repositories/Interfaces/IJobRecordRepository.cs, plus the plain
    /// CRUD JobRecordController used to hit accountmanagementContext directly for (normalized here
    /// the same way ContractController's/SystemReviewController's direct DbContext usage was in
    /// earlier phases). GetJobRecords(systemid, jobrecordid) -&gt; JobRecordModel and
    /// EditJobRecord/AddJobRecord(AccountModel, ...) are Employees-area (Phase 8) concerns - not
    /// ported here, since they operate on the same field-diffing flow as AccountController.Edit.
    /// </summary>
    public interface IJobRecordRepository
    {
        /// <summary>Ports GetJobRecordsNeedingReview, minus the HttpContext-built absolute URL (that's
        /// a UI concern - the Blazor page builds the link itself via NavigationManager).</summary>
        Task<List<(string Name, int JobRecordId, int? SystemId)>> GetJobRecordsNeedingReviewAsync();

        Task<int> JobRecordCountAsync(int systemId);
        Task<List<JobRecord>> GetJobRecordsBySystemIdAsync(int systemId);
        Task<JobRecord?> GetJobRecordByIdAsync(int jobRecordId);
        Task<int> CreateAsync(JobRecord jobRecord);
        Task UpdateAsync(JobRecord jobRecord);
        Task DeleteAsync(int jobRecordId);
        Task<bool> ExistsAsync(int jobRecordId);
    }
}
