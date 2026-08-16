using Microsoft.AspNetCore.Components.Authorization;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;

namespace StaffManagement.Blazor.Services
{
    /// <summary>
    /// Scoped, one instance per Blazor circuit - this is the Blazor-layer replacement for the legacy
    /// session-backed StaffManagement.Models.AppUser. It implements ICurrentUserContext so Core
    /// services/repositories (AuditLoggerService, BaseRepository-derived repos) can read the current
    /// SystemId/impersonation state without depending on Blazor.
    ///
    /// The single biggest simplification versus AppUser: there is no session serialization at all
    /// (no GetAppUserFromSession/SetAppUserToSession, no JSON round-trip of UserLevels). A Blazor
    /// circuit is already scoped to one user for its lifetime, so the resolved identity/permission
    /// state just lives in private fields here for as long as the circuit is open - the entire
    /// session-hydration dance in the source was solving a problem (identity doesn't persist across
    /// requests) that doesn't exist in Blazor Server.
    ///
    /// Deliberately resolves identity/permissions through IUserRepository rather than holding a
    /// DbContext directly (unlike the reference DFSSlateAnalyzerBlazor.Services.UserService, which
    /// does inject its DbContext) - a small best-practice upgrade, not a behavior change.
    /// </summary>
    public class CurrentUserService : ICurrentUserContext
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IUserRepository _userRepository;

        private bool _loaded;
        private string _activeDirectoryName = string.Empty;
        private List<string> _userLevels = new();
        private string? _impersonateUser;

        public event Action? OnChange;

        public CurrentUserService(AuthenticationStateProvider authStateProvider, IUserRepository userRepository)
        {
            _authStateProvider = authStateProvider;
            _userRepository = userRepository;
        }

        public string ActiveDirectoryName => _activeDirectoryName;
        public int SystemId { get; private set; }
        public int ImpersonateSystemId { get; private set; }
        public int PrimaryUnitId { get; private set; }
        public int PermissionRoleId { get; private set; }
        public int NeedsReviewCount { get; private set; }

        /// <summary>Impersonation-aware AD username. Ports AppUser.CurrentUser.</summary>
        public string CurrentUser => _impersonateUser ?? _activeDirectoryName;

        /// <summary>Impersonation-aware SystemId. Ports AppUser.CurrentSystemId.</summary>
        public int CurrentSystemId => _impersonateUser != null ? ImpersonateSystemId : SystemId;

        public bool IsImpersonating => _impersonateUser != null;

        /// <summary>Idempotent - call from a page/layout's OnInitializedAsync before reading any other
        /// member. Resolves the Windows identity once per circuit and loads its permission levels.</summary>
        public async Task EnsureLoadedAsync()
        {
            if (_loaded) return;
            _loaded = true;

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var name = authState.User.Identity?.Name ?? string.Empty;

            // Strip the DOMAIN\ prefix, same as AppUser.ActiveDirectoryName's setter.
            _activeDirectoryName = name.Contains('\\') ? name[(name.IndexOf('\\') + 1)..] : name;

            SystemId = await _userRepository.GetSystemIdByActiveDirectoryNameAsync(_activeDirectoryName);
            _userLevels = await _userRepository.GetUserLevelsAsync(SystemId);

            var summary = await _userRepository.GetEmployeeSummaryAsync(SystemId);
            if (summary != null)
            {
                PrimaryUnitId = summary.Value.PrimaryUnitId;
                PermissionRoleId = summary.Value.PermissionRoleId;
            }

            NeedsReviewCount = await _userRepository.GetNeedsReviewCountAsync();
        }

        public bool IsInLevel(string permissionLevelName) => _userLevels.Contains(permissionLevelName);

        /// <summary>Starts impersonating another AD user: resolves their SystemId the same way the
        /// real signed-in user's is resolved (GetSystemIdByActiveDirectoryNameAsync, ports
        /// AppUser.GetEmployeeID), then re-resolves permission levels for them so IsInLevel reflects
        /// what the impersonated user - not the real signed-in admin - can do. That's the whole point
        /// of an impersonate-for-support feature. Ports AppUser.ImpersonateUser/ImpersonateSystemId +
        /// the session round-trip, minus the session round-trip (see class remarks).</summary>
        public async Task SetImpersonationAsync(string adUsername)
        {
            _impersonateUser = adUsername;
            ImpersonateSystemId = await _userRepository.GetSystemIdByActiveDirectoryNameAsync(adUsername);
            _userLevels = await _userRepository.GetUserLevelsAsync(ImpersonateSystemId);
            OnChange?.Invoke();
        }

        public async Task ClearImpersonationAsync()
        {
            _impersonateUser = null;
            ImpersonateSystemId = 0;
            _userLevels = await _userRepository.GetUserLevelsAsync(SystemId);
            OnChange?.Invoke();
        }
    }
}
