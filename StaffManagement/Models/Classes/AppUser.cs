using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StaffManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace StaffManagement.Models
{
    public class AppUser : BaseViewModel
    {
        private string _activeDirectoryName;
        private int _currentSystemId;

        private readonly accountmanagementContext _db = null;

        //  public accountmanagementContext _db => HttpContext.RequestServices.GetService<accountmanagementContext>();

        public string ActiveDirectoryName
        {

            get
            {
                if (_activeDirectoryName == null)
                {
                    return "";
                }
                else
                {
                    return _activeDirectoryName;
                }
            }
            set
            {
                if (value != null && value.Contains(@"\"))
                {
                    _activeDirectoryName = value.Substring(value.IndexOf(@"\") + 1);
                }
                else
                {
                    _activeDirectoryName = value;
                }
            }


        }



        public int SystemId { get; set; }



        public int PrimaryUnitId { get; set; }

        public string JobTitle { get; set; }

        public int JobCode { get; set; }

        public int NeedsReviewCount { get; set; }


        public int PermissionRoleId { get; set; }

        public string PermissionRole { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            { return FirstName + " " + LastName; }

        }

        public string Email { get; set; }

        public string ImpersonateUser { get; set; }
        public int ImpersonateSystemId { get; set; }

        public string AccountSearchFirstName { get; set; }
        public string AccountSearchLastName { get; set; }

        public string AccountSearchSystemId { get; set; }
        public int AccountSearchUnitId { get; set; }
        public int AccountSearchTitleId { get; set; }
        public int AccountSearchJobCode { get; set; }
        public string AccountSearchPersonnelCode { get; set; }

        public int AccountSearchBuildingId { get; set; }


        public string FromAction { get; set; }


        public string FromController { get; set; }



        public List<string> UserLevels { get; set; }

        private IHttpContextAccessor _httpContextAccessor;

        public IHttpContextAccessor HttpContextAccessor
        {
            set
            {
                this._httpContextAccessor = value;
            }
            get
            {
                return _httpContextAccessor;
            }

        }

        private string _currentUser;
        public string CurrentUser

        {

            get
            {
                if (this.ImpersonateUser == null)
                { _currentUser = this.ActiveDirectoryName; }
                else
                { _currentUser = this.ImpersonateUser; }

                return _currentUser;
            }

            set
            { _currentUser = value; }
        }

        public int CurrentSystemId

        {

            get
            {
                if (this.ImpersonateUser == null)
                { _currentSystemId = this.SystemId; }
                else
                { _currentSystemId = this.ImpersonateSystemId; }

                return _currentSystemId;
            }

        }


        public int SelectedUnitId { get; set; }

        public int SelectedTitleId { get; set; }



        public int EditAccountSystemId { get; set; }
        public int EditAccountTitleGroupId { get; set; }
        public int EditAccountTitleId { get; set; }

        public Nullable<int> EditAccountAccountTypeId { get; set; }
        public Nullable<int> EditAccountEmployeeId { get; set; }
        public string EditAccountADObjectGuid { get; set; }

        public string EditAccountFirstName { get; set; }
        public string EditAccountMiddleName { get; set; }
        public string EditAccountLastName { get; set; }
        public string EditAccountSuffixName { get; set; }
        public string EditAccountAlternateFirstName { get; set; }
        public string EditAccountAlternateLastName { get; set; }
        public string EditAccountNickname { get; set; }
        public Nullable<int> EditAccountPrimaryUnitId { get; set; }

        public string EditAccountUsernameOverride { get; set; }

        public string EditAccountIsActive { get; set; }

        public string EditAccountSource { get; set; }

        public int EditAccountJobRecordId { get; set; }

        public string EditAccountIsPHRST { get; set; }
        public string EditAccountJobRecordIsActive { get; set; }

        public string EditAccountIsPrimary { get; set; }

        public string EditAccountStartDate { get; set; }
        public string EditAccountEndDate { get; set; }

        public Nullable<int> EditAccountUnitId { get; set; }

        public Nullable<int> EditAccountBuildingId { get; set; }

        public Nullable<int> EditAccountSubstituteForId { get; set; }

        public string EditAccountHrWorkLocation { get; set; }
        public string EditAccountHrJobCode { get; set; }
        public string EditAccountHrPersonnelCode { get; set; }
        public string EditAccountHrPrimary { get; set; }

        public virtual SelectList Users { get; set; }

        public string SelectedUser { get; set; }


        public AppUser(IHttpContextAccessor httpContextAccessor)

        {



            var context = (accountmanagementContext)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(accountmanagementContext));
            _db = context;

            GetAppUserData(httpContextAccessor.HttpContext);

        }

        public AppUser()

        {



            var context = (accountmanagementContext)(this.HttpContextAccessor.HttpContext).RequestServices.GetService(typeof(accountmanagementContext));
            _db = context;


            GetAppUserData(this.HttpContextAccessor.HttpContext);

        }


        public AppUser(HttpContext httpContext)

        {



            var context = (accountmanagementContext)httpContext.RequestServices.GetService(typeof(accountmanagementContext));
            _db = context;

            GetAppUserData(httpContext);

        }



        public void GetAppUserData(HttpContext httpContext)
        {



            GetAppUserFromSession(httpContext);

            if (ActiveDirectoryName == "")
            {
                var user = httpContext.User.FindFirst(ClaimTypes.Name).Value;
                ActiveDirectoryName = user;




                if (this.ImpersonateUser != null)
                {

                    ImpersonateSystemId = GetEmployeeID(CurrentUser);
                    UserLevels = GetUserLevels(ImpersonateSystemId);

                    GetEmployee(ImpersonateSystemId);
                    NeedsReviewCount = GetNeedsReviewCount(ImpersonateSystemId);
                }
                else
                {
                    SystemId = GetEmployeeID(CurrentUser);
                    GetEmployee(SystemId);

                    if (UserLevels?.Any() != true)
                    {

                        UserLevels = GetUserLevels(SystemId);

                    }

                    NeedsReviewCount = GetNeedsReviewCount(SystemId);
                }


                SetAppUserToSession(httpContext);


            }



        }





        public bool IsInLevel(string role)
        {
            if (UserLevels?.Any() == true)
            {

                if (UserLevels.Contains(role))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;


            }


        }

        public List<string> GetUserLevels(int systemId)
        {


            var userLevels = new List<string>(_db.Permission
                        .Include(permission => permission.PermissionRole)
                        .ThenInclude(permissionrole => permissionrole.PermissionLevelRole)
                        .ThenInclude(permissionlevelrole => permissionlevelrole.PermissionLevel)
                        .Where(permission => permission.Active == true)
                        .Where(permission => permission.PermissionRole.Active == true)
                        .Where(permission => permission.PermissionRole.PermissionLevelRole.Active == true)
                        .Where(permission => permission.PermissionRole.PermissionLevelRole.PermissionLevel.Active == true)

                        .Include(user => user.Account.Employee)

                        .Where(permission => permission.SystemId == systemId)
                        .Select(levels => levels.PermissionRole.PermissionLevelRole.PermissionLevel.PermissionLevelName.ToString())).ToList();




            return userLevels;

        }

        public int GetEmployeeID(string currentUser)
        {
            int systemId;

            var query = _db.Users
                   .Include(user => user.Account)

                    .Where(user => user.SamaccountName == currentUser)
                    .SingleOrDefault();

            if (query?.Account != null)
            {
                systemId = query.Account.SystemId;
            }
            else
            {
                systemId = 0;
            }
            return systemId;
        }

        public int GetNeedsReviewCount(int systemId)
        {


            var query = _db.JobRecord

                    .Where(jobrecord => jobrecord.IsPHRST == true)
                    .Where(jobrecord => jobrecord.NeedsReview == true)
                    .Count();


            return query;
        }



        public bool GetEmployee(int systemId)
        {


            var query = _db.Users
                   .Include(user => user.Account)
                   .ThenInclude(account => account.Permission)

                   .Include(user => user.Account)
                   .ThenInclude(account => account.AccountUnit)
                   .ThenInclude(accountunit => accountunit.Unit)




                   // .ThenInclude(permission => permission.PermissionRole)
                   .Where(user => user.Account.SystemId == systemId)
                   .Where(user => user.Account.Permission.SystemId == systemId)
                   .Where(user => user.Account.Permission.Active == true)
                    // .Where(user => user.Account.Permission.PermissionRole.Active == true)







                    .FirstOrDefault();

            if (query?.Account.AccountUnit != null)
            {

                PrimaryUnitId = query.Account.AccountUnit.UnitId;
                PermissionRoleId = query.Account.Permission.PermissionRoleID;
                // PermissionRole = query.Account.Permission.PermissionRole.ToString();
                // JobCode = query.Account.Employee.Jobcode;
                //  JobTitle = query.Account.Employee.JobTitle;

            }
            else
            {

            }
            return true;
        }



        public bool GetAppUserFromSession(HttpContext context)
        {


            List<string> userLevels = new List<string>();



            ActiveDirectoryName = context.Session.GetString("ActiveDirectoryName");

            if (context.Session.GetInt32("SystemId") != null)
            { SystemId = (int)context.Session.GetInt32("SystemId"); }

            ImpersonateUser = context.Session.GetString("ImpersonateUser");
            SelectedUser = context.Session.GetString("SelectedUser");

            if (context.Session.GetInt32("ImpersonateSystemId") != null)
            {
                ImpersonateSystemId = (int)context.Session.GetInt32("ImpersonateSystemId");
            }

            if (context.Session.GetString("JobTitle") != null)
            {
                JobTitle = context.Session.GetString("JobTitle");
            }
            if (context.Session.GetString("JobCode") != null)
            {
                JobCode = int.Parse(context.Session.GetString("JobCode"));
            }
            if (context.Session.GetString("PrimaryUnitId") != null)
            {
                PrimaryUnitId = (int)context.Session.GetInt32("PrimaryUnitId");
            }
            if (context.Session.GetString("SelectedUnitId") != null)
            {
                SelectedUnitId = (int)context.Session.GetInt32("SelectedUnitId");
            }

            if (context.Session.GetInt32("PermissionRoleID") != null)
            {
                PermissionRoleId = (int)context.Session.GetInt32("PermissionRoleID");
            }

            if (context.Session.GetString("AccountSearchFirstName") != null)
            {
                AccountSearchFirstName = context.Session.GetString("AccountSearchFirstName");
            }

            if (context.Session.GetString("AccountSearchLastName") != null)
            {
                AccountSearchLastName = context.Session.GetString("AccountSearchLastName");
            }

            if (context.Session.GetString("AccountSearchSystemId") != null)
            {
                AccountSearchSystemId = context.Session.GetString("AccountSearchSystemId");
            }

            if (context.Session.GetInt32("AccountSearchUnitId") != null)
            {
                AccountSearchUnitId = (int)context.Session.GetInt32("AccountSearchUnitId");
            }

            if (context.Session.GetInt32("AccountSearchTitleId") != null)
            {
                AccountSearchTitleId = (int)context.Session.GetInt32("AccountSearchTitleId");
            }

            if (context.Session.GetInt32("AccountSearchJobCode") != null)
            {
                AccountSearchJobCode = (int)context.Session.GetInt32("AccountSearchJobCode");
            }

            if (context.Session.GetString("AccountSearchPersonnelCode") != null)
            {
                AccountSearchPersonnelCode = context.Session.GetString("AccountSearchPersonnelCode");
            }



            if (context.Session.GetInt32("AccountSearchBuildingId") != null)
            {
                AccountSearchBuildingId = (int)context.Session.GetInt32("AccountSearchBuildingId");
            }





            if (context.Session.GetString("FromController") != null)
            {
                FromController = context.Session.GetString("FromController");
            }

            if (context.Session.GetString("FromAction") != null)
            {
                FromAction = context.Session.GetString("FromAction");
            }

            var value = context.Session.GetString("UserLevels");

            if (string.IsNullOrEmpty(value))

            {
                return false;
            }
            else
            {
                userLevels = JsonConvert.DeserializeObject<List<string>>(value);

                UserLevels = userLevels;

                return true;
            }



        }

        public bool SetAppUserToSession(HttpContext context)
        {


            List<string> userLevels = new List<string>();


            context.Session.SetString("ActiveDirectoryName", ActiveDirectoryName);
            context.Session.SetInt32("SystemID", SystemId);

            if (ImpersonateUser != null && ImpersonateUser != "")
            {
                context.Session.SetString("ImpersonateUser", ImpersonateUser);
                context.Session.SetString("SelectedUser", SelectedUser);
            }

            if (ImpersonateSystemId != 0)
            {
                context.Session.SetInt32("ImpersonateSystemId", ImpersonateSystemId);
            }


            context.Session.SetString("CurrentUser", CurrentUser);

            if (!(string.IsNullOrEmpty(JobTitle)))
            {
                context.Session.SetString("JobTitle", JobTitle);
            }
            if (!(JobCode == 0))
            {
                context.Session.SetString("JobCode", JobCode.ToString());
            }
            if (!(PrimaryUnitId == 0))
            {
                context.Session.SetInt32("PrimaryUnitId", PrimaryUnitId);
            }

            context.Session.SetInt32("SelectedUnitId", SelectedUnitId);


            if (!(PermissionRoleId == 0))
            {
                context.Session.SetInt32("PermissionRoleID", PermissionRoleId);
            }

            if (UserLevels?.Any() == true)
            {
                var serializedUserLevels = JsonConvert.SerializeObject(UserLevels);
                context.Session.SetString("UserLevels", serializedUserLevels);
            }

            return true;

        }

        public void SaveAccountSearch(HttpContext context, string firstName, string lastName, string systemId, int unitId, int titleId, int jobCode, string personnelCode, int buildingId)
        {

            context.Session.SetString("AccountSearchFirstName", firstName == null ? "" : firstName);
            context.Session.SetString("AccountSearchLastName", lastName == null ? "" : lastName);
            context.Session.SetString("AccountSearchSystemId", systemId == null ? "" : systemId);
            context.Session.SetInt32("AccountSearchUnitId", unitId);
            context.Session.SetInt32("AccountSearchTitleId", titleId);
            context.Session.SetInt32("AccountSearchJobCode", jobCode);
            context.Session.SetString("AccountSearchPersonnelCode", personnelCode);
            context.Session.SetInt32("AccountSearchBuildingId", buildingId);


        }

        public void GetCurrentAccount(HttpContext context)
        {
            EditAccountSystemId = (int)context.Session.GetInt32("EditAccountSystemId");
            EditAccountAccountTypeId = context.Session.GetInt32("EditAccountAccountTypeId") != null ? context.Session.GetInt32("EditAccountAccountTypeId") : null;
            EditAccountEmployeeId = context.Session.GetInt32("EditAccountEmployeeId") != null ? context.Session.GetInt32("EditAccountEmployeeId") : null;
            EditAccountADObjectGuid = context.Session.GetString("EditAccountADObjectGuid") != "" ? context.Session.GetString("EditAccountADObjectGuid") : "";
            EditAccountFirstName = context.Session.GetString("EditAccountFirstName") != "" ? context.Session.GetString("EditAccountFirstName") : null;
            EditAccountMiddleName = context.Session.GetString("EditAccountMiddleName") != "" ? context.Session.GetString("EditAccountMiddleName") : null;
            EditAccountLastName = context.Session.GetString("EditAccountLastName") != "" ? context.Session.GetString("EditAccountLastName") : null;
            EditAccountSuffixName = context.Session.GetString("EditAccountSuffixName") != "" ? context.Session.GetString("EditAccountSuffixName") : null;
            EditAccountAlternateFirstName = context.Session.GetString("EditAccountAlternateFirstName") != "" ? context.Session.GetString("EditAccountAlternateFirstName") : null;
            EditAccountAlternateLastName = context.Session.GetString("EditAccountAlternateLastName") != "" ? context.Session.GetString("EditAccountAlternateLastName") : null;
            EditAccountNickname = context.Session.GetString("EditAccountNickname") != "" ? context.Session.GetString("EditAccountNickname") : null;
            EditAccountUsernameOverride = context.Session.GetString("EditAccountUsernameOverride") != "" ? context.Session.GetString("EditAccountUsernameOverride") : null;
            EditAccountPrimaryUnitId = context.Session.GetInt32("EditAccountPrimaryUnitId") != null ? context.Session.GetInt32("EditAccountPrimaryUnitId") : null;
            EditAccountIsActive = context.Session.GetString("EditAccountIsActive") != "" ? context.Session.GetString("EditAccountIsActive") : "";
            EditAccountSource = context.Session.GetString("EditAccountSource") != "" ? context.Session.GetString("EditAccountSource") : null;





        }


        public void SaveCurrentAccount(HttpContext context, AccountModel model)
        {


            context.Session.SetInt32("EditAccountSystemId", model.SystemId);
            context.Session.SetInt32("EditAccountAccountTypeId", model.AccountTypeId != null ? (int)model.AccountTypeId : 0);
            context.Session.SetInt32("EditAccountEmployeeId", model.EmployeeId != null ? (int)model.EmployeeId : 0);
            context.Session.SetString("EditAccountADObjectGuid", model.ADObjectGuid != null ? model.ADObjectGuid.ToString() : string.Empty);
            context.Session.SetString("EditAccountFirstName", model.FirstName != null ? model.FirstName.ToString() : string.Empty);
            context.Session.SetString("EditAccountMiddleName", model.MiddleName != null ? model.MiddleName.ToString() : string.Empty);
            context.Session.SetString("EditAccountLastName", model.LastName != null ? model.LastName.ToString() : string.Empty);
            context.Session.SetString("EditAccountSuffixName", model.SuffixName != null ? model.SuffixName.ToString() : string.Empty);
            context.Session.SetString("EditAccountAlternateFirstName", model.AlternateFirstName != null ? model.AlternateFirstName.ToString() : string.Empty);
            context.Session.SetString("EditAccountAlternateLastName", model.AlternateLastName != null ? model.AlternateLastName.ToString() : string.Empty);
            context.Session.SetString("EditAccountNickname", model.Nickname != null ? model.Nickname.ToString() : string.Empty);
            context.Session.SetInt32("EditAccountPrimaryUnitId", model.PrimaryUnitId != null ? (int)model.PrimaryUnitId : 0);
            context.Session.SetString("EditAccountUsernameOverride", model.UsernameOverride != null ? model.UsernameOverride.ToString() : string.Empty);
            context.Session.SetString("EditAccountIsActive", model.IsActive != null ? model.IsActive.ToString() : string.Empty);
            context.Session.SetString("EditAccountSource", model.Source != null ? model.Source.ToString() : string.Empty);


        }


        public void GetCurrentJobRecord(HttpContext context)
        {
            EditAccountJobRecordId = context.Session.GetInt32("EditAccountJobRecordId") != null ? (int)context.Session.GetInt32("EditAccountJobRecordId") : 0;
            EditAccountIsPHRST = context.Session.GetString("EditAccountIsPHRST") != "" ? context.Session.GetString("EditAccountIsPHRST") : "";
            EditAccountJobRecordIsActive = context.Session.GetString("EditAccountJobRecordIsActive") != "" ? context.Session.GetString("EditAccountJobRecordIsActive") : "";
            EditAccountIsPrimary = context.Session.GetString("EditAccountIsPrimary") != "" ? context.Session.GetString("EditAccountIsPrimary") : "";

            EditAccountStartDate = context.Session.GetString("EditAccountStartDate") != "" ? context.Session.GetString("EditAccountStartDate") : null;
            EditAccountEndDate = context.Session.GetString("EditAccountEndDate") != "" ? context.Session.GetString("EditAccountEndDate") : null;

            EditAccountUnitId = context.Session.GetInt32("EditAccountUnitId") != null ? context.Session.GetInt32("EditAccountUnitId") : null;
            EditAccountBuildingId = context.Session.GetInt32("EditAccountBuildingId") != null ? context.Session.GetInt32("EditAccountBuildingId") : null;
            EditAccountSubstituteForId = context.Session.GetInt32("EditAccountSubstituteForId") != null ? context.Session.GetInt32("EditAccountSubstituteForId") : null;
            EditAccountTitleGroupId = context.Session.GetInt32("EditAccountTitleGroupId") != null ? (int)context.Session.GetInt32("EditAccountTitleGroupId") : 0;
            EditAccountTitleId = context.Session.GetInt32("EditAccountTitleId") != null ? (int)context.Session.GetInt32("EditAccountTitleId") : 0;
            EditAccountHrWorkLocation = context.Session.GetString("EditAccountHrWorkLocation") != "" ? context.Session.GetString("EditAccountHrWorkLocation") : null;
            EditAccountHrJobCode = context.Session.GetString("EditAccountHrJobCode") != "" ? context.Session.GetString("EditAccountHrJobCode") : null;
            EditAccountHrPersonnelCode = context.Session.GetString("EditAccountHrPersonnelCode") != "" ? context.Session.GetString("EditAccountHrPersonnelCode") : null;
            EditAccountHrPrimary = context.Session.GetString("EditAccountHrPrimary") != "" ? context.Session.GetString("EditAccountHrPrimary") : "";





        }



        public void SaveCurrentJobRecord(HttpContext context, AccountModel model)
        {


            context.Session.SetInt32("EditAccountJobRecordId", model.JobRecord.JobRecordId);
            context.Session.SetString("EditAccountIsPHRST", model.JobRecord.IsPHRST != null ? model.JobRecord.IsPHRST.ToString() : string.Empty);
            context.Session.SetString("EditAccountJobRecordIsActive", model.JobRecord.IsActive.ToString());
            context.Session.SetString("EditAccountIsPrimary", model.JobRecord.IsPrimary.ToString());
            context.Session.SetString("EditAccountStartDate", model.JobRecord.StartDate != null ? model.JobRecord.StartDate.ToString() : string.Empty);
            context.Session.SetString("EditAccountEndDate", model.JobRecord.EndDate != null ? model.JobRecord.EndDate.ToString() : string.Empty);
            context.Session.SetInt32("EditAccountUnitId", model.JobRecord.UnitId != null ? (int)model.JobRecord.UnitId : 0);
            context.Session.SetInt32("EditAccountBuildingId", model.JobRecord.BuildingId != null ? (int)model.JobRecord.BuildingId : 0);
            context.Session.SetInt32("EditAccountSubstituteForId", model.JobRecord.SubstituteForId != null ? (int)model.JobRecord.SubstituteForId : 0);
            context.Session.SetInt32("EditAccountTitleGroupId", model.JobRecord.TitleGroupId);
            context.Session.SetInt32("EditAccountTitleId", model.JobRecord.TitleId);
            context.Session.SetString("EditAccountHrWorkLocation", model.JobRecord.HrWorkLocation != null ? model.JobRecord.HrWorkLocation.ToString() : string.Empty);
            context.Session.SetString("EditAccountHrJobCode", model.JobRecord.HrJobCode != null ? model.JobRecord.HrJobCode.ToString() : string.Empty);
            context.Session.SetString("EditAccountHrPersonnelCode", model.JobRecord.HrPersonnelCode != null ? model.JobRecord.HrPersonnelCode.ToString() : string.Empty);
            context.Session.SetString("EditAccountHrPrimary", model.JobRecord.HrPrimary.ToString());

        }






        public void SaveNavigationPath(HttpContext context, string fromController, string fromAction)
        {


            context.Session.SetString("FromController", fromController == null ? "" : fromController);
            context.Session.SetString("FromAction", fromAction == null ? "" : fromAction);


        }




        public void SaveEncryptedValue(HttpContext context, string encryptedValue, string decryptedValue)
        {

            context.Session.SetString(encryptedValue, decryptedValue);

        }

        public string GetSavedEncryptedValue(HttpContext context, string encryptedValue)
        {

            var decryptedValue = context.Session.GetString(encryptedValue).ToString();

            return decryptedValue;
        }


    }
}
