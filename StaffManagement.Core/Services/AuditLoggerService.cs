using Microsoft.EntityFrameworkCore;
using StaffManagement.Data;

namespace StaffManagement.Core.Services
{
    public enum ActionTypes : int
    {
        Create = 1,
        Edit = 2,
        Delete = 3,
        ReActivate = 4,
        Search = 5
    }

    /// <summary>Ported from StaffManagement/Services/AuditLoggerService.cs, unchanged behavior:
    /// writes one AuditLog row per call, stamped with the current (and, if applicable, impersonated)
    /// SystemId. AppUser -&gt; ICurrentUserContext is the only substantive change (see that interface's
    /// doc comment for why). Takes IDbContextFactory rather than accountmanagementContext directly -
    /// see the comment on AddStaffManagementCoreClasses' AddDbContextFactory call for why.</summary>
    public class AuditLoggerService : IAuditLoggerService
    {
        private readonly IDbContextFactory<accountmanagementContext> _dbFactory;
        private readonly ICurrentUserContext _currentUser;

        public AuditLoggerService(IDbContextFactory<accountmanagementContext> dbFactory, ICurrentUserContext currentUser)
        {
            _dbFactory = dbFactory;
            _currentUser = currentUser;
        }

        public bool Log(int actiontypeid, string referencetable, string referenceid, string comment)
        {
            using var db = _dbFactory.CreateDbContext();

            var logentry = new AuditLog()
            {
                LogDate = DateTime.Now,
                ReferenceTable = string.IsNullOrEmpty(referencetable) ? null : referencetable,
                ReferenceId = string.IsNullOrEmpty(referenceid) ? null : referenceid,
                ActionTypeId = actiontypeid,
                AppUser_SystemId = _currentUser.SystemId,
                AppUser_ImpersonatedUserId = _currentUser.ImpersonateSystemId,
                Comment = string.IsNullOrEmpty(comment) ? null : comment
            };

            db.AuditLogs.Add(logentry);
            db.SaveChanges();

            return true;
        }
    }
}
