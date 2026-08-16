using StaffManagement.Data;

namespace StaffManagement.Core.Repositories.Interfaces
{
    /// <summary>
    /// New interface backing the plain CRUD portion of SystemReviewController's Group actions
    /// (Details/Create/Edit/Delete against the Group entity - AD security group records). The
    /// source hit accountmanagementContext directly from the controller for these; normalized here
    /// the same way ContractController's direct DbContext usage was in Phase 5.
    ///
    /// Deliberately NOT covering SystemReviewController.Index ("Group Admin") - that action builds a
    /// dashboard of AD-group-to-StaffGroup associations with live employee-matching counts, and
    /// depends on five IAccountRepository dropdown methods (GetUnitsAsync/GetBuildings/GetTitles/
    /// GetTitleGroups/GetActiveEmployees) that belong to Phase 8. Flagged as a deferred follow-up in
    /// Pages/Admin/GroupAdmin.razor rather than silently reduced in scope.
    /// </summary>
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllAsync();
        Task<Group?> GetByIdAsync(Guid id);
        Task CreateAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
