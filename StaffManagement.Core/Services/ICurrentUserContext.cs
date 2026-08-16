namespace StaffManagement.Core.Services
{
    /// <summary>
    /// The small read-only slice of "who is doing this" that Core-layer services/repositories need
    /// (audit-log stamping, LastUpdateUser fields, permission checks). This exists so Core doesn't
    /// depend on the Blazor project's circuit-scoped CurrentUserService (or, previously, the old MVC
    /// project's session-backed AppUser) - StaffManagement.Blazor.Services.CurrentUserService
    /// implements this interface and is registered for it in Program.cs.
    /// </summary>
    public interface ICurrentUserContext
    {
        /// <summary>Account.SystemId of the actually-signed-in Windows identity (not impersonation-aware).</summary>
        int SystemId { get; }

        /// <summary>Account.SystemId currently being impersonated, or 0 if not impersonating.</summary>
        int ImpersonateSystemId { get; }

        /// <summary>Impersonation-aware AD username: the impersonated user's name if impersonating, else the real signed-in user's name.</summary>
        string CurrentUser { get; }

        /// <summary>Impersonation-aware SystemId: ImpersonateSystemId if impersonating, else SystemId.</summary>
        int CurrentSystemId { get; }

        bool IsInLevel(string permissionLevelName);
    }
}
