using StaffManagement.Core.Models;

namespace StaffManagement.Core.Repositories.Interfaces
{
    /// <summary>
    /// Ported incrementally from StaffManagement/Repositories/Interfaces/ISystemReviewRepository.cs.
    /// Covers the JobTitleAdmin and AuditLog pages (both self-contained, no IAccountRepository
    /// dependency). The Group-membership matching methods (GetStaffGroup,
    /// GetEmployeesForStaffGroupAssociation, GetEmployeesForGroupMembership,
    /// GetActiveDirectoryGroups) are deferred - see IGroupRepository's doc comment for why.
    /// </summary>
    public interface ISystemReviewRepository
    {
        void RemoveTitleGroupTitle(int titlegroupid, int titleid);
        void AddTitleGroupTitle(int titlegroupid, int titleid);
        string EditTitle(int selectedtitleid, string newtitle);
        string EditTitleStatus(int selectedtitleid, bool status);
        int AddTitle(string newTitleText);
        Task<List<TitleModel>> GetAllTitlesAsync(int titlegroupid);
        Task<List<TitleGroupModel>> GetTitleGroupsAsync();

        /// <summary>Ports GetAuditLog, minus the PagedList.Core paging (the source loads the whole
        /// stored-proc result set into memory and pages it with LINQ regardless, so returning the
        /// full filtered/sorted list here and letting MudTable's own pager slice it client-side is
        /// behavior-equivalent, just without a second paging abstraction on top).</summary>
        Task<List<AuditLogViewModel>> GetAuditLogAsync(string? sortOrder, string? searchString);
    }
}
