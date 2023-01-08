using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
    public class JobRecordRepository : BaseRepository, IJobRecordRepository
    {

        public JobRecordRepository(accountmanagementContext db, IAuditLoggerService log, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(db, log, contextAccessor, memoryCache, appUser)
        {



        }


        public async Task<List<JobRecordModel>> GetJobRecords(int systemid, int jobrecordid)


        {
            // var jobrecord = JobRecord();


            var jobrecordmodellist = new List<JobRecordModel>();

            var jobrecorddatalist =

                           from j in _db.Set<JobRecord>()

                           join a in _db.Set<Account>() on j.SystemId equals a.SystemId
                           join e in _db.Set<Employee>() on a.EmployeeId equals e.EmployeeId

                           join ej in _db.EmployeeJobs
                                   on new { EmployeeId = a.EmployeeId, HrRecordNumber = j.HrRecordNumber }
                                   equals new { EmployeeId = (int?)ej.EmployeeId, HrRecordNumber = (int?)ej.JobId } into employeejobinfo

                           from eji in employeejobinfo

                           where j.SystemId == systemid
                           select new
                           {
                               ejIsPrimary = eji.IsPrimary,
                               eji.FsfBuildingId,
                               ejPersonnelCode = eji.PersonnelCode,
                               ejJobcode = eji.Jobcode,

                               j.IsPHRST,
                               j.IsActive,
                               j.StartDate,
                               j.EndDate,
                               j.HrRecordNumber,
                               j.HrJobCode,
                               j.HrPrimary,
                               j.HrWorkLocation,
                               j.HrMatched,
                               j.HrPersonnelCode,

                               j.SystemId,
                               e.JobTitle,
                               j.UnitId,
                               j.CreateDate,
                               j.CreateUser,
                               j.LastUpdate,
                               j.LastUpdateUser,
                               j.NeedsReview,
                               j.JobRecordId,
                               j.IsPrimary
                           };





            await jobrecorddatalist.ToListAsync<dynamic>();

            if (jobrecorddatalist?.Any() == true)
            {
                foreach (var jobrecord in jobrecorddatalist)
                {

                    jobrecordmodellist.Add(new JobRecordModel()
                    {
                        JobRecordId = jobrecord.JobRecordId,
                        SystemId = jobrecord.SystemId,
                        employeeJobTitle = jobrecord.JobTitle,
                        IsActive = jobrecord.IsActive,
                        UnitId = jobrecord.UnitId,
                        HrJobCode = jobrecord.HrJobCode,
                        HrMatched = jobrecord.HrMatched,
                        HrPrimary = jobrecord.HrPrimary,
                        HrRecordNumber = jobrecord.HrRecordNumber,
                        HrPersonnelCode = jobrecord.HrPersonnelCode,
                        HrWorkLocation = jobrecord.HrWorkLocation,
                        employeejobHrJobCode = jobrecord.ejJobcode.ToString(),
                        employeejobHrPersonnelCode = jobrecord.ejPersonnelCode,
                        employeejobIsPrimary = jobrecord.ejIsPrimary,
                        StartDate = jobrecord.StartDate,
                        EndDate = jobrecord.EndDate,
                        CreateDate = jobrecord.CreateDate,
                        CreateUser = jobrecord.CreateUser,
                        LastUpdate = jobrecord.LastUpdate,
                        LastUpdateUser = jobrecord.LastUpdateUser,
                        NeedsReview = jobrecord.NeedsReview,
                        IsPrimary = jobrecord.IsPrimary,
                        IsCurrentJobRecord = (jobrecord.JobRecordId == jobrecordid)
                    }









                        );
                }

            }


            return jobrecordmodellist;


        }



        public List<dynamic> GetJobRecordsNeedingReview()


        {
            //var accountmodel = new AccountModel();
            //var employeemodel = new EmployeeModel();

            //var jobrecordmodellist = new List<JobRecordModel>();

            var jobrecorddatalist =

                          _db.JobRecord.Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.Unit)
                             .Include(x => x.Account).ThenInclude(x => x.PrimaryUnit)
                             //  .Include(x => x.Account).ThenInclude(x => x.Title)
                             // .Include(x => x.Account).ThenInclude(x => x.TitleGroup)
                             .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.EmployeeJobs)
                             .Include(j => j.Unit)
                             .Where(x => x.NeedsReview == true)
                             .Select(n => new
                             {
                                 Name = n.Account.FirstName + " " + n.Account.LastName,
                                 Review = _httpContextAccessor.HttpContext.Request.Scheme +
                                 "://" + _httpContextAccessor.HttpContext.Request.Host.Value.ToString() +
                                 "/jobrecord/edit/" + n.JobRecordId + "?systemid=" + n.SystemId
                             })
                             .ToList<dynamic>();




            return jobrecorddatalist;


        }


        //public async Task<List<EmployeeModel>> LoadUnlinkedEmployees()
        //{
        //    //add unit filter 


        //    //  var predicateNeedsReview = PredicateBuilder.New<Employee>(true);
        //    // predicateNeedsReview = predicateNeedsReview.Or(a => _db.NeedsReview.Any(e => e.ReferenceId == a.EmployeeId ));
        //    // predicateNeedsReview = predicateNeedsReview.Or(a => !_db.NeedsReview.Any(e => e.ReferenceId == a.EmployeeId)  );



        //    IQueryable<Employee> query = _db.Employees
        //                  .Include(x => x.Unit)
        //                  .Include(x => x.FsfBuilding)
        //                  // .Include(x => x.NeedsReview)
        //                  .Where(e => !_db.Accounts.Any(a => a.EmployeeId == e.EmployeeId))
        //                  .Where(e => e.IsDeleted == false)
        //                  .Where(e => e.EndDate == null || e.EndDate > DateTime.Now);

        //    ////if (ViewBag.ShowHandled == null || ViewBag.ShowHandled  != "true")
        //    ////  {
        //    ////      query = query.Where(e => e.NeedsReview.needsReview == true);
        //    ////  }

        //    // .Where(predicateNeedsReview)


        //    //  query = query.OrderBy(x => x.NeedsReview.needsReview).ThenBy(x => x.Unit.Name).ThenBy(x => x.JobTitle);

        //    var employeemodellist = new List<EmployeeModel>();
        //    //var employedatalist = new List<Employee>();

        //    // employedatalist = await query.ToListAsync();
        //    var employedatalist = await query.Select(n => new
        //    {

        //        //  NeedsReviewID = n.NeedsReview.NeedsReviewId,
        //        //   NeedsReview = n.NeedsReview.needsReview,
        //        EmployeeId = n.EmployeeId,
        //        Name = n.Unit.Name,
        //        Reason = n.
        //        FirstName = n.FirstName,
        //        LastName = n.LastName,
        //        JobTitle = n.JobTitle,
        //        Building = n.FsfBuilding.Description,
        //        Jobcode = n.Jobcode,
        //        PersonnelCode = n.PersonnelCode,
        //        //  Comment = n.NeedsReview.Comment
        //    }
        //    ).ToListAsync<dynamic>();


        //    if (employedatalist?.Any() == true)
        //    {
        //        foreach (var employee in employedatalist)
        //        {
        //            var fsfbuilding = new hr_Building();
        //            //  var needsReview = new NeedsReview();
        //            var unit = new Unit();
        //            fsfbuilding.Description = employee.Building;
        //            // needsReview.NeedsReviewId = employee.NeedsReviewID;
        //            //  needsReview.needsReview = employee.NeedsReview;
        //            // needsReview.Comment = employee.Comment;
        //            unit.Name = employee.Name;

        //            employeemodellist.Add(new EmployeeModel()
        //            {

        //                EmployeeId = employee.EmployeeId,
        //                FirstName = employee.FirstName,
        //                LastName = employee.LastName,
        //                // UnitId = employee.UnitId,
        //                Unit = unit,
        //                JobTitle = employee.JobTitle,

        //                // FsfBuildingId = employee.FsfBuildingId,
        //                fsfBuilding = fsfbuilding,
        //                Jobcode = employee.Jobcode,
        //                PersonnelCode = employee.PersonnelCode,
        //                //  EmployeeStatus = employee.EmployeeStatus,

        //                // NeedsReview = needsReview

        //                //EncryptedRouteId = _protector.Encode(employee.EmployeeId.ToString()).Substring(0,10).ToString()


        //            }
        //                );
        //            employeemodellist.LastOrDefault().EncryptedRouteId = employeemodellist.LastOrDefault().Slug.ToString();
        //            //  employeemodellist.LastOrDefault().NeedsReview.NeedsReviewId = employee.NeedsReviewID;
        //            _appUser.SaveEncryptedValue(_httpContextAccessor.HttpContext, employeemodellist.LastOrDefault().Slug.ToString(), employee.EmployeeId.ToString());

        //        }

        //    }
        //    return employeemodellist;
        //}




        internal static T Cast<T>(object target, T example)

        { return (T)target; }

        public async Task<List<AccountModel>> GetAccounts(string search, int strddlUnitValue)


        {   // Ex.  var products = context.Prducts.Where(p => p.CategoryId == 1 && p.UnitsInStock < 10);
            //  var data = db.LenderProgram.Where(i => DbFunctions.Like(i.LenderProgramCode, "OTO%"))
            //.ToList();


            var accountmodellist = new List<AccountModel>();
            var accountdatalist = new List<Account>();

            IQueryable<Account> query = _db.Accounts
                                        //   .Include(account => account.Title)
                                        //   .Include(account => account.TitleGroup)
                                        .Include(account => account.PrimaryUnit)
                                       //.ThenInclude(accountunit => accountunit.Unit)
                                       ;
            query = query.Where(x => x.IsActive != false);

            if (search != null)
            {
                query = query.Where(x => x.LastName.Contains(search));
            }

            if (strddlUnitValue != 0)
            {
                query = query.Where(x => x.AccountUnit.UnitId.Equals(strddlUnitValue));
            }



            query = query.Where(x => x.Source != "STU");

            ////if (search == null && strddlUnitValue == 0)
            ////{
            ////    query = query.Take(500);
            ////}


            accountdatalist = await query.OrderBy(x => x.LastName).ToListAsync();



            // System.DateTime? lastupdate ;

            if (accountdatalist?.Any() == true)
            {
                foreach (var account in accountdatalist)
                {
                    //if (account.LastUpdate.HasValue)
                    //{  lastupdate = (DateTime)account.LastUpdate; }
                    //else
                    //{
                    //    lastupdate = null;
                    //}
                    AccountModel accountmodel = new AccountModel();
                    accountmodellist.Add(accountmodel.Load(account));

                    //  accountmodellist.Add(new AccountModel()
                    ////{
                    ////    SystemId = account.SystemId,
                    ////    EmployeeId = account.EmployeeId,
                    ////    ADObjectGuid = account.AdobjectGuid,
                    ////    FirstName = account.FirstName,
                    ////    MiddleName = account.MiddleName,
                    ////    LastName = account.LastName,
                    ////    SuffixName = account.SuffixName,
                    ////    AlternateFirstName = account.AlternateFirstName,
                    ////    AlternateLastName = account.AlternateLastName,
                    ////    Nickname = account.Nickname,
                    ////    //DisplayName = account.DisplayName,
                    ////    JobTitle = account.JobTitle,
                    ////    Description = account.Description,
                    ////    // DisplayJobTitle = account.DisplayJobTitle,
                    ////    UsernameOverride = account.UsernameOverride,
                    ////    IsActive = account.IsActive,
                    ////    Source = account.Source,
                    ////    LastUpdate = lastupdate,
                    ////    LastUpdateUser = account.LastUpdateUser,
                    ////    PrimaryUnitId = account.PrimaryUnitId,
                    ////    PrimaryUnit = account.PrimaryUnit,
                    ////    //TitleId = (int)account.TitleId,
                    ////    //TitleGroupId = (int)account.TitleGroupId,

                    ////    Title = account.Title != null ? account.Title : null,
                    ////    TitleGroup = account.TitleGroup != null ? account.TitleGroup : null,

                    ////    CreateDate = account.CreateDate,
                    ////    CreateUser = account.CreateUser
                    ////}



                    ////    ) ;
                }

            }





            return accountmodellist;


        }


        public async Task<List<AccountModel>> GetAccounts(string searchfirstname, string searchlastname)


        {

            var accounts = new List<AccountModel>();
            var allaccounts = new List<Account>();

            if (searchfirstname != "" && searchlastname != "")
            {
                IQueryable<Account> query = _db.Accounts
                                        //  .Include(account => account.Title)
                                        // .Include(account => account.TitleGroup)
                                        .Include(account => account.AccountUnit)
                                        .ThenInclude(accountunit => accountunit.Unit)
                                       ;

                if (searchfirstname != null)
                {
                    query = query.Where(x => x.FirstName.Contains(searchfirstname));
                }

                if (searchlastname != null)
                {
                    query = query.Where(x => x.LastName.Contains(searchlastname));
                }

                query = query.Where(x => x.Source != "STU");

                allaccounts = await query.OrderBy(x => x.LastName).ToListAsync();

                if (allaccounts?.Any() == true)
                {
                    foreach (var account in allaccounts)
                    {

                        accounts.Add(new AccountModel()
                        {
                            SystemId = account.SystemId,
                            FirstName = account.FirstName,
                            MiddleName = account.MiddleName,
                            LastName = account.LastName,
                            SuffixName = account.SuffixName,
                            Nickname = account.Nickname,
                            JobTitle = account.JobTitle,
                            IsActive = account.IsActive,
                            AccountUnit = account.AccountUnit,
                            //TitleId = (int)account.TitleId,
                            //TitleGroupId = (int)account.TitleGroupId,

                            // Title = account.Title != null ? account.Title : null,
                            // TitleGroup = account.TitleGroup != null ? account.TitleGroup : null
                        }



                            );
                    }

                }

            }



            return accounts;


        }







        // HTTP GET for  Edit page 
        public async Task<AccountModel> GetAccountByID(int id)
        {
            var model = new AccountModel();
            var employeemodel = new EmployeeModel();
            var jobrecordmodel = new JobRecordModel();
            var title = new Title();
            var titlegroup = new TitleGroup();

            var data = await _db.Accounts
                .Include(x => x.Employee)
                .Include(x => x.JobRecords).Where(x => x.JobRecords.Any(x => x.IsPrimary == true))
                //   .Include(x => x.Title)
                //   .Include(x => x.TitleGroup)
                .Where(x => x.SystemId == id)
                .FirstOrDefaultAsync();


            System.DateTime? lastupdate;

            if (data.PrimaryUnitId != null)
            {
                model.PrimaryUnitId = data.PrimaryUnitId;
            }

            //if (data.TitleId != null)
            //{
            //    model.TitleId = (int)data.TitleId;
            //}

            //if (data.TitleGroupId != null)
            //{
            //    model.TitleGroupId = (int)data.TitleGroupId;
            //}




            if (data.LastUpdate.HasValue)
            { lastupdate = (DateTime)data.LastUpdate; }
            else
            {
                lastupdate = null;
            }

            model.SystemId = data.SystemId;
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
            model.LastUpdate = lastupdate;
            model.LastUpdateUser = data.LastUpdateUser;
            //model.TitleId = data.TitleId != null ? (int)data.TitleId : 0;
            //model.TitleGroupId = data.TitleGroupId != null ? (int)data.TitleGroupId : 0;
            model.PrimaryUnitId = data.PrimaryUnitId;
            model.CreateDate = data.CreateDate;
            model.CreateUser = data.CreateUser;

            if (data.EmployeeId != null)
            {
                employeemodel.EmployeeId = data.Employee.EmployeeId;
                employeemodel.FirstName = data.Employee.FirstName;
                employeemodel.MiddleName = data.Employee.MiddleName;
                employeemodel.LastName = data.Employee.LastName;
                employeemodel.SuffixName = data.Employee.SuffixName;
                employeemodel.FsfBuildingId = data.Employee.FsfBuildingId;
                employeemodel.PersonnelCode = data.Employee.PersonnelCode;
                employeemodel.Jobcode = data.Employee.Jobcode;
                employeemodel.UnitId = data.Employee.UnitId;
                employeemodel.UnitIdManual = data.Employee.UnitIdManual;
                employeemodel.JobTitle = data.Employee.JobTitle;
                employeemodel.JobTitleManual = data.Employee.JobTitleManual;
                employeemodel.EndDate = data.Employee.EndDate;
                employeemodel.IsDeleted = data.Employee.IsDeleted;
                employeemodel.LastUpdate = data.Employee.LastUpdate;
                model.Employee = employeemodel;
            }
            else // user doesn't have hr.Employee record yet
            {
                employeemodel.FirstName = model.FirstName;
                employeemodel.MiddleName = model.MiddleName;
                employeemodel.LastName = model.LastName;
                employeemodel.SuffixName = model.SuffixName;
                model.Employee = employeemodel;
            }

            if ((int)data.JobRecords.ElementAt(0).JobRecordId > 0)
            {
                jobrecordmodel.IsPHRST = data.JobRecords.ElementAt(0).IsPHRST;
                jobrecordmodel.IsActive = data.JobRecords.ElementAt(0).IsActive;
                jobrecordmodel.StartDate = data.JobRecords.ElementAt(0).StartDate;
                jobrecordmodel.EndDate = data.JobRecords.ElementAt(0).EndDate;
                jobrecordmodel.JobRecordId = data.JobRecords.ElementAt(0).JobRecordId;
                jobrecordmodel.UnitId = data.JobRecords.ElementAt(0).UnitId;
                jobrecordmodel.BuildingId = data.JobRecords.ElementAt(0).BuildingId;
                jobrecordmodel.SubstituteForId = data.JobRecords.ElementAt(0).SubstituteForId;
                jobrecordmodel.TitleGroupId = data.JobRecords.ElementAt(0).TitleGroupId ?? 0;
                jobrecordmodel.TitleId = data.JobRecords.ElementAt(0).TitleId ?? 0;
                jobrecordmodel.SystemId = (int)data.JobRecords.ElementAt(0).SystemId;
                jobrecordmodel.IsPrimary = data.JobRecords.ElementAt(0).IsPrimary;
                jobrecordmodel.HrWorkLocation = data.JobRecords.ElementAt(0).HrWorkLocation;
                jobrecordmodel.HrRecordNumber = data.JobRecords.ElementAt(0).HrRecordNumber;
                jobrecordmodel.HrPrimary = data.JobRecords.ElementAt(0).HrPrimary;
                jobrecordmodel.HrPersonnelCode = data.JobRecords.ElementAt(0).HrPersonnelCode;
                jobrecordmodel.HrMatched = data.JobRecords.ElementAt(0).HrMatched;
                jobrecordmodel.HrJobCode = data.JobRecords.ElementAt(0).HrJobCode;
                jobrecordmodel.NeedsReview = data.JobRecords.ElementAt(0).NeedsReview;
                jobrecordmodel.CreateDate = data.JobRecords.ElementAt(0).CreateDate;
                jobrecordmodel.CreateUser = data.JobRecords.ElementAt(0).CreateUser;
                jobrecordmodel.LastUpdate = data.JobRecords.ElementAt(0).LastUpdate;
                jobrecordmodel.LastUpdateUser = data.JobRecords.ElementAt(0).LastUpdateUser;

                model.JobRecord = jobrecordmodel;

            }

            if (data.JobRecords.ElementAt(0).Title != null)
            {
                title.TitleId = data.JobRecords.ElementAt(0).Title.TitleId;
                title.Text = data.JobRecords.ElementAt(0).Title.Text;
                model.JobRecord.Title = title;
            }

            if (data.JobRecords.ElementAt(0).TitleGroup != null)
            {
                titlegroup.TitleGroupId = data.JobRecords.ElementAt(0).TitleGroup.TitleGroupId;
                titlegroup.Description = data.JobRecords.ElementAt(0).TitleGroup.Description;
                model.JobRecord.TitleGroup = titlegroup;
            }




            return model;

        }




        public async Task<AccountModel> GetAccountDetailsByID(int id)
        {
            var accountmodel = new AccountModel();
            var employeemodel = new EmployeeModel();
            var studentmodel = new StudentModel();
            var unitmodel = new Unit();
            var usermodel = new UserModel();

            var data = await _db.Accounts
                .Include(x => x.User)
                .Include(x => x.Student)
                .Include(x => x.Employee)
                // .Include(x => x.Contract)
                .Where(x => x.SystemId == id).SingleOrDefaultAsync();







            // Get Unit selections and selected item
            if (data.PrimaryUnitId != null)
            {
                var unitdata = await _db.Units.FindAsync(data.PrimaryUnitId);
                accountmodel.PrimaryUnitId = unitdata.UnitId;
            }
            else
            {
                var unitdata = await _db.Units.OrderBy(x => x.Name).ToListAsync();

            }
            // accountmodel.PrimaryUnit = data.PrimaryUnit;





            accountmodel.SystemId = data.SystemId;
            accountmodel.ADObjectGuid = data.AdobjectGuid;
            accountmodel.FirstName = data.FirstName;
            accountmodel.MiddleName = data.MiddleName;
            accountmodel.LastName = data.LastName;
            accountmodel.SuffixName = data.SuffixName;
            accountmodel.AlternateFirstName = data.AlternateFirstName;
            accountmodel.AlternateLastName = data.AlternateLastName;
            accountmodel.Nickname = data.Nickname;
            //DisplayName = account.DisplayName,
            accountmodel.JobTitle = data.JobTitle;
            accountmodel.Description = data.Description;
            // DisplayJobTitle = account.DisplayJobTitle,
            accountmodel.UsernameOverride = data.UsernameOverride;
            accountmodel.IsActive = data.IsActive;
            accountmodel.Source = data.Source;
            // accountmodel.LastUpdate = lastupdate;
            accountmodel.LastUpdateUser = data.LastUpdateUser;

            if (data.EmployeeId != null)
            {

                employeemodel.EmployeeId = data.Employee.EmployeeId;
                employeemodel.FsfBuildingId = data.Employee.FsfBuildingId;
                employeemodel.PersonnelCode = data.Employee.PersonnelCode;
                employeemodel.Jobcode = data.Employee.Jobcode;
                employeemodel.UnitId = data.Employee.UnitId;
                employeemodel.UnitIdManual = data.Employee.UnitIdManual;
                employeemodel.JobTitle = data.Employee.JobTitle;
                employeemodel.JobTitleManual = data.Employee.JobTitleManual;
                employeemodel.EndDate = data.Employee.EndDate;
                employeemodel.IsDeleted = data.Employee.IsDeleted;
                employeemodel.LastUpdate = data.Employee.LastUpdate;
                accountmodel.Employee = employeemodel;
            }


            if (data.StudentId != null)
            {
                studentmodel.StudentId = data.Student.StudentId;
                studentmodel.FirstName = data.Student.FirstName;
                studentmodel.LastName = data.Student.LastName;
                studentmodel.SisBuildingId = data.Student.SisBuildingId;
                studentmodel.GradeId = data.Student.GradeId;
                studentmodel.CurrentStatus = data.Student.CurrentStatus;
                studentmodel.WithdrawalDate = data.Student.WithdrawalDate;
                studentmodel.GraduationYear = data.Student.GraduationYear;
                studentmodel.StateReportId = data.Student.StateReportId;
                studentmodel.LastModified = data.Student.LastModified;

            }
            accountmodel.Student = studentmodel;

            if (data.AdobjectGuid != null)
            {
                usermodel.ObjectGuid = data.User.ObjectGuid;
                usermodel.SamaccountName = data.User.SamaccountName;
                usermodel.IsDisabled = data.User.IsDisabled;
                usermodel.Mail = data.User.Mail;
                usermodel.Name = data.User.Name;
                usermodel.JobTitle = data.User.JobTitle;
                usermodel.OrganizationalUnitGuid = data.User.OrganizationalUnitGuid;
                usermodel.Department = data.User.Department;
                usermodel.Office = data.User.Office;
                usermodel.AdEmployeeId = data.User.AdEmployeeId;
                usermodel.WhenChanged = data.User.WhenChanged;
                usermodel.WhenCreated = data.User.WhenCreated;
                usermodel.IsDeleted = data.User.IsDeleted;

                accountmodel.User = usermodel;

            }

            return accountmodel;

        }

        public async Task<SelectList> GetUnits()
        {

            var unitdata = new SelectList(await _db.Units.OrderBy(x => x.Name).ToDictionaryAsync(m => m.UnitId.ToString(), m => m.Name), "Key", "Value");

            return unitdata;

        }

        public int JobRecordCount(int systemid)
        {
            return _db.JobRecord.Where(e => e.SystemId == systemid).Count();
        }


        public void EditJobRecord(AccountModel accountModel, string comment, bool expireJobOnly)
        {


            var jobrecordToEdit = _db.JobRecord.Find(accountModel.JobRecord.JobRecordId);

            if (expireJobOnly == true)
            {
                jobrecordToEdit.EndDate = accountModel.JobRecord.EndDate;
                jobrecordToEdit.IsPrimary = accountModel.JobRecord.IsPrimary;
                jobrecordToEdit.IsActive = accountModel.JobRecord.IsActive;
            }
            else
            {
                jobrecordToEdit.IsPHRST = false;
                jobrecordToEdit.StartDate = accountModel.JobRecord.StartDate;
                jobrecordToEdit.EndDate = accountModel.JobRecord.EndDate;
                jobrecordToEdit.IsActive = accountModel.JobRecord.IsActive;
                jobrecordToEdit.IsPrimary = accountModel.JobRecord.IsPrimary;
                jobrecordToEdit.UnitId = accountModel.JobRecord.UnitId;
                jobrecordToEdit.BuildingId = accountModel.JobRecord.BuildingId;
                jobrecordToEdit.SubstituteForId = accountModel.JobRecord.SubstituteForId;
                jobrecordToEdit.TitleGroupId = accountModel.JobRecord.TitleGroupId;
                jobrecordToEdit.TitleId = accountModel.JobRecord.TitleId;
                jobrecordToEdit.HrWorkLocation = accountModel.JobRecord.HrWorkLocation;
                jobrecordToEdit.HrJobCode = accountModel.JobRecord.HrJobCode;
                jobrecordToEdit.HrPersonnelCode = accountModel.JobRecord.HrPersonnelCode;
                jobrecordToEdit.HrPrimary = accountModel.JobRecord.HrPrimary;
            }



            jobrecordToEdit.LastUpdateUser = _appUser.CurrentUser;
            jobrecordToEdit.LastUpdate = DateTime.Now;


            _db.Update(jobrecordToEdit);

            _db.SaveChanges();


            _log.Log((int)ActionTypes.Edit, "dbo.JobRecord", accountModel.JobRecord.JobRecordId.ToString(), comment);

        }




        public int AddJobRecord(JobRecord jobrecord, string comment)
        {



            _db.JobRecord.Add(jobrecord);
            _db.SaveChanges();

            _log.Log((int)ActionTypes.Create, "dbo.JobRecord", jobrecord.JobRecordId.ToString(), comment);

            return jobrecord.JobRecordId;


        }



        public async Task<SelectList> GetBuildings()
        {

            var buildingdata = new SelectList(await _db.Buildings
                // .Where(x => x.Active == true)
                .OrderBy(x => x.Name)
                .ToDictionaryAsync(m => m.BuildingId, m => m.Name), "Key", "Value");

            return buildingdata;

        }


        public async Task<SelectList> GetBuildingsByUnit(int unitid)
        {

            var buildingdata = new SelectList(await _db.UnitBuildings
                 .Include(unitbuildings => unitbuildings.Building)
                 .Include(unitbuildings => unitbuildings.Unit)
                 .Where(x => x.UnitId == unitid)
                 .Where(x => x.Active == true)
                 .OrderBy(x => x.Building.Name)
                .ToDictionaryAsync(m => m.BuildingId, m => m.Building.Name), "Key", "Value");

            return buildingdata;

        }




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



            return employee;
        }




    }
}

