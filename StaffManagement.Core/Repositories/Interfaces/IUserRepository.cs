using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffManagement.Core.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>Impersonation-target dropdown data (AD users with an active Permission), used by
        /// the ported ImpersonateWidget. Ports IUserRepository.GetAppUsers.</summary>
        Task<SelectList> GetAppUsersAsync();

        /// <summary>Resolves the Account.SystemId for an AD SAM account name (post-domain-prefix-strip),
        /// or 0 if none found. Ports AppUser.GetEmployeeID.</summary>
        Task<int> GetSystemIdByActiveDirectoryNameAsync(string samAccountName);

        /// <summary>Resolves an ad.User by email (Mail or UserPrincipalName) or SamaccountName - the
        /// identifier a SAML IdP (ClassLink) asserts about a user isn't guaranteed to be their bare
        /// username, so this accepts either. Excludes deleted/disabled accounts. Returns the matched
        /// user's bare SamaccountName (no domain prefix) so the caller can feed it straight into
        /// GetSystemIdByActiveDirectoryNameAsync/CurrentUserService's existing resolution path, or null
        /// if nothing matches.</summary>
        Task<string?> ResolveSamAccountNameAsync(string emailOrUsername);

        /// <summary>Flat list of permission-level names active for the given SystemId, walking
        /// Permission -&gt; PermissionRole -&gt; PermissionLevelRole -&gt; PermissionLevel (all four Active
        /// flags ANDed). Ports AppUser.GetUserLevels verbatim - this is the sole source of truth
        /// CurrentUserService.IsInLevel checks against.</summary>
        Task<List<string>> GetUserLevelsAsync(int systemId);

        /// <summary>PrimaryUnitId + PermissionRoleId for the given SystemId's active Account/Permission,
        /// or null if none found. Ports the meaningful output of AppUser.GetEmployee (its JobCode/JobTitle
        /// assignments were already commented out in the source).</summary>
        Task<(int PrimaryUnitId, int PermissionRoleId)?> GetEmployeeSummaryAsync(int systemId);

        /// <summary>Global count of PHRST job records flagged NeedsReview. NOTE: the legacy
        /// AppUser.GetNeedsReviewCount(int systemId) took a systemId parameter but never actually used
        /// it to filter the query - this is a straight, parameterless port of what the code actually
        /// does, not a behavior change.</summary>
        Task<int> GetNeedsReviewCountAsync();
    }
}
