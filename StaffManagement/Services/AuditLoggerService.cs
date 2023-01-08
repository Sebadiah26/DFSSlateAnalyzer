using Microsoft.AspNetCore.Http;
using StaffManagement.Data;
using StaffManagement.Models;
using System;

namespace StaffManagement.Services
{
    public enum ActionTypes : int
    {
        Create = 1,
        Edit = 2,
        Delete = 3,
        ReActivate = 4,
        Search = 5
    }

    public class AuditLoggerService : IAuditLoggerService

    {


        private readonly accountmanagementContext _db = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AppUser _appUser = null;

        public AuditLoggerService(accountmanagementContext context, IHttpContextAccessor httpContextAccessor, AppUser appUser)
        {
            _db = context;
            _httpContextAccessor = httpContextAccessor;
            _appUser = appUser;
            //_appuser = new AppUser(_httpContextAccessor);

            //_appUser = appuser;
            //if (_httpContextAccessor.HttpContext != null)
            //{

            //    _appUser = appuser;
            //    _appUser.GetAppUserData(_httpContextAccessor.HttpContext);
            //}


        }

        //string.IsNullOrEmpty(someString) ? "0" : someString;

        public bool Log(int actiontypeid, string referencetable, string referenceid, string comment)
        {
            var logentry = new AuditLog()
            {
                LogDate = DateTime.Now,
                ReferenceTable = string.IsNullOrEmpty(referencetable) ? null : referencetable,
                ReferenceId = string.IsNullOrEmpty(referenceid) ? null : referenceid,
                ActionTypeId = actiontypeid,
                AppUser_SystemId = _appUser.SystemId,
                AppUser_ImpersonatedUserId = _appUser.ImpersonateSystemId,
                Comment = string.IsNullOrEmpty(comment) ? null : comment
            };

            _db.AuditLogs.Add(logentry);
            _db.SaveChanges();

            return true;
        }
    }
}
