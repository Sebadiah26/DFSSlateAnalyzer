using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {


        public AccountRepository(accountmanagementContext db,
                                IAuditLoggerService log,
                                IHttpContextAccessor contextAccessor,
                                IMemoryCache memoryCache,
                                AppUser appUser)

                             : base(db, log, contextAccessor, memoryCache, appUser)
        {


        }

        public PagedList.Core.IPagedList<AccountModel> GetAccounts(string firstName, string lastName, string systemId, int unitId, int titleId, int jobCode, string personnelCode, int buildingId, string sortOrder, int pageNumber, int pageSize)

        {


            var query = from a in _db.Accounts

                        join jr in _db.JobRecord_Ordered.Where(e => e.Order == 1)
                             on a.SystemId equals jr.SystemId

                             into ajr
                        from jr in ajr.DefaultIfEmpty() // technique for outer join 

                        join a2 in _db.Accounts
                            on jr.SubstituteForId equals a2.SystemId into jra2
                        from a2 in jra2.DefaultIfEmpty()

                        join at in _db.AccountTypes
                           on a.AccountTypeId equals at.AccountTypeId into aat
                        from at in aat.DefaultIfEmpty()

                        join t in _db.Titles
                            on jr.TitleId equals t.TitleId into jrt
                        from t in jrt.DefaultIfEmpty()

                        join tg in _db.TitleGroups
                            on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                        from tg in jrtg.DefaultIfEmpty()

                        join u in _db.Units
                            on jr.UnitId equals u.UnitId into jru
                        from u in jru.DefaultIfEmpty()

                        join bg in _db.Buildings
                            on jr.BuildingId equals bg.BuildingId into jrbg
                        from bg in jrbg.DefaultIfEmpty()


                        join e in _db.Employees
                            on a.EmployeeId equals e.EmployeeId into ae
                        from e in ae.DefaultIfEmpty()

                        join us in _db.Users
                           on a.AdobjectGuid equals us.ObjectGuid into aus
                        from us in aus.DefaultIfEmpty()

                        join ou in _db.OrganizationalUnits
                         on us.OrganizationalUnitGuid equals ou.ObjectGuid into usou
                        from ou in usou.DefaultIfEmpty()



                        select new
                        {
                            SystemId = a.SystemId,
                            EmployeeId = a.EmployeeId == null ? 0 : a.EmployeeId ?? 0,
                            FirstName = (a.FirstName == null || a.FirstName == "") ? (e.FirstName ?? (us.GivenName ?? "")) : a.FirstName ?? "",
                            LastName = (a.LastName == null || a.LastName == "") ? (e.LastName ?? (us.Surname ?? "")) : a.LastName ?? "",
                            EmployeeType = at.Description,
                            JobTitle = (a.JobTitle == null || a.JobTitle == "") ? (e.JobTitle ?? (us.JobTitle ?? "")) : a.JobTitle ?? "",
                            Title = t.Text ?? "",
                            TitleGroup = tg.Description ?? "",
                            Department = u.Name ?? "",
                            //Department = (u.Name == null || u.Name == "") ? u2.Name : u.Name ,
                            Building = bg.Name ?? "",
                            AccountLastUpdate = a.LastUpdate,
                            AccountLastUpdateUser = a.LastUpdateUser ?? "",
                            EmployeeLastUpdate = (DateTime?)e.LastUpdate,
                            EmployeeLastUpdateUser = "PHRST",
                            JobRecordLastUpdate = jr.LastUpdate,
                            JobRecordLastUpdateUser = jr.LastUpdateUser ?? "",
                            IsDeleted = e.IsDeleted == null ? false : e.IsDeleted,
                            IsActive = a.IsActive == null ? true : a.IsActive,
                            AccountExpires = us.AccountExpires,
                            AlternateFirstName = a.AlternateFirstName ?? "",
                            AlternateLastName = a.AlternateLastName ?? "",
                            UsernameOverride = a.UsernameOverride ?? "",
                            //PrimaryUnitId = a.PrimaryUnitId == null ? 0 : a.PrimaryUnitId,
                            PrimaryUnitId = jr.UnitId ?? 0,
                            TitleId = jr.TitleId ?? 0,
                            Jobcode = e.Jobcode == null ? 0 : e.Jobcode,
                            PersonnelCode = e.PersonnelCode == null ? "" : e.PersonnelCode,
                            BuildingId = jr.BuildingId == null ? 0 : jr.BuildingId ?? 0,
                            Source = a.Source ?? "",
                            SubstituteFor = (a2.FirstName + " " + a2.LastName) ?? "",
                            UserIsDisabled = us.IsDisabled == null ? false : us.IsDisabled,
                            UserIsDeleted = us.IsDeleted == null ? false : us.IsDeleted,
                            OrganizationalUnit = ou.OrganizationalUnitName
                        };


            // If ID field is searched, ignore other search parameters.

            if (systemId != "")

            {

                int number;

                if (int.TryParse(systemId, out number))

                { // look for systemid or employeeid match
                    query = query.Where(x => x.SystemId.Equals(number)
                                        || x.EmployeeId.Equals(number));
                }


            }

            else

            {   // check other parameters one by one add to search if entered/selected.


                if (firstName != "")
                {

                    query = query.Where(x => x.FirstName.Contains(firstName)
                              || x.AlternateFirstName.Contains(firstName)
                              || x.UsernameOverride.Contains(firstName)
                              || _db.Soundex(x.FirstName) == _db.Soundex(firstName));

                }

                if (lastName != "")
                {
                    query = query.Where(x => x.LastName.Contains(lastName)
                            || x.AlternateLastName.Contains(lastName)
                            || x.UsernameOverride.Contains(lastName)
                            || _db.Soundex(x.LastName) == _db.Soundex(lastName)); ;
                }



                if (unitId != 0)
                {
                    query = query.Where(x => x.PrimaryUnitId.Equals(unitId));
                }

                if (titleId != 0)
                {
                    query = query.Where(x => x.TitleId.Equals(titleId));
                }

                if (jobCode != 0)
                {
                    query = query.Where(e => e.Jobcode.Equals(jobCode));
                }

                if (personnelCode != "")
                {
                    query = query.Where(x => x.PersonnelCode.Equals(personnelCode));
                }

                if (buildingId != 0)
                {
                    query = query.Where(x => x.BuildingId.Equals(buildingId));
                }

            }


            query = query.Where(x => x.Source != "STU" && x.Source != null);

            query = query.Where(x =>
                        x.OrganizationalUnit == null ||
                        x.OrganizationalUnit == "_Disabled User Accounts" ||
                        x.OrganizationalUnit == "Student Support" ||
                        x.OrganizationalUnit == "Users");


            query = sortOrder switch
            {
                " Jobtitle_Asc_Sort" => query.OrderBy(s => s.JobTitle),
                " Jobtitle_Desc_Sort" => query.OrderByDescending(s => s.JobTitle),
                " TitleGroup_Asc_Sort" => query.OrderBy(s => s.TitleGroup).ThenBy(s => s.Title),
                " TitleGroup_Desc_Sort" => query.OrderByDescending(s => s.TitleGroup).ThenByDescending(s => s.Title),
                " Title_Asc_Sort" => query.OrderBy(s => s.Title),
                " Title_Desc_Sort" => query.OrderByDescending(s => s.Title),
                " AccountType_Asc_Sort" => query.OrderBy(s => s.EmployeeType),
                " AccountType_Desc_Sort" => query.OrderByDescending(s => s.EmployeeType),
                " FirstName_Asc_Sort" => query.OrderBy(s => s.FirstName),
                " FirstName_Desc_Sort" => query.OrderByDescending(s => s.FirstName),
                " LastName_Asc_Sort" => query.OrderBy(s => s.LastName),
                " LastName_Desc_Sort" => query.OrderByDescending(s => s.LastName),

                _ => query.OrderBy(s => s.SystemId),
            };
            var accountModelList = new List<AccountModel>();
            var totalCount = query.Count();

            IPagedList<dynamic> accountDataList = query.ToPagedList(pageNumber, pageSize);


            if (accountDataList?.Any() == true)
            {
                foreach (var account in accountDataList)
                {
                    AccountModel accountModel = new AccountModel();


                    accountModel.SystemId = account.SystemId;
                    accountModel.EmployeeId = account.EmployeeId;
                    accountModel.FirstName = account.FirstName;
                    accountModel.LastName = account.LastName;
                    accountModel.AccountType.Description = account.EmployeeType;
                    accountModel.JobTitle = account.JobTitle;
                    accountModel.JobRecord.Title.Text = account.Title;
                    accountModel.JobRecord.TitleGroup.Description = account.TitleGroup;
                    accountModel.JobRecord.SubstituteFor = account.SubstituteFor;
                    accountModel.PrimaryUnit.Name = account.Department;
                    accountModel.PrimaryUnit.Building.Name = account.Building;
                    accountModel.AccountLastUpdate = account.AccountLastUpdate;
                    accountModel.AccountLastUpdateUser = account.AccountLastUpdateUser;
                    accountModel.JobRecordLastUpdate = account.JobRecordLastUpdate;
                    accountModel.JobRecordLastUpdateUser = account.JobRecordLastUpdateUser;
                    accountModel.Employee.IsDeleted = account.IsDeleted;
                    accountModel.IsActive = account.IsActive;
                    accountModel.User.IsDisabled = account.UserIsDisabled;
                    accountModel.User.IsDeleted = account.UserIsDeleted;
                    accountModel.User.AccountExpires = account.AccountExpires;


                    accountModelList.Add(accountModel);
                    //}

                }

            }


            IPagedList<AccountModel> pagedAccountModelList = new StaticPagedList<AccountModel>(accountModelList, pageNumber, pageSize, totalCount);






            return pagedAccountModelList;


        }




        public async Task<List<AccountModel>> GetAccounts(string searchFirstName, string searchLastName)


        {

            var query = from a in _db.Accounts

                        join jr in _db.JobRecord_Ordered.Where(e => e.Order == 1)
                         on a.SystemId equals jr.SystemId

                         into ajr
                        from jr in ajr.DefaultIfEmpty() // technique for outer join 

                        join t in _db.Titles
                            on jr.TitleId equals t.TitleId into jrt
                        from t in jrt.DefaultIfEmpty()

                        join tg in _db.TitleGroups
                            on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                        from tg in jrtg.DefaultIfEmpty()

                        join u in _db.Units
                            on jr.UnitId equals u.UnitId into jru
                        from u in jru.DefaultIfEmpty()

                        join bg in _db.Buildings
                            on jr.BuildingId equals bg.BuildingId into jrbg
                        from bg in jrbg.DefaultIfEmpty()

                        join e in _db.Employees
                            on a.EmployeeId equals e.EmployeeId into ae
                        from e in ae.DefaultIfEmpty()

                        join us in _db.Users
                          on a.AdobjectGuid equals us.ObjectGuid into au
                        from us in au.DefaultIfEmpty()
                        select new
                        {
                            SystemId = (a.SystemId == null) ? 0 : a.SystemId,
                            EmployeeId = a.EmployeeId,
                            FirstName = (a.FirstName == null || a.FirstName == "") ? (e.FirstName ?? (us.GivenName ?? "")) : a.FirstName ?? "",
                            LastName = (a.LastName == null || a.LastName == "") ? (e.LastName ?? (us.Surname ?? "")) : a.LastName ?? "",
                            JobTitle = a.JobTitle ?? "",
                            Title = t.Text ?? "",
                            TitleGroup = tg.Description ?? "",
                            Department = u.Name ?? "",
                            Building = bg.Name ?? "",
                            LastUpdate = a.LastUpdate,
                            IsDeleted = (e.IsDeleted == null) ? false : e.IsDeleted,
                            IsActive = a.IsActive,
                            AlternateFirstName = a.AlternateFirstName ?? "",
                            AlternateLastName = a.AlternateLastName ?? "",
                            UsernameOverride = a.UsernameOverride ?? "",
                            PrimaryUnitId = jr.UnitId,
                            TitleId = jr.TitleId,
                            Jobcode = (e.Jobcode == null) ? 0 : e.Jobcode,
                            PersonnelCode = e.PersonnelCode ?? "",
                            BuildingId = (u.BuildingId == null) ? 0 : u.BuildingId,
                            Source = a.Source ?? "",
                            IsDisabled = (us.IsDisabled == null) ? false : us.IsDisabled,
                            UserIsDeleted = (us.IsDeleted == null) ? false : us.IsDeleted,
                            Description = us.Description ?? "",
                            AccountExpires = us.AccountExpires,
                            DistinguishedName = us.DistinguishedName ?? "",
                            WhenCreated = (us.WhenCreated == null) ? DateTime.Now : us.WhenCreated,
                            WhenChanged = (us.WhenChanged == null) ? DateTime.Now : us.WhenChanged,
                        };


            if (searchFirstName != null)
            {
                query = query.Where(x => x.FirstName.Contains(searchFirstName)
                       || x.AlternateFirstName.Contains(searchFirstName)
                       || x.UsernameOverride.Contains(searchFirstName)
                       || _db.Soundex(x.FirstName) == _db.Soundex(searchFirstName)

                );
            }


            if (searchLastName != null)
            {
                query = query.Where(x => x.LastName.Contains(searchLastName)
                            || x.AlternateLastName.Contains(searchLastName)
                            || x.UsernameOverride.Contains(searchLastName)
                            || _db.Soundex(x.LastName) == _db.Soundex(searchLastName)

                );
            }




            query = query.Where(x => x.Source != "STU" && x.Source != null);


            var accountModelList = new List<AccountModel>();
            var accountDataList = await query.OrderBy(x => x.LastName).Take(50).ToListAsync<dynamic>();


            accountDataList.Select(d => new
            {
                SystemId = d.SystemId,
                EmployeeId = d.EmployeeId.RemoveNull(),
                FirstName = d.FirstName.RemoveNull(),
                LastName = d.LastName.RemoveNull(),
                JobTitle = d.JobTitle.RemoveNull(),
                Title = d.Title.RemoveNull(),
                TitleGroup = d.TitleGroup.RemoveNull(),
                Department = d.Department.RemoveNull(),
                Building = d.Building.RemoveNull(),
                LastUpdate = d.LastUpdate,
                IsDeleted = d.IsDeleted.RemoveNull(false),
                IsActive = d.IsActive.RemoveNull(true),
                AlternateFirstName = d.AlternateFirstName.RemoveNull(),
                AlternateLastName = d.AlternateLastName.RemoveNull(),
                UsernameOverride = d.UsernameOverride.RemoveNull(),
                PrimaryUnitId = d.PrimaryUnitId.RemoveNull(),
                TitleId = d.TitleId.RemoveNull(),
                Jobcode = d.Jobcode.RemoveNull(),
                PersonnelCode = d.PersonnelCode.RemoveNull(),
                BuildingId = d.BuildingId.RemoveNull(),
                Source = d.Source.RemoveNull(),
                IsDisabled = d.IsDisabled.RemoveNull(false),
                UserIsDeleted = d.IsDeleted.RemoveNull(false),
                Description = d.Description.RemoveNull(),
                AccountExpires = d.AccountExpires.RemoveNull(),
                DistinguishedName = d.DistinguishedName.RemoveNull(),
                WhenCreated = d.WhenCreated.RemoveNull(),
                WhenChanged = d.WhenChanged.RemoveNull(),
            });


            if (accountDataList?.Any() == true)
            {
                foreach (var account in accountDataList)
                {

                    AccountModel accountModel = new AccountModel();

                    accountModel.SystemId = account.SystemId;
                    accountModel.FirstName = account.FirstName;
                    accountModel.LastName = account.LastName;
                    accountModel.EmployeeId = account.EmployeeId;
                    accountModel.JobTitle = account.JobTitle;
                    accountModel.JobRecord.Title.Text = account.Title;
                    accountModel.JobRecord.TitleGroup.Description = account.TitleGroup;
                    accountModel.PrimaryUnit.Name = account.Department;
                    accountModel.PrimaryUnit.Building.Name = account.Building;
                    accountModel.LastUpdate = account.LastUpdate;
                    accountModel.Employee.IsDeleted = account.IsDeleted;
                    accountModel.IsActive = account.IsActive;
                    accountModel.Employee.Jobcode = account.Jobcode;
                    accountModel.Employee.PersonnelCode = account.PersonnelCode;
                    accountModel.JobRecord.Unit.Building.Name = account.Building;
                    accountModel.JobRecord.Unit.Name = account.Department;
                    accountModel.Source = account.Source;
                    accountModel.User.IsDisabled = account.IsDisabled;
                    accountModel.User.IsDeleted = account.UserIsDeleted;
                    accountModel.User.Description = account.Description;
                    accountModel.User.AccountExpires = account.AccountExpires;
                    accountModel.User.DistinguishedName = account.DistinguishedName;
                    accountModel.User.WhenCreated = account.WhenCreated;
                    accountModel.User.WhenChanged = account.WhenChanged;


                    accountModelList.Add(accountModel);

                }

            }


            return accountModelList;


        }


        public async Task<SelectList> GetEmployees()
        {

            var userdata = new SelectList(await _db.Accounts.Where(a => a.Source != "STU").Where(a => a.LastName != null).OrderBy(a => a.LastName)
                .ToDictionaryAsync(a => a.SystemId, a => a.Name), "Key", "Value");


            return userdata;
        }


        public async Task<SelectList> GetActiveEmployees()
        {


            var userdata = new SelectList(await _db.Accounts.Where(a => a.EmployeeId != null).Where(a => a.Source != "STU").Where(a => a.LastName != null).Where(a => a.Employee.IsDeleted == false).OrderBy(a => a.JobTitle)
                .ToDictionaryAsync(a => a.SystemId, a => a.JobTitle + " " + a.PrimaryUnitId.ToString() + " " + a.Name), "Key", "Value");


            return userdata;
        }



        public async Task<List<EmployeeModel>> SearchEmployeesByName(string searchFirstName, string searchLastName)

        {

            var employeeModelList = new List<EmployeeModel>();


            if (searchFirstName != "" && searchLastName != "")
            {

                var query = from e in _db.Employees

                            join u in _db.Units
                                on e.UnitId equals u.UnitId

                            join a in _db.Accounts
                                on e.EmployeeId equals a.EmployeeId into grouping
                            from a in grouping.DefaultIfEmpty()



                            select new
                            {
                                FirstName = e.FirstName ?? "",
                                LastName = e.LastName ?? "",
                                JobTitle = e.JobTitle ?? "",
                                IsDeleted = (e.IsDeleted == null) ? false : e.IsDeleted,
                                Department = u.Name ?? "",
                                SystemId = (a.SystemId == null) ? 0 : a.SystemId,
                                EmployeeId = (e.EmployeeId == null) ? 0 : e.EmployeeId,
                                EndDate = e.EndDate,
                                LastUpdate = (e.LastUpdate == null) ? DateTime.Now : e.LastUpdate


                            };


                if (searchFirstName != null)
                {
                    query = query.Where(e => e.FirstName.Contains(searchFirstName)
                     || _db.Soundex(e.FirstName) == _db.Soundex(searchFirstName));
                }




                if (searchLastName != null)
                {
                    query = query.Where(e => e.LastName.Contains(searchLastName)
                    || _db.Soundex(e.LastName) == _db.Soundex(searchLastName));
                }




                var employeeDataList = await query.OrderBy(x => x.LastName).Take(50).ToListAsync<dynamic>();

                employeeDataList.Select(d => new
                {
                    FirstName = d.FirstName.RemoveNull(),
                    LastName = d.LastName.RemoveNull(),
                    JobTitle = d.JobTitle.RemoveNull(),

                    IsDeleted = d.IsDeleted.RemoveNull(false),
                    Department = d.Name.RemoveNull(),
                    SystemId = d.SystemId.RemoveNull(),
                    EmployeeId = d.EmployeeId.RemoveNull(),
                    EndDate = d.EndDate,
                    LastUpdate = d.LastUpdate


                });



                if (employeeDataList?.Any() == true)
                {
                    foreach (var employee in employeeDataList)
                    {

                        var unit = new Unit();
                        unit.Name = employee.Department;

                        //  var needsReview = new NeedsReview();
                        //  needsReview.needsReview = employee.NeedsReview;

                        var account = new Account();
                        account.SystemId = employee.SystemId;

                        employeeModelList.Add(new EmployeeModel()
                        {
                            EmployeeId = employee.EmployeeId,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            JobTitle = employee.JobTitle,
                            //UnitId = employee.UnitId,
                            IsDeleted = employee.IsDeleted,
                            EndDate = employee.EndDate,
                            LastUpdate = employee.LastUpdate,

                            Unit = unit,
                            //  NeedsReview = needsReview,
                            Account = account


                        });



                    }

                }

            }



            return employeeModelList;


        }

        public void EditAccount(Account account, string comment, bool ChangeNameOnly)
        {


            if (account.SuffixName == null)
            { account.SuffixName = ""; }

            if (account.MiddleName == null)
            { account.MiddleName = ""; }



            var accountToEdit = _db.Accounts.Find(account.SystemId);


            accountToEdit.FirstName = account.FirstName;
            accountToEdit.MiddleName = account.MiddleName;
            accountToEdit.LastName = account.LastName;
            accountToEdit.SuffixName = account.SuffixName;

            if (ChangeNameOnly == false)
            {
                accountToEdit.EmployeeId = account.EmployeeId;
                accountToEdit.AccountTypeId = account.AccountTypeId;
                accountToEdit.AdobjectGuid = account.AdobjectGuid;
                accountToEdit.AlternateFirstName = account.AlternateFirstName;
                accountToEdit.AlternateLastName = account.AlternateLastName;
                accountToEdit.Nickname = account.Nickname;
                accountToEdit.JobTitle = account.JobTitle;
                accountToEdit.PrimaryUnitId = account.PrimaryUnitId;
                accountToEdit.UsernameOverride = account.UsernameOverride;
                // accountToEdit.IsActive = account.IsActive;
                accountToEdit.LastUpdateUser = account.LastUpdateUser;
                accountToEdit.LastUpdate = account.LastUpdate;
            }


            _db.Update(accountToEdit);

            _db.SaveChanges();


            _log.Log((int)ActionTypes.Edit, "dbo.Account", account.SystemId.ToString(), comment);


        }


        public void EditAccountEmployee(Account account)
        {


            var accountToEdit = _db.Accounts.Find(account.SystemId);

            accountToEdit.EmployeeId = account.EmployeeId;
            accountToEdit.LastUpdateUser = _appUser.CurrentUser;
            accountToEdit.LastUpdate = DateTime.Now;


            _db.Update(accountToEdit);

            _db.SaveChanges();



        }



        public void ActivateEmployee(int employeeid)
        {


            var employeeToEdit = _db.Employees.Find(employeeid);

            employeeToEdit.LastUpdate = DateTime.Now;
            employeeToEdit.IsDeleted = false;


            _db.Update(employeeToEdit);

            _db.SaveChanges();



        }
        /// <summary>
        /// Get Account/Employee/JobRecord info for an employee and load into object model. Used by "Views/Account/Edit.cshtml"
        /// </summary>
        /// <param name="id">SystemId</param>
        /// <param name="jobrecordId">Current job record being viewed</param>
        /// <returns>AccountModel</returns>

        // HTTP GET for  Edit page 
        public async Task<AccountModel> GetAccountByID(int id, int jobRecordId)
        {
            var model = new AccountModel();
            var employeeModel = new EmployeeModel();
            var jobRecordModel = new JobRecordModel();
            var title = new Title();
            var titleGroup = new TitleGroup();
            var buildingModel = new BuildingModel();
            var unitModel = new UnitModel();
            // var jobTitleAssignment = new JobTitleAssignment();



            var data = new Account();

            IQueryable<Account> query = _db.Accounts

                .Include(x => x.Employee)  //.ThenInclude(x => x.EmployeeLatestJob)
                .Include(x => x.PrimaryUnit)
                .Include(u => u.User)
                .ThenInclude(u => u.OrganizationalUnit)
              ;


            query = query.Where(x => x.SystemId == id);

            data = await query.FirstOrDefaultAsync();


            System.DateTime? lastUpdate;

            if (data.PrimaryUnitId != null)
            {
                model.PrimaryUnitId = data.PrimaryUnitId;
            }



            if (data.LastUpdate.HasValue)
            { lastUpdate = (DateTime)data.LastUpdate; }
            else
            {
                lastUpdate = null;
            }

            model.SystemId = data.SystemId;
            model.AccountTypeId = data.AccountTypeId;
            model.EmployeeId = data.EmployeeId;
            model.ContractorId = data.ContractorId;
            model.StudentId = data.StudentId;
            model.ADObjectGuid = data.AdobjectGuid;
            model.FirstName = data.FirstName;
            model.MiddleName = data.MiddleName;
            model.LastName = data.LastName;
            model.SuffixName = data.SuffixName;
            model.AlternateFirstName = data.AlternateFirstName;
            model.AlternateLastName = data.AlternateLastName;
            model.Nickname = data.Nickname;
            model.JobTitle = data.JobTitle;
            model.Description = data.Description;
            model.UsernameOverride = data.UsernameOverride;
            model.IsActive = data.IsActive;
            model.Source = data.Source;
            model.LastUpdate = lastUpdate;
            model.LastUpdateUser = data.LastUpdateUser;

            model.PrimaryUnitId = data.PrimaryUnitId;

            model.CreateDate = data.CreateDate;
            model.CreateUser = data.CreateUser;

            //Field for keeping track of name edits - default to Account values
            model.EditFirstName = data.FirstName;
            model.EditMiddleName = data.MiddleName;
            model.EditLastName = data.LastName;
            model.EditSuffixName = data.SuffixName;

            if (data.AdobjectGuid != null)
            {
                model.User.GivenName = data.User.GivenName;
                model.User.Surname = data.User.Surname;
                model.User.SamaccountName = data.User.SamaccountName;
                model.User.Mail = data.User.Mail;
                model.User.JobTitle = data.User.JobTitle;
                model.User.IsDisabled = data.User.IsDisabled;
                model.User.IsDeleted = data.User.IsDeleted;
                model.User.OrganizationalUnit.OrganizationalUnitName = data.User.OrganizationalUnit.OrganizationalUnitName;
                model.User.AccountExpires = data.User.AccountExpires;

                if (model.EditFirstName == null)
                { model.EditFirstName = model.User.GivenName; }
                if (model.EditLastName == null)
                { model.EditLastName = model.User.Surname; }
            }



            else
            {
                model.User.SamaccountName = model.DerivedSAMAccountName;
                model.User.Mail = model.DerivedEmail;
            }




            if (data.Employee != null)
            {
                //Field for keeping track of name edits - override with PHRST Values
                model.EditFirstName = data.Employee.FirstName;
                model.EditMiddleName = data.Employee.MiddleName;
                model.EditLastName = data.Employee.LastName;
                model.EditSuffixName = data.Employee.SuffixName;

                employeeModel.EmployeeId = data.Employee.EmployeeId;
                employeeModel.FirstName = data.Employee.FirstName;
                employeeModel.MiddleName = data.Employee.MiddleName;
                employeeModel.LastName = data.Employee.LastName;
                employeeModel.SuffixName = data.Employee.SuffixName;
                employeeModel.FsfBuildingId = data.Employee.FsfBuildingId;
                employeeModel.PersonnelCode = data.Employee.PersonnelCode;
                employeeModel.Jobcode = data.Employee.Jobcode;
                employeeModel.UnitId = data.Employee.UnitId;
                employeeModel.UnitIdManual = data.Employee.UnitIdManual;
                employeeModel.JobTitle = data.Employee.JobTitle;
                employeeModel.JobTitleManual = data.Employee.JobTitleManual;
                employeeModel.EndDate = data.Employee.EndDate;
                employeeModel.IsDeleted = data.Employee.IsDeleted;
                employeeModel.LastUpdate = data.Employee.LastUpdate;
                employeeModel.IsSubstitute = false;


                model.Employee = employeeModel;
            }


            int primaryIndex = 0;
            int i = 0;

            //Get job records




            //IQueryable<JobRecord> jobrecordquery = _db.JobRecord;
            var jobRecordQuery = from jr in _db.JobRecord

                                 join t in _db.Titles
                                on jr.TitleId equals t.TitleId into jrt
                                 from t in jrt.DefaultIfEmpty()

                                 join tg in _db.TitleGroups
                                 on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                                 from tg in jrtg.DefaultIfEmpty()

                                 join u in _db.Units
                                 on jr.UnitId equals u.UnitId into jru
                                 from u in jru.DefaultIfEmpty()

                                 join pc in _db.PersonnelCodes
                                 on jr.HrPersonnelCode equals pc.Personnel_Code into jrpc
                                 from pc in jrpc.DefaultIfEmpty()

                                 join jc in _db.Jobcodes
                                 on jr.HrJobCode equals jc.JobcodeId into jrjc
                                 from jc in jrjc.DefaultIfEmpty()

                                 join b in _db.Buildings
                                 on jr.HrWorkLocation equals b.FsfBuildingId into jrb
                                 from b in jrb.DefaultIfEmpty()

                                 select new
                                 {
                                     IsPHRST = jr.IsPHRST,
                                     IsActive = jr.IsActive,
                                     StartDate = jr.StartDate,
                                     EndDate = jr.EndDate,
                                     JobRecordId = jr.JobRecordId,
                                     UnitId = jr.UnitId ?? 0,
                                     BuildingId = jr.BuildingId ?? 0,
                                     SubstituteForId = jr.SubstituteForId ?? 0,
                                     TitleId = jr.TitleId ?? 0,
                                     TitleGroupId = jr.TitleGroupId ?? 0,
                                     SystemId = jr.SystemId,
                                     IsPrimary = jr.IsPrimary,
                                     HrWorkLocation = jr.HrWorkLocation ?? "",
                                     HrRecordNumber = jr.HrRecordNumber,
                                     HrPrimary = jr.HrPrimary,
                                     HrPersonnelCode = jr.HrPersonnelCode ?? "",
                                     HrMatched = jr.HrMatched,
                                     HrJobCode = jr.HrJobCode ?? 0,
                                     NeedsReview = jr.NeedsReview,
                                     CreateDate = jr.CreateDate,
                                     CreateUser = jr.CreateUser,
                                     LastUpdate = jr.LastUpdate,
                                     LastUpdateUser = jr.LastUpdateUser,
                                     TitleText = t.Text ?? "",
                                     TitleGroupDescription = tg.Description ?? "",
                                     BuildingName = b.Name ?? "",
                                     UnitName = u.Name ?? ""

                                 };




            jobRecordQuery = jobRecordQuery.Where(x => x.SystemId == id);

            var jobRecordData = jobRecordQuery.ToList();

            // flag primary or selected record so it can then be set as the individual jobrecord object
            if (jobRecordData.Any() == true)
            {
                foreach (var jobRecord in jobRecordData)
                {
                    if ((jobRecordId == 0 && jobRecord.IsPrimary == true) || (jobRecord.JobRecordId == jobRecordId))
                    {
                        primaryIndex = i;

                        break;
                    }

                    i++;
                }

                // check if specific active jobrecord selected
                if (jobRecordId != -1)
                {
                    jobRecordModel.IsPHRST = jobRecordData.ElementAt(primaryIndex).IsPHRST;
                    jobRecordModel.IsActive = jobRecordData.ElementAt(primaryIndex).IsActive;
                    jobRecordModel.StartDate = jobRecordData.ElementAt(primaryIndex).StartDate;
                    jobRecordModel.EndDate = jobRecordData.ElementAt(primaryIndex).EndDate;
                    jobRecordModel.JobRecordId = jobRecordData.ElementAt(primaryIndex).JobRecordId;
                    jobRecordModel.UnitId = jobRecordData.ElementAt(primaryIndex).UnitId;
                    jobRecordModel.BuildingId = jobRecordData.ElementAt(primaryIndex).BuildingId;
                    jobRecordModel.SubstituteForId = jobRecordData.ElementAt(primaryIndex).SubstituteForId;
                    jobRecordModel.TitleId = jobRecordData.ElementAt(primaryIndex).TitleId != null ? (int)jobRecordData.ElementAt(primaryIndex).TitleId : 0;
                    jobRecordModel.TitleGroupId = jobRecordData.ElementAt(primaryIndex).TitleGroupId != null ? (int)jobRecordData.ElementAt(primaryIndex).TitleGroupId : 0;
                    jobRecordModel.SystemId = (int)jobRecordData.ElementAt(primaryIndex).SystemId;
                    jobRecordModel.IsPrimary = jobRecordData.ElementAt(primaryIndex).IsPrimary;
                    jobRecordModel.HrWorkLocation = jobRecordData.ElementAt(primaryIndex).HrWorkLocation ?? "";
                    jobRecordModel.HrRecordNumber = jobRecordData.ElementAt(primaryIndex).HrRecordNumber;
                    jobRecordModel.HrPrimary = jobRecordData.ElementAt(primaryIndex).HrPrimary;
                    jobRecordModel.HrPersonnelCode = jobRecordData.ElementAt(primaryIndex).HrPersonnelCode ?? "";
                    jobRecordModel.HrMatched = jobRecordData.ElementAt(primaryIndex).HrMatched;
                    jobRecordModel.HrJobCode = jobRecordData.ElementAt(primaryIndex).HrJobCode;
                    jobRecordModel.NeedsReview = jobRecordData.ElementAt(primaryIndex).NeedsReview;
                    jobRecordModel.CreateDate = jobRecordData.ElementAt(primaryIndex).CreateDate;
                    jobRecordModel.CreateUser = jobRecordData.ElementAt(primaryIndex).CreateUser;
                    jobRecordModel.LastUpdate = jobRecordData.ElementAt(primaryIndex).LastUpdate;
                    jobRecordModel.LastUpdateUser = jobRecordData.ElementAt(primaryIndex).LastUpdateUser;

                    employeeModel.IsSubstitute = (jobRecordData.ElementAt(primaryIndex).SubstituteForId) > 0;
                    employeeModel.SelectedEmployeeId = jobRecordData.ElementAt(primaryIndex).SubstituteForId;


                    model.JobRecord = jobRecordModel;


                    if (jobRecordData.ElementAt(primaryIndex).TitleText != null)
                    {
                        title.TitleId = jobRecordData.ElementAt(primaryIndex).TitleId;
                        title.Text = jobRecordData.ElementAt(primaryIndex).TitleText;
                        model.JobRecord.Title = title;
                    }

                    if (jobRecordData.ElementAt(primaryIndex).TitleGroupId != null)
                    {
                        titleGroup.TitleGroupId = jobRecordData.ElementAt(primaryIndex).TitleGroupId;
                        titleGroup.Description = jobRecordData.ElementAt(primaryIndex).TitleGroupDescription;
                        model.JobRecord.TitleGroup = titleGroup;
                    }

                    if (jobRecordData.ElementAt(primaryIndex).BuildingName != null)
                    {
                        buildingModel.Name = jobRecordData.ElementAt(primaryIndex).BuildingName;
                        model.JobRecord.Building = buildingModel;
                    }

                    if (jobRecordData.ElementAt(primaryIndex).UnitName != null)
                    {
                        unitModel.Name = jobRecordData.ElementAt(primaryIndex).UnitName;
                        model.JobRecord.Unit = unitModel;
                    }

                }
                else
                {
                    jobRecordModel.JobRecordId = -1;
                    jobRecordModel.IsPrimary = true;
                    model.JobRecord = jobRecordModel;
                }


                // loop through JobRecords and add all to model


                foreach (var jobRecord in jobRecordData)
                {
                    var jobRecordModelItem = new JobRecordModel();

                    jobRecordModelItem.Title.Text = jobRecord.TitleText ?? "";
                    jobRecordModelItem.Unit.Name = jobRecord.UnitName ?? "";

                    jobRecordModelItem.IsActive = jobRecord.IsActive;
                    jobRecordModelItem.StartDate = jobRecord.StartDate;
                    jobRecordModelItem.EndDate = jobRecord.EndDate;
                    jobRecordModelItem.JobRecordId = jobRecord.JobRecordId;
                    jobRecordModelItem.UnitId = jobRecord.UnitId;
                    jobRecordModelItem.BuildingId = jobRecord.BuildingId;
                    jobRecordModelItem.SubstituteForId = jobRecord.SubstituteForId;
                    jobRecordModelItem.TitleId = jobRecord.TitleId != null ? (int)jobRecord.TitleId : 0;
                    jobRecordModelItem.TitleGroupId = jobRecord.TitleGroupId != null ? (int)jobRecord.TitleGroupId : 0;
                    jobRecordModelItem.SystemId = (int)jobRecord.SystemId;
                    jobRecordModelItem.IsPrimary = jobRecord.IsPrimary;
                    jobRecordModelItem.HrWorkLocation = jobRecord.HrWorkLocation ?? "";
                    jobRecordModelItem.HrRecordNumber = jobRecord.HrRecordNumber;
                    jobRecordModelItem.HrPrimary = jobRecord.HrPrimary;
                    jobRecordModelItem.HrPersonnelCode = jobRecord.HrPersonnelCode ?? "";
                    jobRecordModelItem.HrMatched = jobRecord.HrMatched;
                    jobRecordModelItem.HrJobCode = jobRecord.HrJobCode;
                    jobRecordModelItem.NeedsReview = jobRecord.NeedsReview;
                    jobRecordModelItem.CreateDate = jobRecord.CreateDate;
                    jobRecordModelItem.CreateUser = jobRecord.CreateUser;
                    jobRecordModelItem.LastUpdate = jobRecord.LastUpdate;
                    jobRecordModelItem.LastUpdateUser = jobRecord.LastUpdateUser;

                    model.JobRecords.Add(jobRecordModelItem);
                }
            }
            else
            {
                jobRecordModel.JobRecordId = -1;
                jobRecordModel.IsPrimary = true;
                model.JobRecord = jobRecordModel;
            }






            // loop through EmployeeJob Records and add all to model

            if (model.EmployeeId != null)
            {

                model.Employee.EmployeeJobs = await GetEmployeeJobs((int)model.EmployeeId);

                //var employeejobdata = new List<EmployeeJob>();

                // IQueryable<EmployeeJob> employeejobquery = _db.EmployeeJobs;

                // employeejobquery = employeejobquery.Include(x => x.Building).Where(x => x.FsfBuildingId != null);
                // employeejobquery = employeejobquery.Include(x => x.PersonnelCodes).Where(x => x.PersonnelCode != null);
                // employeejobquery = employeejobquery.Include(x => x.Unit).Where(x => x.UnitId != null);
                // employeejobquery = employeejobquery.Include(x => x.Jobcodes).Where(x => x.Jobcode != null);
                // employeejobquery = employeejobquery.Where(x => x.EmployeeId == model.EmployeeId);

                // employeejobdata = employeejobquery.ToList();



                // foreach (var employeejob in employeejobdata)
                // {
                //     var employeejobmodelitem = new EmployeeJobModel();

                //     employeejobmodelitem.EmployeeId = employeejob.EmployeeId;
                //     employeejobmodelitem.FsfBuildingId = employeejob.FsfBuildingId;
                //     employeejobmodelitem.PersonnelCode = employeejob.PersonnelCode;
                //     employeejobmodelitem.Jobcode = employeejob.Jobcode;
                //     employeejobmodelitem.UnitId = employeejob.UnitId;
                //     employeejobmodelitem.IsPrimary = employeejob.IsPrimary;
                //     employeejobmodelitem.Buildings.Description = employeejob.Building.Description;
                //     employeejobmodelitem.PersonnelCodes.Description = employeejob.PersonnelCodes == null ? "" : employeejob.PersonnelCodes.Description;
                //     employeejobmodelitem.Jobcodes.Description = employeejob.Jobcodes == null ? "" : employeejob.Jobcodes.Description;

                //     model.Employee.EmployeeJobs.Add(employeejobmodelitem);



                // }

            }

            return model;
        }




        public async Task<List<JobRecordModel>> GetJobRecordsByAccount(int systemId)
        {

            List<JobRecordModel> jobRecords = new List<JobRecordModel>();

            var jobRecordQuery = from jr in _db.JobRecord

                                 join t in _db.Titles
                                on jr.TitleId equals t.TitleId into jrt
                                 from t in jrt.DefaultIfEmpty()

                                 join tg in _db.TitleGroups
                                 on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                                 from tg in jrtg.DefaultIfEmpty()

                                 join u in _db.Units
                                 on jr.UnitId equals u.UnitId into jru
                                 from u in jru.DefaultIfEmpty()

                                 join pc in _db.PersonnelCodes
                                 on jr.HrPersonnelCode equals pc.Personnel_Code into jrpc
                                 from pc in jrpc.DefaultIfEmpty()

                                 join jc in _db.Jobcodes
                                 on jr.HrJobCode equals jc.JobcodeId into jrjc
                                 from jc in jrjc.DefaultIfEmpty()

                                 join b in _db.Buildings
                                 on jr.HrWorkLocation equals b.FsfBuildingId into jrb
                                 from b in jrb.DefaultIfEmpty()

                                 select new
                                 {
                                     IsPHRST = jr.IsPHRST,
                                     IsActive = jr.IsActive,
                                     StartDate = jr.StartDate,
                                     EndDate = jr.EndDate,
                                     JobRecordId = jr.JobRecordId,
                                     UnitId = jr.UnitId ?? 0,
                                     BuildingId = jr.BuildingId ?? 0,
                                     SubstituteForId = jr.SubstituteForId ?? 0,
                                     TitleId = jr.TitleId ?? 0,
                                     TitleGroupId = jr.TitleGroupId ?? 0,
                                     SystemId = jr.SystemId,
                                     IsPrimary = jr.IsPrimary,
                                     HrWorkLocation = jr.HrWorkLocation ?? "",
                                     HrRecordNumber = jr.HrRecordNumber,
                                     HrPrimary = jr.HrPrimary,
                                     HrPersonnelCode = jr.HrPersonnelCode ?? "",
                                     HrMatched = jr.HrMatched,
                                     HrJobCode = jr.HrJobCode ?? 0,
                                     NeedsReview = jr.NeedsReview,
                                     CreateDate = jr.CreateDate,
                                     CreateUser = jr.CreateUser,
                                     LastUpdate = jr.LastUpdate,
                                     LastUpdateUser = jr.LastUpdateUser,
                                     TitleText = t.Text ?? "",
                                     TitleGroupDescription = tg.Description ?? "",
                                     BuildingName = b.Name ?? "",
                                     UnitName = u.Name ?? ""

                                 };


            jobRecordQuery = jobRecordQuery.Where(x => x.SystemId == systemId);

            var jobRecordData = await jobRecordQuery.ToListAsync();


            // loop through JobRecords and add all to model


            foreach (var jobRecord in jobRecordData)
            {
                var jobRecordModelItem = new JobRecordModel();

                jobRecordModelItem.Title.Text = jobRecord.TitleText ?? "";
                jobRecordModelItem.Unit.Name = jobRecord.UnitName ?? "";

                jobRecordModelItem.IsActive = jobRecord.IsActive;
                jobRecordModelItem.StartDate = jobRecord.StartDate;
                jobRecordModelItem.EndDate = jobRecord.EndDate;
                jobRecordModelItem.JobRecordId = jobRecord.JobRecordId;
                jobRecordModelItem.UnitId = jobRecord.UnitId;
                jobRecordModelItem.BuildingId = jobRecord.BuildingId;
                jobRecordModelItem.SubstituteForId = jobRecord.SubstituteForId;
                jobRecordModelItem.TitleId = jobRecord.TitleId != null ? (int)jobRecord.TitleId : 0;
                jobRecordModelItem.TitleGroupId = jobRecord.TitleGroupId != null ? (int)jobRecord.TitleGroupId : 0;
                jobRecordModelItem.SystemId = (int)jobRecord.SystemId;
                jobRecordModelItem.IsPrimary = jobRecord.IsPrimary;
                jobRecordModelItem.HrWorkLocation = jobRecord.HrWorkLocation ?? "";
                jobRecordModelItem.HrRecordNumber = jobRecord.HrRecordNumber;
                jobRecordModelItem.HrPrimary = jobRecord.HrPrimary;
                jobRecordModelItem.HrPersonnelCode = jobRecord.HrPersonnelCode ?? "";
                jobRecordModelItem.HrMatched = jobRecord.HrMatched;
                jobRecordModelItem.HrJobCode = jobRecord.HrJobCode;
                jobRecordModelItem.NeedsReview = jobRecord.NeedsReview;
                jobRecordModelItem.CreateDate = jobRecord.CreateDate;
                jobRecordModelItem.CreateUser = jobRecord.CreateUser;
                jobRecordModelItem.LastUpdate = jobRecord.LastUpdate;
                jobRecordModelItem.LastUpdateUser = jobRecord.LastUpdateUser;

                jobRecords.Add(jobRecordModelItem);
            }


            return jobRecords;

            // var model = new AccountModel();
            // var employeemodel = new EmployeeModel();
            // var jobrecordmodel = new JobRecordModel();
            // var title = new Title();
            // var titlegroup = new TitleGroup();

            // // var sis_staff = new Sis_Staff();

            //var data = new Account();

            // IQueryable<Account> query = _db.Accounts
            //     .Include(x => x.Employee)


            //     .Include(x => x.PrimaryUnit);


            // query = query.Include(x => x.JobRecords).ThenInclude(x => x.Title).Include(x => x.JobRecords).ThenInclude(x => x.Unit);


            // query = query.Where(x => x.SystemId == systemid);
            // data = await query.FirstOrDefaultAsync();



            // System.DateTime? lastupdate;




            // if (data.LastUpdate.HasValue)
            // { lastupdate = (DateTime)data.LastUpdate; }
            // else
            // {
            //     lastupdate = null;
            // }

            // model.SystemId = data.SystemId;
            // model.EmployeeId = data.EmployeeId;
            // model.ContractorId = data.ContractorId;
            // model.StudentId = data.StudentId;
            // model.ADObjectGuid = data.AdobjectGuid;
            // model.FirstName = data.FirstName;
            // model.MiddleName = data.MiddleName;
            // model.LastName = data.LastName;
            // model.SuffixName = data.SuffixName;
            // model.AlternateFirstName = data.AlternateFirstName;
            // model.AlternateLastName = data.AlternateLastName;
            // model.Nickname = data.Nickname;
            // model.JobTitle = data.JobTitle;
            // model.Description = data.Description;
            // model.UsernameOverride = data.UsernameOverride;
            // model.IsActive = data.IsActive;
            // model.Source = data.Source;
            // model.LastUpdate = lastupdate;
            // model.LastUpdateUser = data.LastUpdateUser;

            // model.PrimaryUnitId = data.PrimaryUnitId;
            // // model.PrimaryUnit = data.PrimaryUnit;
            // model.CreateDate = data.CreateDate;
            // model.CreateUser = data.CreateUser;

            // if (data.Employee != null)
            // {
            //     //sis_staff.StaffId = data.Employee.Sis_Staff.StaffId;
            //     //sis_staff.Email = data.Employee.Sis_Staff.Email;
            //     //sis_staff.ImsuserRecId = data.Employee.Sis_Staff.ImsuserRecId;

            //     employeemodel.EmployeeId = data.Employee.EmployeeId;
            //     employeemodel.FirstName = data.Employee.FirstName;
            //     employeemodel.MiddleName = data.Employee.MiddleName;
            //     employeemodel.LastName = data.Employee.LastName;
            //     employeemodel.SuffixName = data.Employee.SuffixName;
            //     employeemodel.FsfBuildingId = data.Employee.FsfBuildingId;
            //     employeemodel.PersonnelCode = data.Employee.PersonnelCode;
            //     employeemodel.Jobcode = data.Employee.Jobcode;
            //     employeemodel.UnitId = data.Employee.UnitId;
            //     employeemodel.UnitIdManual = data.Employee.UnitIdManual;
            //     employeemodel.JobTitle = data.Employee.JobTitle;
            //     employeemodel.JobTitleManual = data.Employee.JobTitleManual;
            //     employeemodel.EndDate = data.Employee.EndDate;
            //     employeemodel.IsDeleted = data.Employee.IsDeleted;
            //     employeemodel.LastUpdate = data.Employee.LastUpdate;
            //     employeemodel.IsSubstitute = false;
            //     //employeemodel.Sis_Staff = sis_staff;
            //     model.Employee = employeemodel;
            // }
            // else // user doesn't have hr.Employee record yet
            // {
            //     employeemodel.FirstName = model.FirstName;
            //     employeemodel.MiddleName = model.MiddleName;
            //     employeemodel.LastName = model.LastName;
            //     employeemodel.SuffixName = model.SuffixName;
            //     model.Employee = employeemodel;
            // }


            // // flag primary or selected record so it can then be set as the individual jobrecord object
            // if (data.JobRecords.Any() == true)
            // {


            //     {
            //         jobrecordmodel.JobRecordId = -1;
            //         model.JobRecord = jobrecordmodel;
            //     }


            //     // loop through JobRecords


            //     foreach (var jobrecord in data.JobRecords)
            //     {
            //         var jobrecordmodelitem = new JobRecordModel();

            //         if (jobrecord.Title != null)
            //         { 
            //         jobrecordmodelitem.Title.Text = jobrecord.Title.Text;
            //         }
            //         if (jobrecord.Unit != null)
            //         {
            //             jobrecordmodelitem.Unit.Name = jobrecord.Unit.Name;
            //         }
            //         jobrecordmodelitem.IsActive = jobrecord.IsActive;
            //         jobrecordmodelitem.StartDate = jobrecord.StartDate;
            //         jobrecordmodelitem.EndDate = jobrecord.EndDate;
            //         jobrecordmodelitem.JobRecordId = jobrecord.JobRecordId;
            //         jobrecordmodelitem.UnitId = jobrecord.UnitId;
            //         jobrecordmodelitem.BuildingId = jobrecord.BuildingId;
            //         jobrecordmodelitem.SubstituteForId = jobrecord.SubstituteForId;
            //         jobrecordmodelitem.TitleId = jobrecord.TitleId != null ? (int)jobrecord.TitleId : 0;
            //         jobrecordmodelitem.TitleGroupId = jobrecord.TitleGroupId != null ? (int)jobrecord.TitleGroupId : 0;
            //         jobrecordmodelitem.SystemId = (int)jobrecord.SystemId;
            //         jobrecordmodelitem.IsPrimary = jobrecord.IsPrimary;
            //         jobrecordmodelitem.HrWorkLocation = jobrecord.HrWorkLocation;
            //         jobrecordmodelitem.HrRecordNumber = jobrecord.HrRecordNumber;
            //         jobrecordmodelitem.HrPrimary = jobrecord.HrPrimary;
            //         jobrecordmodelitem.HrPersonnelCode = jobrecord.HrPersonnelCode;
            //         jobrecordmodelitem.HrMatched = jobrecord.HrMatched;
            //         jobrecordmodelitem.HrJobCode = jobrecord.HrJobCode;
            //         jobrecordmodelitem.NeedsReview = jobrecord.NeedsReview;
            //         jobrecordmodelitem.CreateDate = jobrecord.CreateDate;
            //         jobrecordmodelitem.CreateUser = jobrecord.CreateUser;
            //         jobrecordmodelitem.LastUpdate = jobrecord.LastUpdate;
            //         jobrecordmodelitem.LastUpdateUser = jobrecord.LastUpdateUser;

            //         model.JobRecords.Add(jobrecordmodelitem);
            //     }




            // }

            // //

            // else
            // {
            //     jobrecordmodel.JobRecordId = -1;
            //     jobrecordmodel.IsPrimary = true;
            //     model.JobRecord = jobrecordmodel;
            // }

            // return model.JobRecords;

        }










        public async Task<AccountModel> GetAccountDetailsByID(int id)
        {
            var accountModel = new AccountModel();
            var employeeModel = new EmployeeModel();
            var studentModel = new StudentModel();
            var unitModel = new Unit();
            var userModel = new UserModel();

            var data = await _db.Accounts
                .Include(x => x.User)
                .Include(x => x.Student)
                .Include(x => x.Employee)
                // .Include(x => x.Contract)
                .Where(x => x.SystemId == id).SingleOrDefaultAsync();





            //FROM[dbo].Account[a]

            //                    LEFT OUTER JOIN[AccountManagement].[ad].[User] us on us.ObjectGuid = a.ADObjectGuid

            //                    LEFT OUTER JOIN[dbo].[Unit] AS[un] ON[a].[PrimaryUnitId] = [un].[UnitId]

            //                    LEFT OUTER JOIN[dbo].[Building] AS[b] ON[un].[BuildingId] = [b].[BuildingId]

            //                    LEFT OUTER JOIN[sis].[Student] AS[s] ON[s].StudentId = [a].StudentId

            //                    LEFT OUTER JOIN[hr].[Employee] AS[e] ON[e].EmployeeId = [a].EmployeeId

            //                    LEFT OUTER JOIN[contractor].[Contract] AS[c] ON[c].ContractorId = [a].ContractorId




            // Get Unit selections and selected item
            if (data.PrimaryUnitId != null)
            {
                var unitData = await _db.Units.FindAsync(data.PrimaryUnitId);
                accountModel.PrimaryUnitId = unitData.UnitId;
            }
            else
            {
                var unitData = await _db.Units.OrderBy(x => x.Name).ToListAsync();

            }





            accountModel.SystemId = data.SystemId;
            accountModel.ADObjectGuid = data.AdobjectGuid;
            accountModel.FirstName = data.FirstName;
            accountModel.MiddleName = data.MiddleName;
            accountModel.LastName = data.LastName;
            accountModel.SuffixName = data.SuffixName;
            accountModel.AlternateFirstName = data.AlternateFirstName;
            accountModel.AlternateLastName = data.AlternateLastName;
            accountModel.Nickname = data.Nickname;
            //DisplayName = account.DisplayName,
            accountModel.JobTitle = data.JobTitle;
            accountModel.Description = data.Description;
            // DisplayJobTitle = account.DisplayJobTitle,
            accountModel.UsernameOverride = data.UsernameOverride;
            accountModel.IsActive = data.IsActive;
            accountModel.Source = data.Source;
            // accountModel.LastUpdate = lastupdate;
            accountModel.LastUpdateUser = data.LastUpdateUser;

            if (data.EmployeeId != null)
            {

                employeeModel.EmployeeId = data.Employee.EmployeeId;
                employeeModel.FsfBuildingId = data.Employee.FsfBuildingId;
                employeeModel.PersonnelCode = data.Employee.PersonnelCode;
                employeeModel.Jobcode = data.Employee.Jobcode;
                employeeModel.UnitId = data.Employee.UnitId;
                employeeModel.UnitIdManual = data.Employee.UnitIdManual;
                employeeModel.JobTitle = data.Employee.JobTitle;
                employeeModel.JobTitleManual = data.Employee.JobTitleManual;
                employeeModel.EndDate = data.Employee.EndDate;
                employeeModel.IsDeleted = data.Employee.IsDeleted;
                employeeModel.LastUpdate = data.Employee.LastUpdate;
                accountModel.Employee = employeeModel;
            }


            if (data.StudentId != null)
            {
                studentModel.StudentId = data.Student.StudentId;
                studentModel.FirstName = data.Student.FirstName;
                studentModel.LastName = data.Student.LastName;
                studentModel.SisBuildingId = data.Student.SisBuildingId;
                studentModel.GradeId = data.Student.GradeId;
                studentModel.CurrentStatus = data.Student.CurrentStatus;
                studentModel.WithdrawalDate = data.Student.WithdrawalDate;
                studentModel.GraduationYear = data.Student.GraduationYear;
                studentModel.StateReportId = data.Student.StateReportId;
                studentModel.LastModified = data.Student.LastModified;

            }
            accountModel.Student = studentModel;

            if (data.AdobjectGuid != null)
            {
                userModel.ObjectGuid = data.User.ObjectGuid;
                userModel.SamaccountName = data.User.SamaccountName;
                userModel.IsDisabled = data.User.IsDisabled;
                userModel.Mail = data.User.Mail;
                userModel.Name = data.User.Name;
                userModel.JobTitle = data.User.JobTitle;
                userModel.OrganizationalUnitGuid = data.User.OrganizationalUnitGuid;
                userModel.Department = data.User.Department;
                userModel.Office = data.User.Office;
                userModel.AdEmployeeId = data.User.AdEmployeeId;
                userModel.WhenChanged = data.User.WhenChanged;
                userModel.WhenCreated = data.User.WhenCreated;
                userModel.IsDeleted = data.User.IsDeleted;

                accountModel.User = userModel;

            }

            return accountModel;

        }


        /// <summary>
        /// Get Unit/Department from cache if it exists, otherwise from database
        /// </summary>
        /// <returns>Units selectlist</returns>

        public SelectList GetUnits()
        {

            SelectList units = null;


            if (!_cache.TryGetValue("Units", out units))
            {
                units = new SelectList(_db.Units.OrderBy(x => x.Name).ToDictionary(m => m.UnitId.ToString(), m => m.Name), "Key", "Value");
                SetCache("Units", units);

            }

            return units;

        }

        /// <summary>
        ///  Get Unit/Department from cache if it exists, otherwise from database - async
        /// </summary>
        /// <returns></returns>
        public async Task<SelectList> GetUnitsAsync()
        {
            SelectList units = null;

            if (!_cache.TryGetValue("Units", out units))
            {
                units = new SelectList(await _db.Units.OrderBy(x => x.Name).ToDictionaryAsync(m => m.UnitId.ToString(), m => m.Name), "Key", "Value");
                SetCache("Units", units);
            }
            return units;

        }

        /// <summary>
        /// Get JobCodes from database 
        /// </summary>
        /// <returns>JobCode selectlist</returns>

        public async Task<SelectList> GetJobCodes()
        {
            SelectList jobCodeData = null;

            if (!_cache.TryGetValue("Jobcodes", out jobCodeData))
            {

                jobCodeData = new SelectList(await _db.Jobcodes
                  .OrderBy(x => x.Description)
                  .ToDictionaryAsync(m => m.JobcodeId, m => m.Description + " (" + m.JobcodeId + ")"), "Key", "Value");

                SetCache("Jobcodes", jobCodeData);
            }
            return jobCodeData;

        }

        /// <summary>
        /// Get Personnel codes from database
        /// </summary>
        /// <returns>Personnel Code selectlist</returns>

        public async Task<SelectList> GetPersonnelCodes()
        {
            SelectList personnelCodeData = null;

            if (!_cache.TryGetValue("PersonnelCodes", out personnelCodeData))
            {
                personnelCodeData = new SelectList(await _db.PersonnelCodes
                .OrderBy(x => x.Description)
                .ToDictionaryAsync(m => m.Personnel_Code, m => m.Description + " (" + m.Personnel_Code + ")"), "Key", "Value");

                SetCache("PersonnelCodes", personnelCodeData);
            }
            return personnelCodeData;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        //public async Task<SelectList> GetAvailableJobTitles()
        //{


        //    IQueryable<Select_Available_JobTitles> query =
        //    _db.Select_Available_JobTitles
        //    .FromSqlInterpolated($"EXEC Select_Available_JobTitles ");



        //   // jobtitledata = await query.ToListAsync();

        //    var jobTitleData = new SelectList(await query.ToDictionaryAsync(x => x.jobcode + ":" + x.personnel_code, x => x.SortCoding + "- " + x.JobTitle + " (" + x.AdditionalText + ")"), "Key", "Value");

        //    return jobTitleData;




        //   }

        /// <summary>
        /// Add dbo.Account and log in dbo.AuditLog
        /// </summary>
        /// <param name="account"></param>
        /// <param name="comment"></param>
        /// <returns>new SystemId</returns>

        public int AddAccount(Account account, string comment)
        {
            var newAccount = new Account()
            {
                EmployeeId = account.EmployeeId,
                AccountTypeId = account.AccountTypeId,
                FirstName = account.FirstName,
                MiddleName = account.MiddleName != null ? account.MiddleName : "",
                LastName = account.LastName,
                Nickname = account.Nickname,
                SuffixName = account.SuffixName != null ? account.SuffixName : "",
                AlternateFirstName = account.AlternateFirstName,
                AlternateLastName = account.AlternateLastName,

                // Description = model.Description,
                UsernameOverride = account.UsernameOverride != null ? account.UsernameOverride : "",
                Source = account.Source,
                LastUpdate = account.CreateDate,
                LastUpdateUser = account.CreateUser,
                IsActive = true,
                PrimaryUnitId = account.PrimaryUnitId,
                //TitleGroupId = account.TitleGroupId,
                // TitleId = account.TitleId,
                JobTitle = account.JobTitle,
                CreateDate = account.CreateDate,
                CreateUser = account.CreateUser
            };



            _db.Accounts.Add(newAccount);
            _db.SaveChanges();

            comment = "for [SystemId]: [" + newAccount.SystemId.ToString() + "] " + comment;

            _log.Log((int)ActionTypes.Create, "dbo.Account", newAccount.SystemId.ToString(), comment);

            return newAccount.SystemId;


        }


        /// <summary>
        /// Add dbo.JobRecord and log in dbo.AuditLog
        /// </summary>
        /// <param name="jobRecord"></param>
        /// <param name="comment"></param>
        /// <returns>new JobRecordId</returns>

        public int AddJobRecord(JobRecord jobRecord, string comment)
        {
            var newJobRecord = new JobRecord()
            {

                IsPHRST = jobRecord.IsPHRST,
                IsPrimary = true,
                UnitId = jobRecord.UnitId,
                BuildingId = jobRecord.BuildingId,
                SubstituteForId = jobRecord.SubstituteForId,
                TitleGroupId = jobRecord.TitleGroupId,
                TitleId = jobRecord.TitleId,
                SystemId = jobRecord.SystemId,
                StartDate = jobRecord.StartDate,
                EndDate = jobRecord.EndDate,
                CreateDate = jobRecord.CreateDate,
                CreateUser = jobRecord.CreateUser,
                HrJobCode = jobRecord.HrJobCode,
                HrPersonnelCode = jobRecord.HrPersonnelCode,
                HrWorkLocation = jobRecord.HrWorkLocation,
                IsActive = true,
                LastUpdate = jobRecord.CreateDate,
                LastUpdateUser = jobRecord.CreateUser,
                NeedsReview = false
            };


            _db.JobRecord.Add(newJobRecord);
            _db.SaveChanges();

            comment = "for [JobRecordId]: [" + newJobRecord.JobRecordId.ToString() + "] " + comment;


            _log.Log((int)ActionTypes.Create, "dbo.JobRecord", newJobRecord.JobRecordId.ToString(), comment);

            return newJobRecord.JobRecordId;


        }





        /// <summary>
        /// Get active dbo.Title records from database 
        /// </summary>
        /// <returns>Title selectlist </returns>

        public async Task<SelectList> GetTitles()
        {

            SelectList titles = null;

            if (!_cache.TryGetValue("Titles:0", out titles))
            {

                titles = new SelectList(await _db.Titles
                .Where(x => x.Active == true)
                .OrderBy(x => x.Text)
                .ToDictionaryAsync(m => m.TitleId, m => m.Text), "Key", "Value");

                SetCache("Titles:0", titles);
                // titles.Append(new SelectListItem() { Text = base.GetCacheItemInfo((MemoryCache)_cache).SingleOrDefault().AbsoluteExpiry.ToString()   , Value = "0" });
            }

            return titles;

        }

        public void RefreshTitles()
        {

            base.ClearCache();
        }

        /// <summary>
        /// Get individual dbo.Title record based on titleid
        /// </summary>
        /// <param name="titleid"></param>
        /// <returns>string title</returns>

        public string GetTitlebyTitleId(int titleId)
        {
            string title;

            var query = _db.Titles


                    .Where(x => x.TitleId == titleId)
                    .SingleOrDefault();

            if (query?.Text != null)
            {
                title = query.Text;
            }
            else
            {
                title = "";
            }
            return title;
        }

        /// <summary>
        /// Get dbo.TitleGroup records from database
        /// </summary>
        /// <returns>TitleGroup selectlist</returns>

        public async Task<SelectList> GetTitleGroups()
        {
            SelectList titleGroups = null;


            if (!_cache.TryGetValue("TitleGroups", out titleGroups))
            {
                titleGroups = new SelectList(await _db.TitleGroups
                .Where(x => x.Active == true)
                .OrderBy(x => x.Description)
                .ToDictionaryAsync(m => m.TitleGroupId, m => m.Description), "Key", "Value");

                SetCache("TitleGroups", titleGroups);

            }


            return titleGroups;

        }

        /// <summary>
        /// Get dbo.AccountType records from database
        /// </summary>
        /// <returns>AccountType selectlist</returns>

        public async Task<SelectList> GetAccountTypes()
        {
            SelectList accountTypes = null;


            if (!_cache.TryGetValue("AccountTypes", out accountTypes))
            {
                accountTypes = new SelectList(await _db.AccountTypes

                .Where(x => x.IsStaff == true)
                .OrderBy(x => x.AccountTypeId)
                .ToDictionaryAsync(m => m.AccountTypeId, m => m.Description), "Key", "Value");

                SetCache("AccountTypes", accountTypes);

            }


            return accountTypes;

        }

        /// <summary>
        /// Get dbo.Building records from database
        /// </summary>
        /// <returns>Building selectlist</returns>
        public async Task<SelectList> GetBuildings()
        {
            SelectList buildings = null;

            if (!_cache.TryGetValue("Buildings", out buildings))
            {
                buildings = new SelectList(await _db.Buildings
               // .Where(x => x.Active == true)
               .OrderBy(x => x.Name)
                .ToDictionaryAsync(m => m.BuildingId, m => m.Name), "Key", "Value");
                SetCache("Buildings", buildings);
            }
            return buildings;

        }

        /// <summary>
        /// Set hr.building records from database
        /// </summary>
        /// <returns>hr.building selectlist</returns>
        public async Task<SelectList> GetfsfBuildings()
        {

            var fsfBuildingData = new SelectList(await _db.Hr_Buildings
                // .Where(x => x.Active == true)
                .OrderBy(x => x.Description)
                .ToDictionaryAsync(m => m.FsfBuildingId, m => m.Description), "Key", "Value");

            return fsfBuildingData;

        }

        /// <summary>
        /// Get dbo.TitleGroup records based on titleid
        /// </summary>
        /// <param name="titleid"></param>
        /// <returns>TitleGroup selectlist</returns>
        public async Task<SelectList> GetTitleGroupsByTitle(int titleId)
        {

            var titleGroupData = new SelectList(await _db.TitleGroup_Titles
                 .Include(titlegrouptitles => titlegrouptitles.Title)
                 .Include(titlegrouptitles => titlegrouptitles.TitleGroup)
                 .Where(x => x.TitleId == titleId)
                 .Where(x => x.Active == true)
                 .Where(x => x.Title.Active == true)
                 .Where(x => x.TitleGroup.Active == true)
                 .OrderBy(x => x.TitleGroup.Description)
                .ToDictionaryAsync(m => m.TitleGroupId, m => m.TitleGroup.Description), "Key", "Value");

            return titleGroupData;

        }

        /// <summary>
        /// Get dbo.Building records from database based on unitid
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns>Building selectlist</returns>
        public async Task<SelectList> GetBuildingsByUnit(int unitId)
        {
            SelectList buildings = null;

            if (!_cache.TryGetValue("Buildings:" + unitId, out buildings))
            {
                buildings = new SelectList(await _db.UnitBuildings
                 .Include(unitbuildings => unitbuildings.Building)
                 .Include(unitbuildings => unitbuildings.Unit)
                 .Where(x => x.UnitId == unitId)
                 .Where(x => x.Active == true).OrderBy(x => x.Building.Name)
                .ToDictionaryAsync(m => m.BuildingId, m => m.Building.Name), "Key", "Value");
                SetCache("Buildings:" + unitId, buildings);
            }
            return buildings;

        }

        /// <summary>
        /// Get dbo.Title records based on titlegroupid
        /// </summary>
        /// <param name="titleGroupId"></param>
        /// <returns>titles selectlist</returns>

        public async Task<SelectList> GetTitlesByTitleGroup(int titleGroupId)
        {

            SelectList titles = null;

            if (!_cache.TryGetValue("Units:" + titleGroupId, out titles))
            {


                titles = new SelectList(await _db.TitleGroup_Titles
                 .Include(titlegrouptitles => titlegrouptitles.Title)
                 .Include(titlegrouptitles => titlegrouptitles.TitleGroup)
                 .Where(x => x.TitleGroupId == titleGroupId)
                 .Where(x => x.Active == true)
                 .Where(x => x.Title.Active == true)
                 .Where(x => x.TitleGroup.Active == true).OrderBy(x => x.Title.Text)
                .ToDictionaryAsync(m => m.TitleId, m => m.Title.Text), "Key", "Value");

                SetCache("Units:" + titleGroupId, titles);
            }
            return titles;

        }

        /// <summary>
        /// Get PHRST data (WorkLocation, PersonnelCode, JobCode) for a given employee from dbo.EmployeeJobs
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>EmployeeJob selectlist</returns>

        public async Task<SelectList> GetEmployeeJobsSelectList(int employeeId)
        {

            SelectList employeeJobs = null;

            //if (!_cache.TryGetValue("EmployeeJobs:" + employeeid, out employeejobs))
            //{


            employeeJobs = new SelectList(await _db.EmployeeJobs
                .Include(x => x.Building)
                .Include(x => x.Unit)
                .Include(x => x.Jobcodes)
                .Include(x => x.PersonnelCodes)
                .Where(x => x.EmployeeId == employeeId)


                .ToDictionaryAsync(m => m.FsfBuildingId + "#" + m.PersonnelCode + "#" + m.Jobcode + "#" + m.UnitId + "#" + m.EffectiveDate,

                                    m => (m.Building == null ? "" : (m.Building.Description ?? ""))
                                    + "-" + (m.PersonnelCodes == null ? "" : (m.PersonnelCodes.Description ?? ""))
                                    + "-" + (m.Jobcodes == null ? "" : (m.Jobcodes.Description ?? ""))
                                     + "-" + (m.UnitId == null ? "" : (m.Unit.Name ?? ""))
                                       + "-" + (m.EffectiveDate == null ? "" : (m.EffectiveDate))
                                    ),

                                    "Key",

                                    "Value");

            //    SetCache("EmployeeJobs:" + employeeid, employeejobs);
            //}

            return employeeJobs;

        }


        public async Task<List<EmployeeJobModel>> GetEmployeeJobs(int employeeId)
        {

            List<EmployeeJobModel> employeeJobs = new List<EmployeeJobModel>();

            var employeeJobData = new List<EmployeeJob>();

            IQueryable<EmployeeJob> employeeJobQuery = _db.EmployeeJobs;

            employeeJobQuery = employeeJobQuery.Include(x => x.Building).Where(x => x.FsfBuildingId != null);
            employeeJobQuery = employeeJobQuery.Include(x => x.PersonnelCodes).Where(x => x.PersonnelCode != null);
            employeeJobQuery = employeeJobQuery.Include(x => x.Unit).Where(x => x.UnitId != null);
            employeeJobQuery = employeeJobQuery.Include(x => x.Jobcodes).Where(x => x.Jobcode != null);
            employeeJobQuery = employeeJobQuery.Where(x => x.EmployeeId == employeeId);

            employeeJobData = await employeeJobQuery.ToListAsync();

            if (employeeJobData?.Any() == true)
            {

                foreach (var employeeJob in employeeJobData)
                {
                    var employeeJobModelItem = new EmployeeJobModel();

                    employeeJobModelItem.EmployeeId = employeeJob.EmployeeId;
                    employeeJobModelItem.FsfBuildingId = employeeJob.FsfBuildingId;
                    employeeJobModelItem.PersonnelCode = employeeJob.PersonnelCode;
                    employeeJobModelItem.Jobcode = employeeJob.Jobcode;
                    employeeJobModelItem.UnitId = employeeJob.UnitId;
                    employeeJobModelItem.IsPrimary = employeeJob.IsPrimary;
                    employeeJobModelItem.Buildings.Description = employeeJob.Building.Description;
                    employeeJobModelItem.PersonnelCodes.Description = employeeJob.PersonnelCodes == null ? "" : employeeJob.PersonnelCodes.Description;
                    employeeJobModelItem.Jobcodes.Description = employeeJob.Jobcodes == null ? "" : employeeJob.Jobcodes.Description;

                    employeeJobs.Add(employeeJobModelItem);



                }

            }
            return employeeJobs;

        }



        /// <summary>
        /// Get hr.employee name based on employeeid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>EmployeeModel</returns>

        public EmployeeModel GetEmployee(int employeeId)
        {
            var employee = new EmployeeModel();


            var query = _db.Employees


                    .Where(e => e.EmployeeId == employeeId)
                    .SingleOrDefault();

            employee.EmployeeId = employeeId;
            employee.FirstName = query.FirstName;
            employee.MiddleName = query.MiddleName;
            employee.LastName = query.LastName;
            employee.SuffixName = query.SuffixName;


            return employee;
        }

        public async Task<EmployeeModel> GetEmployeePHRSTdata(int employeeId)
        {
            var employee = new EmployeeModel();
            var employeeJob = new EmployeeJobModel();



            var employeeQuery = from e in _db.Employees

                                join ej in _db.EmployeeJobs
                               on e.EmployeeId equals ej.EmployeeId


                                join u in _db.Units
                                on ej.UnitId equals u.UnitId into eju
                                from u in eju.DefaultIfEmpty()

                                join pc in _db.PersonnelCodes
                                on ej.PersonnelCode equals pc.Personnel_Code into ejpc
                                from pc in ejpc.DefaultIfEmpty()

                                join jc in _db.Jobcodes
                                on ej.Jobcode equals jc.JobcodeId into jcpc
                                from jc in jcpc.DefaultIfEmpty()

                                join b in _db.Buildings
                                on ej.FsfBuildingId equals b.FsfBuildingId into ejb
                                from b in ejb.DefaultIfEmpty()

                                select new
                                {
                                    EmployeeId = e.EmployeeId,
                                    FirstName = e.FirstName,
                                    MiddleName = e.MiddleName,
                                    LastName = e.LastName,
                                    SuffixName = e.SuffixName,
                                    UnitId = ej.UnitId ?? 0,
                                    BuildingId = b.BuildingId,
                                    JobTitle = e.JobTitle,
                                    IsPrimary = ej.IsPrimary,
                                    FsfBuildingId = ej.FsfBuildingId,
                                    HrRecordNumber = ej.JobId,
                                    HrPrimary = ej.IsPrimary,
                                    HrPersonnelCode = ej.PersonnelCode,
                                    HrJobCode = ej.Jobcode,
                                    BuildingName = b.Name,
                                    UnitName = u.Name

                                };


            employeeQuery = employeeQuery.Where(x => x.EmployeeId == employeeId);

            var employeeQueryData = await employeeQuery.FirstOrDefaultAsync();


            employee.EmployeeId = employeeQueryData.EmployeeId;
            employee.FirstName = employeeQueryData.FirstName;
            employee.MiddleName = employeeQueryData.MiddleName;
            employee.LastName = employeeQueryData.LastName;
            employee.SuffixName = employeeQueryData.SuffixName;
            employee.JobTitle = employeeQueryData.JobTitle;

            employeeJob.EmployeeId = employeeQueryData.EmployeeId;
            employeeJob.UnitId = employeeQueryData.UnitId;
            employeeJob.FsfBuildingId = employeeQueryData.FsfBuildingId;
            employeeJob.Jobcode = employeeQueryData.HrJobCode;
            employeeJob.PersonnelCode = employeeQueryData.HrPersonnelCode;
            employeeJob.IsPrimary = employeeQueryData.IsPrimary;

            employee.EmployeeJob = employeeJob;


            return employee;
        }



        /// <summary>
        /// Get list of dbo.Account records that are contractors
        /// </summary>
        /// <returns>AccountModel list</returns>
        public async Task<List<AccountModel>> GetContractors()


        {



            var accountModelList = new List<AccountModel>();


            //var predicateEmployee = PredicateBuilder.New<Account>(true);
            //predicateEmployee = predicateEmployee.Or(a => a.Employee.IsDeleted != true);
            //predicateEmployee = predicateEmployee.Or(a => !_db.Employees.Any(e => e.EmployeeId == a.EmployeeId));

            //var predicateJobRecord = PredicateBuilder.New<Account>(true);
            //predicateJobRecord = predicateJobRecord.Or(a => a.JobRecords.Any(r => r.IsPHRST == false));
            //predicateJobRecord = predicateJobRecord.Or(a => !_db.JobRecord.Any(r => r.SystemId == a.SystemId));

            //IQueryable<Account> query = _db.Accounts

            //                            .Include(account => account.PrimaryUnit).ThenInclude(unit => unit.Building)
            //                            .Where(predicateEmployee)
            //                            .Where(predicateJobRecord)

            //                           ;
            //query = query.Where(x => x.IsActive != false);


            //query = query.Where(x => x.Source != "STU" && x.Source != null);




            var query = from a in _db.Accounts

                        join jr in _db.JobRecord_Ordered.Where(e => e.Order == 1)
                             on a.SystemId equals jr.SystemId

                             into ajr
                        from jr in ajr.DefaultIfEmpty() // technique for outer join 

                        join a2 in _db.Accounts
                            on jr.SubstituteForId equals a2.SystemId into jra2
                        from a2 in jra2.DefaultIfEmpty()

                        join at in _db.AccountTypes
                           on a.AccountTypeId equals at.AccountTypeId into aat
                        from at in aat.DefaultIfEmpty()

                        join t in _db.Titles
                            on jr.TitleId equals t.TitleId into jrt
                        from t in jrt.DefaultIfEmpty()

                        join tg in _db.TitleGroups
                            on jr.TitleGroupId equals tg.TitleGroupId into jrtg
                        from tg in jrtg.DefaultIfEmpty()

                        join u in _db.Units
                            on jr.UnitId equals u.UnitId into jru
                        from u in jru.DefaultIfEmpty()

                        join bg in _db.Buildings
                            on jr.BuildingId equals bg.BuildingId into jrbg
                        from bg in jrbg.DefaultIfEmpty()


                        join e in _db.Employees
                            on a.EmployeeId equals e.EmployeeId into ae
                        from e in ae.DefaultIfEmpty()

                        join us in _db.Users
                           on a.AdobjectGuid equals us.ObjectGuid into aus
                        from us in aus.DefaultIfEmpty()

                        join ou in _db.OrganizationalUnits
                         on us.OrganizationalUnitGuid equals ou.ObjectGuid into usou
                        from ou in usou.DefaultIfEmpty()



                        select new
                        {
                            SystemId = a.SystemId,
                            EmployeeId = a.EmployeeId == null ? 0 : a.EmployeeId ?? 0,
                            FirstName = (a.FirstName == null || a.FirstName == "") ? (e.FirstName ?? (us.GivenName ?? "")) : a.FirstName ?? "",
                            LastName = (a.LastName == null || a.LastName == "") ? (e.LastName ?? (us.Surname ?? "")) : a.LastName ?? "",
                            AccountTypeId = at.AccountTypeId == null ? 0 : at.AccountTypeId,
                            JobTitle = (a.JobTitle == null || a.JobTitle == "") ? (e.JobTitle ?? (us.JobTitle ?? "")) : a.JobTitle ?? "",
                            Title = t.Text ?? "",
                            TitleGroup = tg.Description ?? "",
                            Department = u.Name ?? "",
                            //Department = (u.Name == null || u.Name == "") ? u2.Name : u.Name ,
                            Building = bg.Name ?? "",
                            LastUpdate = a.LastUpdate == null ? (e.LastUpdate == null ? null : e.LastUpdate) : a.LastUpdate ?? null,
                            IsDeleted = e.IsDeleted == null ? false : e.IsDeleted,
                            IsActive = a.IsActive == null ? true : a.IsActive,
                            AccountExpires = us.AccountExpires,
                            AlternateFirstName = a.AlternateFirstName ?? "",
                            AlternateLastName = a.AlternateLastName ?? "",
                            UsernameOverride = a.UsernameOverride ?? "",
                            //PrimaryUnitId = a.PrimaryUnitId == null ? 0 : a.PrimaryUnitId,
                            PrimaryUnitId = jr.UnitId ?? 0,
                            TitleId = jr.TitleId ?? 0,
                            Jobcode = e.Jobcode == null ? 0 : e.Jobcode,
                            PersonnelCode = e.PersonnelCode == null ? "" : e.PersonnelCode,
                            BuildingId = jr.BuildingId == null ? 0 : jr.BuildingId ?? 0,
                            Source = a.Source ?? "",
                            SubstituteFor = (a2.FirstName + " " + a2.LastName) ?? "",
                            UserIsDisabled = us.IsDisabled == null ? false : us.IsDisabled,
                            UserIsDeleted = us.IsDeleted == null ? false : us.IsDeleted,
                            OrganizationalUnit = ou.OrganizationalUnitName,
                            StartDate = jr.StartDate,
                            EndDate = jr.EndDate
                        };


            query = query.Where(x => x.Source != "STU" && x.Source != null);

            query = query.Where(x => (x.AccountExpires ?? DateTime.Now) > Settings.SixMonthsAgo);


            query = query.Where(x => x.FirstName.StartsWith("BSD") == false);

            query = query.Where(x =>
                        x.OrganizationalUnit == null ||
                        //   x.OrganizationalUnit == "_Disabled User Accounts" ||
                        x.OrganizationalUnit == "Student Support" ||
                        x.OrganizationalUnit == "Users");


            query = query.Where(x =>
                       x.AccountTypeId == (int)EnumAccountTypeDescription.Contractor || x.AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub ||
                       x.EmployeeId == 0
                       );


            var totalCount = query.Count();

            var accountDataList = await query.ToListAsync();




            // System.DateTime? lastupdate;

            if (accountDataList?.Any() == true)
            {
                foreach (var account in accountDataList)
                {


                    //AccountModel accountmodel = new AccountModel();
                    //accountmodellist.Add(accountmodel.Load(account));

                    AccountModel accountModel = new AccountModel();


                    accountModel.SystemId = account.SystemId;
                    accountModel.EmployeeId = account.EmployeeId;
                    accountModel.FirstName = account.FirstName;
                    accountModel.LastName = account.LastName;
                    accountModel.AccountTypeId = account.AccountTypeId;
                    accountModel.JobTitle = account.JobTitle;
                    accountModel.JobRecord.Title.Text = account.Title;
                    accountModel.JobRecord.TitleGroup.Description = account.TitleGroup;
                    accountModel.JobRecord.SubstituteFor = account.SubstituteFor;
                    accountModel.JobRecord.Unit.Name = account.Department;
                    accountModel.JobRecord.Unit.Building.Name = account.Building;
                    accountModel.JobRecord.StartDate = account.StartDate;
                    accountModel.JobRecord.EndDate = account.EndDate;
                    accountModel.LastUpdate = account.LastUpdate;
                    accountModel.Employee.IsDeleted = account.IsDeleted;
                    accountModel.IsActive = account.IsActive;
                    accountModel.User.IsDisabled = account.UserIsDisabled;
                    accountModel.User.IsDeleted = account.UserIsDeleted;
                    accountModel.User.AccountExpires = account.AccountExpires;


                    accountModelList.Add(accountModel);




                }

            }


            return accountModelList;


        }

        /// <summary>
        /// Get dbo.AuditLog records based on criteria (SystemId, JobRecordId, User.ObjectGuid, name)
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="jobRecordId"></param>
        /// <param name="objectGuid"></param>
        /// <param name="name"></param>
        /// <returns>AuditLogView list</returns>

        public List<AuditLogView> GetAuditLog(int systemId, int jobRecordId, Guid objectGuid, string name, int employeeId)

        {


            // Select query to the AuditLog SQL View 
            IQueryable<AuditLogView> query = _db.AuditLogView;

            //  create predicate builder object to concatonate OR statements together   
            var predicateAuditLogView = PredicateBuilder.New<AuditLogView>(true);

            // Account match
            predicateAuditLogView = predicateAuditLogView.Or(a => a.ReferenceTable.Equals("dbo.Account") && a.ReferenceID.Equals(systemId.ToString()));


            if (jobRecordId != 0)
            { // or Job Record match
                predicateAuditLogView = predicateAuditLogView.Or(a => a.ReferenceTable.Equals("dbo.JobRecord") && a.ReferenceID.Equals(jobRecordId.ToString()));
            }

            if (employeeId != 0)
            {
                predicateAuditLogView = predicateAuditLogView.Or(a => a.ReferenceTable.Equals("hr.Employee") && a.ReferenceID.Equals(employeeId.ToString()));
                predicateAuditLogView = predicateAuditLogView.Or(a => a.ReferenceTable.Equals("hr.EmployeeJob") && a.ReferenceID.Equals(employeeId.ToString()));
            }

            if (objectGuid != Guid.Empty)
            {    //  or Active Directory log record match 
                predicateAuditLogView = predicateAuditLogView.Or(a => a.ReferenceTable.Equals("ad.User") && a.ReferenceID.Equals(objectGuid.ToString()));
            }



            //  or  Name match across all records
            predicateAuditLogView = predicateAuditLogView.Or(a => a.Employee.Equals(name));


            // order by  Log Date descending
            query = query.Where(predicateAuditLogView).OrderByDescending(x => x.LogDate);


            var querydata = query.ToList();


            return querydata;


        }

        public async Task<List<Select_PHRST_Mismatched_Employees>> GetPHRSTMismatchedRecordsByEmployee(int systemId)

        {



            //if (!_cache.TryGetValue("StaffGroupAssociations:" + staffgroupassociationid, out employees))
            //{

            var employeeDataList = new List<Select_PHRST_Mismatched_Employees>();


            IQueryable<Select_PHRST_Mismatched_Employees> query =
                _db.Select_PHRST_Mismatched_Employees
                .FromSqlInterpolated($"EXEC Select_PHRST_Mismatched_Employees {systemId}");


            employeeDataList = await query.ToListAsync();

            //SetCache("StaffGroupAssociations:" + staffgroupassociationid, employees);
            // }

            return employeeDataList;


        }


    }
}

