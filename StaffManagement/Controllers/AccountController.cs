using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StaffManagement.Controllers
{
    /// <summary>
    /// Contains "GET" and "POST" action methods such as "Index", "Add", "Edit" referenced in "Views/Account"
    /// </summary>

    [DisplayName("Employees")]
    public class AccountController : BaseController
    {


        private readonly IAccountRepository _accountRepository = null;
        private readonly IJobRecordRepository _jobRecordRepository = null;
        private readonly IUnitRepository _unitRepository = null;


        /// <summary>
        /// constructor for Account Controller.  Get repository objects using dependency injection and access log and http context objects from base class.
        /// </summary>
        /// <param name="accountRepository">dbo.Account database calls and related</param>
        /// <param name="jobRecordRepository">dbo.JobRecord database calls and related</param>
        /// <param name="unitRepository">dbo.Unit database calls</param>
        /// <param name="log">dbo.Auditlog database calls</param>
        /// <param name="contextAccessor">HttpContext object</param>
        /// <param name="protector"></param>
        public AccountController(IAccountRepository accountRepository, IJobRecordRepository jobRecordRepository, IUnitRepository unitRepository, IAuditLoggerService log, IHttpContextAccessor contextAccessor, URLEncrypter protector, AppUser appUser) : base(log, contextAccessor, protector, appUser)
        {
            _accountRepository = accountRepository;
            _jobRecordRepository = jobRecordRepository;
            _unitRepository = unitRepository;


        }



        /// <summary>
        /// GET action method on "Views/Account/Edit.cshtml".  Data to render an employee's account info on Edit page.
        /// </summary>
        /// <param name="id">SystemId for employee</param>
        /// <param name="jobrecordId">Load a particular job record if tab selected</param>
        /// <returns>AccountModel</returns>

        [DisplayName("Edit")]
        public async Task<ViewResult> Edit(int id, int jobRecordId)
        {
            // lookup employee's account
            var model = await _accountRepository.GetAccountByID(id, jobRecordId);

            //save current values of account and job record to session
            _appUser.SaveCurrentAccount(_httpContextAccessor.HttpContext, model);
            _appUser.SaveCurrentJobRecord(_httpContextAccessor.HttpContext, model);

            // loading values for dropdownlists

            model.AccountType.AccountTypes = await GetAccountTypes();
            model.JobRecord.Unit.Units = await GetUnits();
            model.JobRecord.TitleGroup.TitleGroups = await GetTitleGroups();
            model.JobRecord.Title.Titles = await GetTitles(model.JobRecord.TitleGroupId);
            model.JobRecord.Title.AllTitles = await GetTitles(0);
            model.JobRecord.Unit.Building.Buildings = await GetBuildings((model.JobRecord.UnitId == null) ? 0 : (int)model.JobRecord.UnitId);

            if (model.EmployeeId != null)
            {
                model.Employee.EmployeeJob.EmployeeJobs = await GetEmployeeJobs((int)model.EmployeeId);

                // Remove from list if a JobRecord matches the Employee Job
                model.Employee.EmployeeJob.EmployeeJobs = RemoveMatches(model, model.Employee.EmployeeJob.EmployeeJobs);
                if (model.Employee.EmployeeJob.EmployeeJobs.Count() > 0)
                {
                    string[] employeeJob = model.Employee.EmployeeJob.EmployeeJobs.ElementAt(0).Value.Split('#');
                    model.Employee.EmployeeJob.FsfBuildingId = employeeJob[0].ToString();
                    model.Employee.EmployeeJob.PersonnelCode = employeeJob[1].ToString();
                    model.Employee.EmployeeJob.Jobcode = int.Parse(employeeJob[2]);
                    model.Employee.EmployeeJob.UnitId = int.Parse(employeeJob[3]);
                    model.Employee.EmployeeJob.EffectiveDate = ((employeeJob[4] == null) ? null : DateTime.Parse(employeeJob[4].ToString()));
                }


            }

            ViewBag.FsfBuildingData = await GetfsfBuildings();
            ViewBag.JobCodeData = await GetJobCodes();
            ViewBag.PersonnelCodeData = await GetPersonnelCodes();

            ViewBag.JobRecordBuildingCount = model.JobRecord.Unit.Building.Buildings.Count();
            ViewBag.JobRecordCount = _jobRecordRepository.JobRecordCount(model.SystemId);
            ViewBag.IsPHRST = model.JobRecord != null ? model.JobRecord.IsPHRST : null;

            model.AuditLogView = _accountRepository.GetAuditLog(model.SystemId, model.JobRecord.JobRecordId, model.ADObjectGuid == null ? Guid.Empty : (Guid)model.ADObjectGuid, model.Name, model.EmployeeId ?? 0);


            model.Employee.Employees = (Dictionary<Int32, string>)(await _accountRepository.GetEmployees()).Items;
            model.Employee.SelectedEmployeeId = model.JobRecord.SubstituteForId;
            model.Employee.SelectedEmployee = model.Employee.Employees.FirstOrDefault(e => e.Key == (model.JobRecord.SubstituteForId ?? 0)).Value;

            model.Employee.IsSubstitute = (model.JobRecord.SubstituteForId ?? 0) > 0;



            var employeeModel = new EmployeeModel();

            if (ViewBag.SelectedEmployee != null && int.Parse(ViewBag.SelectedEmployee) > 0)
            {
                employeeModel.EmployeeId = int.Parse(ViewBag.SelectedEmployee);
            }



            return View(model);

        }

        /// <summary>
        /// Remove dropdown items where the jobrecord matches the employee job, leaving only mismatches
        /// </summary>
        /// <param name="model"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public SelectList RemoveMatches(AccountModel model, SelectList list)
        {
            SelectList newList;
            List<SelectListItem> newListItems = new List<SelectListItem>();

            // make sure calculated field is current
            bool checkCurrentStatus = model.JobIsCurrentWithPHRST;

            foreach (var item in list)
            {
                // logic to not include matches
                string[] employeeJob = item.Value.Split('#');
                var fsfBuildingId = employeeJob[0].ToString();
                var personnelCode = employeeJob[1].ToString();
                var jobCode = int.Parse(employeeJob[2]);



                foreach (var empJob in model.Employee.EmployeeJobs)
                {
                    if (empJob.IsCurrentWithPHRST == false && empJob.FsfBuildingId == fsfBuildingId && empJob.PersonnelCode == personnelCode && empJob.Jobcode == jobCode)
                    {
                        SelectListItem listItem = new SelectListItem() { Value = item.Value, Text = item.Text };
                        empJob.NeedsReview = true;
                        newListItems.Add(listItem);
                        break;
                    }
                }


                foreach (var empJob in model.Employee.EmployeeJobs)
                {
                    if (empJob.NeedsReview == true)
                    {
                        model.NeedsReviewPersonnelCode = empJob.PersonnelCodes.Description;
                        model.NeedsReviewJobCode = empJob.Jobcodes.Description;
                        model.NeedsReviewBuilding = empJob.Buildings.Description;
                        model.NeedsReviewUnit = empJob.Units.Name;
                        break;
                    }

                }


            }
            newList = new SelectList(newListItems, "Value", "Text", null);

            return newList;

        }

        public async Task<SelectList> GetUnits()
        {
            var unitData = await _accountRepository.GetUnitsAsync();
            return unitData;
        }

        public async Task<SelectList> GetTitles(int titleGroupId)
        {
            SelectList titleData;

            if (titleGroupId > 0)
            { titleData = await _accountRepository.GetTitlesByTitleGroup(titleGroupId); }
            else
            {
                titleData = await _accountRepository.GetTitles();
            }
            return titleData;
        }

        public async Task<SelectList> GetTitleGroups()
        {
            var titleGroupData = await _accountRepository.GetTitleGroups();
            return titleGroupData;
        }

        public async Task<SelectList> GetAccountTypes()
        {
            var accountTypeData = await _accountRepository.GetAccountTypes();
            return accountTypeData;
        }


        public async Task<SelectList> GetBuildings(int primaryUnitId)
        {
            SelectList buildingData;

            if (primaryUnitId > 0)
            {
                buildingData = await _accountRepository.GetBuildingsByUnit(primaryUnitId);
            }
            else
            {
                buildingData = await _accountRepository.GetBuildings();
            }
            return buildingData;
        }


        public async Task<SelectList> GetJobCodes()
        {
            var jobCodeData = await _accountRepository.GetJobCodes();
            return jobCodeData;
        }

        public async Task<SelectList> GetPersonnelCodes()
        {
            var personnelCodeData = await _accountRepository.GetPersonnelCodes();
            return personnelCodeData;
        }

        public async Task<SelectList> GetfsfBuildings()
        {
            var fsfBuildingData = await _accountRepository.GetfsfBuildings();
            return fsfBuildingData;
        }


        public async Task<SelectList> GetEmployeeJobs(int employeeId)
        {
            SelectList employeeJobData;


            employeeJobData = await _accountRepository.GetEmployeeJobsSelectList(employeeId);

            return employeeJobData;
        }



        /// <summary>
        /// GET action method on "Views/Account/Add.cshtml".  Data to render and keep track of values for "new employee" entry page.
        /// </summary>
        /// <param name="fromController">When redirected from another page</param>
        /// <param name="linkedemployee">employeeid that is being substituted for</param>
        /// <param name="linkedemployeeInput">substitute's name</param>
        /// <param name="fromAction">When redirected from another page</param>
        /// <param name="AccountSearchFirstName"></param>
        /// <param name="AccountSearchLastName"></param>
        /// <param name="titlegroupid"></param>
        /// <param name="chkAlt">Has Alternate first or last name</param>
        /// <param name="chkContinue">Continue after confirming employee is not a duplicate</param>
        /// <param name="chkUsername">Has an overridden username</param>
        ///  /// <param name="chkNickname">Has an overridden nickname</param>
        /// <param name="IsPHRST"></param>
        /// <param name="primaryunitid"></param>
        /// <param name="id">EmployeeId passed in from another page</param>
        /// <returns>AccountModel</returns>

        [DisplayName("New")]
        // GET: AccountController/Add
        public async Task<ViewResult> Add(string fromController, int? linkedEmployee, string linkedEmployeeInput, string fromAction, string accountSearchFirstName, string accountSearchLastName, int titleGroupId, string chkAlt, string chkContinue, string chkUsername, string chkNickname, bool? IsPHRST, int primaryUnitId, string id)
        {

            int employeeId;
            string derivedAccountSearchFirstName = "";
            string derivedAccountSearchLastName = "";
            var accountModel = new AccountModel();

            // Get employee info to pre-fill form if redirecting from link
            if (id != null)
            {
                // employeeid = int.Parse(_protector.Decode(id));
                employeeId = int.Parse(_appUser.GetSavedEncryptedValue(_httpContextAccessor.HttpContext, id));
                var employeeData = await _accountRepository.GetEmployeePHRSTdata(employeeId);
                if (employeeData != null)
                {
                    accountModel.EmployeeId = employeeId;
                    accountModel.EditFirstName = employeeData.FirstName;
                    accountModel.EditMiddleName = employeeData.MiddleName;
                    accountModel.EditLastName = employeeData.LastName;
                    accountModel.EditSuffixName = employeeData.SuffixName;
                    accountModel.JobRecord.UnitId = employeeData.EmployeeJob.UnitId;
                    accountModel.JobRecord.HrJobCode = employeeData.EmployeeJob.Jobcode;
                    accountModel.JobRecord.HrPersonnelCode = employeeData.EmployeeJob.PersonnelCode;
                    accountModel.JobRecord.HrWorkLocation = employeeData.EmployeeJob.FsfBuildingId;
                    accountModel.JobRecord.IsPrimary = employeeData.EmployeeJob.IsPrimary;
                    accountModel.Employee.EmployeeId = employeeId;
                    accountModel.Employee.JobTitle = employeeData.JobTitle;

                    ViewBag.Continue = "true";
                    derivedAccountSearchFirstName = accountModel.FirstName;
                    derivedAccountSearchLastName = accountModel.LastName;
                }
                _appUser.SaveNavigationPath(_httpContextAccessor.HttpContext, fromController, fromAction);

            }
            else
            {
                derivedAccountSearchFirstName = accountSearchFirstName;
                derivedAccountSearchLastName = accountSearchLastName;
                _appUser.SaveNavigationPath(_httpContextAccessor.HttpContext, "Account", "Index");

            }



            if (derivedAccountSearchFirstName != null)

            {
                derivedAccountSearchFirstName = derivedAccountSearchFirstName.TrimEnd();
                TempData["AccountSearchFirstName"] = derivedAccountSearchFirstName;


            }

            if (derivedAccountSearchLastName != null)

            {
                derivedAccountSearchLastName = derivedAccountSearchLastName.TrimEnd();
                TempData["AccountSearchLastName"] = derivedAccountSearchLastName;


            }

            // load form dropdowns
            accountModel.AccountType.AccountTypes = await GetAccountTypes();
            accountModel.JobRecord.Unit.Units = _accountRepository.GetUnits();
            accountModel.JobRecord.TitleGroup.TitleGroups = await _accountRepository.GetTitleGroups();
            accountModel.JobRecord.Title.Titles = await GetTitles(titleGroupId);
            accountModel.JobRecord.Unit.Building.Buildings = await GetBuildings(primaryUnitId);
            accountModel.Employee.Employees = (Dictionary<Int32, string>)(await _accountRepository.GetEmployees()).Items;
            accountModel.Employee.SelectedEmployeeId = linkedEmployeeInput != null ? linkedEmployee : null;
            accountModel.Employee.SelectedEmployee = linkedEmployeeInput;
            accountModel.Employee.IsSubstitute = Convert.ToBoolean(Request.Query["Employee.IsSubstitute"]);

            ViewBag.FsfBuildingData = await GetfsfBuildings();
            ViewBag.JobCodeData = await GetJobCodes();
            ViewBag.PersonnelCodeData = await GetPersonnelCodes();

            if (titleGroupId != 0)
            {
                ViewBag.Continue = "true";

            }


            //detect whether checkboxes are selected

            if (chkAlt == "on")
            { ViewBag.chkAlt = "true"; }


            if (chkContinue == "on")
            { ViewBag.Continue = "true"; }


            if (chkUsername == "on")
            { ViewBag.chkUsername = "true"; }

            if (chkNickname == "on")
            { ViewBag.chkNickname = "true"; }


            if (IsPHRST != null)
            { ViewBag.IsPHRST = IsPHRST; }


            ViewBag.BuildingCount = accountModel.JobRecord.Unit.Building.Buildings.Count();
            accountModel.JobRecord.IsPHRST = ViewBag.IsPHRST;
            ViewData["JobRecord"] = accountModel.JobRecord;

            return View(accountModel);
        }

        /// <summary>
        ///  POST action method on "Views/Account/Add.cshtml".  Keeps track of user's inputted values on POST for "new employee" entry page and adds dbo.Account record 
        ///  upon validated submission of the form.
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="linkedEmployee"></param>
        /// <param name="linkedEmployeeInput"></param>
        /// <param name="AccountSearchFirstName"></param>
        /// <param name="AccountSearchLastName"></param>
        /// <param name="chkAlt"></param>
        /// <param name="chkContinue"></param>
        /// <param name="chkUsername"></param>
        /// <param name="chkNickname"></param>
        /// <param name="command"></param>
        /// <param name="IsPHRST"></param>
        /// <returns></returns>

        [DisplayName("New")]
        // POST: AccountController/Add
        [HttpPost]
        public async Task<ActionResult> Add(AccountModel accountModel, int? linkedEmployee, string linkedEmployeeInput, string accountSearchFirstName, string accountSearchLastName, string chkAlt, string chkContinue, string chkUsername, string chkNickname, string command, bool? IsPHRST, string submitAction, string fromController, string fromAction)
        {

            TempData["AccountSearchFirstName"] = accountSearchFirstName;
            TempData["AccountSearchLastName"] = accountSearchLastName;

            //load dropdown values into object model
            accountModel.AccountType.AccountTypes = await GetAccountTypes();
            accountModel.JobRecord.Unit.Units = _accountRepository.GetUnits();
            accountModel.JobRecord.TitleGroup.TitleGroups = await _accountRepository.GetTitleGroups();
            accountModel.JobRecord.Title.Titles = await GetTitles(accountModel.JobRecord.TitleGroupId);
            accountModel.JobRecord.Unit.Building.Buildings = await GetBuildings((accountModel.JobRecord.UnitId == null) ? 0 : (int)accountModel.JobRecord.UnitId);

            accountModel.Employee.Employees = (Dictionary<Int32, string>)(await _accountRepository.GetEmployees()).Items;
            accountModel.Employee.SelectedEmployeeId = linkedEmployeeInput != null ? linkedEmployee : null;
            accountModel.Employee.SelectedEmployee = linkedEmployeeInput;
            accountModel.Employee.IsSubstitute = Convert.ToBoolean(Request.Form["Employee.IsSubstitute"]);

            if (submitAction == "refresh")
            { _accountRepository.RefreshTitles(); }




            if (submitAction == "Add Employee And Continue")
            {
                fromAction = "Edit";
                fromController = "Account";
                _appUser.FromAction = "Edit";
                _appUser.FromController = "Account";
            }
            _appUser.SaveNavigationPath(_httpContextAccessor.HttpContext, fromController, fromAction);




            // if add button is selected and form successfully validates
            if ((ModelState.IsValid) && ((submitAction == "Add Employee") || (submitAction == "Add Employee And Continue")))
            {
                var account = new Account();
                var jobRecord = new JobRecord();


                account.EmployeeId = accountModel.EmployeeId;
                account.FirstName = accountModel.EditFirstName;
                account.MiddleName = accountModel.EditMiddleName;
                account.LastName = accountModel.EditLastName;
                account.SuffixName = accountModel.EditSuffixName;
                account.JobTitle = _accountRepository.GetTitlebyTitleId(accountModel.JobRecord.TitleId);
                account.Nickname = accountModel.Nickname;
                account.AlternateFirstName = accountModel.AlternateFirstName;
                account.AlternateLastName = accountModel.AlternateLastName;
                account.UsernameOverride = accountModel.UsernameOverride;
                account.AccountTypeId = accountModel.AccountTypeId;
                account.UsernameOverride = accountModel.UsernameOverride;
                account.IsActive = true;
                account.LastUpdate = null;
                account.LastUpdateUser = null;
                account.PrimaryUnitId = accountModel.JobRecord.UnitId;
                account.Source = accountModel.Source;
                account.CreateDate = DateTime.Now;
                account.CreateUser = _appUser.CurrentUser;
                account.LastUpdate = DateTime.Now;
                account.LastUpdateUser = _appUser.CurrentUser;
                string comment = "";

                int systemId = _accountRepository.AddAccount(account, AccountFieldChanges(accountModel, comment));

                if (systemId != 0)
                {
                    account.SystemId = systemId;
                    jobRecord.IsPHRST = false;
                    jobRecord.SystemId = account.SystemId;
                    jobRecord.UnitId = account.PrimaryUnitId;
                    jobRecord.BuildingId = accountModel.JobRecord.BuildingId;
                    jobRecord.SubstituteForId = linkedEmployee;
                    jobRecord.TitleGroupId = accountModel.JobRecord.TitleGroupId;
                    jobRecord.TitleId = accountModel.JobRecord.TitleId;
                    jobRecord.StartDate = accountModel.JobRecord.StartDate;
                    jobRecord.EndDate = accountModel.JobRecord.EndDate;
                    jobRecord.CreateDate = DateTime.Now;
                    jobRecord.CreateUser = _appUser.CurrentUser;

                    jobRecord.HrWorkLocation = accountModel.JobRecord.HrWorkLocation;
                    jobRecord.HrPersonnelCode = accountModel.JobRecord.HrPersonnelCode;
                    jobRecord.HrJobCode = accountModel.JobRecord.HrJobCode;
                    jobRecord.IsPrimary = accountModel.JobRecord.IsPrimary;
                    jobRecord.IsActive = true;

                    comment = "";


                    int jobRecordId = _accountRepository.AddJobRecord(jobRecord, JobRecordFieldChanges(accountModel, comment, false));


                    if (jobRecordId > 0)
                    {

                        TempData["ResultMessage"] = account.FirstName + " " + account.LastName + " (" + account.SystemId + ") successfully added.";


                        //if (submitAction == "Add Employee And Continue")
                        //{ return RedirectToAction(nameof(Edit)); }
                        //else
                        //{
                        return RedirectToAction(_appUser.FromAction, _appUser.FromController, new { id = systemId });
                        //}







                    }


                }


            }



            // validation not successful or form reload, retain values

            ViewBag.FsfBuildingData = await GetfsfBuildings();
            ViewBag.JobCodeData = await GetJobCodes();
            ViewBag.PersonnelCodeData = await GetPersonnelCodes();


            if (accountModel.AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub)

            {
                accountModel.Employee.IsSubstitute = true;
            }


            ViewBag.Continue = "true";
            ViewBag.BuildingCount = accountModel.JobRecord.Unit.Building.Buildings.Count();

            if (chkAlt == "on")
            { ViewBag.chkAlt = "true"; }


            if (chkContinue == "on")
            { ViewBag.Continue = "true"; }

            if (chkUsername == "on")
            { ViewBag.chkUsername = "true"; }

            if (chkNickname == "on")
            { ViewBag.chkNickname = "true"; }


            if (command == "changelayout")
            {
                ViewBag.DisableValidation = "true";
                ModelState.Clear();

            }


            if (accountModel.JobRecord.IsPHRST != null)
            { ViewBag.IsPHRST = accountModel.JobRecord.IsPHRST; }


            return View(accountModel);
        }





        /// <summary>
        ///  POST action method on "Views/Account/Edit.cshtml".  Keeps track of values on POST for "edit employee" page. 
        ///  Edits dbo.Account record and Add/Edits dbo.JobRecord as needed depending on what values are changed.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountModel"></param>
        /// <param name="submitAction">Used to distinguish a save attempt from other form posts</param>
        /// <param name="selectedEmployee">When matching to existing employee record</param>
        /// <param name="command">used to disable form validation when posting dropdown values</param>
        /// <param name="linkedEmployee">id of employee substituting for</param>
        /// <param name="linkedEmployeeInput">name of substitute</param>
        /// <param name="jobRecordId">selected jobrecord being viewed/edited</param>
        /// <returns></returns>

        [DisplayName("Edit")]
        // form post from Account edit page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AccountModel accountModel, string submitAction, string selectedEmployee, string command, int? linkedEmployee, string linkedEmployeeInput, int jobRecordId)
        {

            bool newJobRecord = false;
            string resultMessage = "";


            if (accountModel.Employee.EmployeeJob.SelectedEmployeeJob != null)
            {
                string[] employeeJob = accountModel.Employee.EmployeeJob.SelectedEmployeeJob.Split('#');
                accountModel.JobRecord.HrWorkLocation = employeeJob[0].ToString();
                accountModel.JobRecord.HrPersonnelCode = employeeJob[1].ToString();
                accountModel.JobRecord.HrJobCode = int.Parse(employeeJob[2]);
                accountModel.JobRecord.UnitId = int.Parse(employeeJob[3]);
                accountModel.JobRecord.StartDate = ((employeeJob[4] == null) ? null : DateTime.Parse(employeeJob[4].ToString()));
            }




            // load look-up data from cache here  so that it is available for auditlog tracking
            accountModel.AccountType.AccountTypes = await GetAccountTypes();
            accountModel.Employee.Employees = (Dictionary<Int32, string>)(await _accountRepository.GetEmployees()).Items;
            accountModel.JobRecord.Unit.Units = await GetUnits();
            accountModel.JobRecord.TitleGroup.TitleGroups = await GetTitleGroups();
            accountModel.JobRecord.Title.Titles = await GetTitles(accountModel.JobRecord.TitleGroupId);
            accountModel.JobRecord.Title.AllTitles = await GetTitles(0);
            accountModel.JobRecord.Unit.Building.Buildings = await GetBuildings((accountModel.JobRecord.UnitId == null) ? 0 : (int)accountModel.JobRecord.UnitId);

            if (accountModel.EmployeeId != null)
            {
                accountModel.Employee.EmployeeJobs = await _accountRepository.GetEmployeeJobs((int)accountModel.EmployeeId);
            }
            accountModel.JobRecords = await _accountRepository.GetJobRecordsByAccount(accountModel.SystemId);




            if ((linkedEmployee ?? 0) > 0)
            {
                accountModel.JobRecord.SubstituteForId = linkedEmployee;
            }

            //check type of submit button 
            if (submitAction == "Select Employee")
            {
                ViewBag.SelectedEmployee = selectedEmployee;

            }


            if (ModelState.IsValid && (submitAction == "Confirm Name Change"))
            {
                var account = new Account()
                {
                    SystemId = accountModel.SystemId,
                    AccountTypeId = accountModel.AccountTypeId,
                    EmployeeId = accountModel.EmployeeId,
                    AdobjectGuid = accountModel.ADObjectGuid,
                    FirstName = accountModel.Employee.FirstName,
                    MiddleName = accountModel.Employee.MiddleName,
                    LastName = accountModel.Employee.LastName,
                    SuffixName = accountModel.Employee.SuffixName,
                    AlternateFirstName = accountModel.AlternateFirstName,
                    AlternateLastName = accountModel.AlternateLastName,
                    Nickname = accountModel.Nickname,

                    JobTitle = accountModel.JobTitle ?? "",
                    PrimaryUnitId = accountModel.JobRecord.UnitId,
                    UsernameOverride = accountModel.UsernameOverride,
                    // IsActive = accountModel.IsActive,
                    // Source = accountModel.Source,
                    LastUpdateUser = _appUser.CurrentUser,
                    LastUpdate = DateTime.Now,
                    CreateDate = accountModel.CreateDate,
                    CreateUser = accountModel.CreateUser
                };
                _accountRepository.EditAccount(account, "", true);
                resultMessage = resultMessage + "Name change confirmed successfully." + "<br />";

                accountModel.EditFirstName = accountModel.Employee.FirstName;
                accountModel.EditMiddleName = accountModel.Employee.MiddleName;
                accountModel.EditLastName = accountModel.Employee.LastName;
                accountModel.EditSuffixName = accountModel.Employee.SuffixName;

                accountModel.FirstName = accountModel.Employee.FirstName;
                accountModel.MiddleName = accountModel.Employee.MiddleName;
                accountModel.LastName = accountModel.Employee.LastName;
                accountModel.SuffixName = accountModel.Employee.SuffixName;



                // return RedirectToAction(nameof(Edit));
            }


            // if save button submitted and passed validation, save appropriate data
            if (ModelState.IsValid && (submitAction == "Save" || submitAction == "Save And Continue"))
            {
                //check if jobrecord fields changed
                if (JobRecordChanged(accountModel) || accountModel.JobRecord.JobRecordId == -1)
                {

                    // Check if JobRecord needs to be created or changed

                    if (accountModel.JobRecord.JobRecordId > 0 && NeedsNewJobRecord(accountModel))
                    {

                        // update and set jobrecord to 0 so it creates a new one

                        string comment = "";

                        comment = "for [JobRecordId]: [" + accountModel.JobRecord.JobRecordId.ToString() + "] ";

                        //Retain current values set for EndDate, IsActive, and IsPrimary
                        DateTime? tmpEndDate = accountModel.JobRecord.EndDate;
                        bool tmpIsActive = accountModel.JobRecord.IsActive;
                        bool tmpIsPrimary = accountModel.JobRecord.IsPrimary;

                        accountModel.JobRecord.EndDate = accountModel.JobRecord.StartDate;  //Set EndDate for expiring jobrecord to current startdate
                        accountModel.JobRecord.IsActive = false;    // Set Expiring jobrecord as inactive
                        accountModel.JobRecord.IsPrimary = false;   // Remove primary status of expiring jobrecord

                        _jobRecordRepository.EditJobRecord(accountModel, JobRecordFieldChanges(accountModel, comment, true), true);

                        resultMessage = resultMessage + "Job record updated successfully." + "<br />";


                        // Set Selected current job record to retained values and JobRecordId of 0 (new)
                        accountModel.JobRecord.JobRecordId = 0;
                        //accountModel.JobRecord.StartDate = DateTime.Now;
                        accountModel.JobRecord.EndDate = tmpEndDate;
                        accountModel.JobRecord.IsActive = tmpIsActive;
                        accountModel.JobRecord.IsPrimary = tmpIsPrimary;
                    }


                    // If JobRecordId > 0 then update jobrecord
                    switch (accountModel.JobRecord.JobRecordId > 0)
                    {
                        case true:
                            try
                            {
                                // Update job record


                                string comment = "";
                                comment = "for [JobRecordId]: [" + accountModel.JobRecord.JobRecordId.ToString() + "] ";

                                _jobRecordRepository.EditJobRecord(accountModel, JobRecordFieldChanges(accountModel, comment, false), false);

                                resultMessage = resultMessage + "Job record updated successfully." + "<br />";



                            }
                            catch (Exception ex)
                            {
                                // log error
                                _log.Log((int)ActionTypes.Edit, "dbo.JobRecord", id.ToString(), ex.Message.ToString());
                                throw;
                            }

                            break;

                        case false:  // JobRecordId = 0, create new jobrecord

                            try
                            {

                                //create job record 

                                var jobRecord = new JobRecord();

                                jobRecord.IsPHRST = accountModel.JobRecord.IsPHRST != null ? (bool)accountModel.JobRecord.IsPHRST : true;
                                jobRecord.StartDate = accountModel.JobRecord.StartDate;
                                jobRecord.EndDate = accountModel.JobRecord.EndDate;
                                jobRecord.IsActive = true;
                                jobRecord.IsPrimary = accountModel.JobRecord.IsPrimary;
                                jobRecord.UnitId = accountModel.JobRecord.UnitId;
                                jobRecord.BuildingId = accountModel.JobRecord.BuildingId;
                                jobRecord.SubstituteForId = linkedEmployee;
                                jobRecord.TitleGroupId = accountModel.JobRecord.TitleGroupId;
                                jobRecord.TitleId = accountModel.JobRecord.TitleId;
                                jobRecord.HrWorkLocation = accountModel.JobRecord.HrWorkLocation;
                                jobRecord.HrRecordNumber = 0;
                                jobRecord.HrJobCode = accountModel.JobRecord.HrJobCode;
                                jobRecord.HrPersonnelCode = accountModel.JobRecord.HrPersonnelCode;
                                jobRecord.HrPrimary = accountModel.JobRecord.HrPrimary;
                                jobRecord.SystemId = accountModel.SystemId;
                                jobRecord.HrMatched = true;
                                jobRecord.NeedsReview = false;
                                jobRecord.CreateDate = DateTime.Now;
                                jobRecord.CreateUser = _appUser.CurrentUser;
                                jobRecord.LastUpdate = DateTime.Now;
                                jobRecord.LastUpdateUser = _appUser.CurrentUser;
                                string comment = "";


                                var newJobRecordId = _jobRecordRepository.AddJobRecord(jobRecord, JobRecordFieldChanges(accountModel, comment, false));



                            }
                            catch (Exception ex)
                            {
                                // log error
                                _log.Log((int)ActionTypes.Create, "dbo.JobRecord", id.ToString(), ex.Message.ToString());
                                throw;
                            }

                            newJobRecord = true;
                            resultMessage = resultMessage + "New Job record created successfully." + "<br />";


                            break;
                    }







                }


                //check if account fields changed
                if (AccountChanged(accountModel))
                {


                    try
                    {
                        // update account instead of adding  


                        var account = new Account()
                        {
                            SystemId = accountModel.SystemId,
                            AccountTypeId = accountModel.AccountTypeId,
                            EmployeeId = accountModel.EmployeeId,
                            AdobjectGuid = accountModel.ADObjectGuid,
                            FirstName = _appUser.EditAccountFirstName,
                            MiddleName = _appUser.EditAccountMiddleName,
                            LastName = _appUser.EditAccountLastName,
                            SuffixName = _appUser.EditAccountSuffixName,
                            AlternateFirstName = accountModel.AlternateFirstName,
                            AlternateLastName = accountModel.AlternateLastName,
                            Nickname = accountModel.Nickname,

                            JobTitle = accountModel.JobTitle ?? "",
                            PrimaryUnitId = accountModel.JobRecord.UnitId,
                            UsernameOverride = accountModel.UsernameOverride,
                            //  IsActive = accountModel.IsActive,
                            // Source = accountModel.Source,
                            LastUpdateUser = _appUser.CurrentUser,
                            LastUpdate = DateTime.Now,
                            CreateDate = accountModel.CreateDate,
                            CreateUser = accountModel.CreateUser
                        };

                        string comment = "";
                        comment = "for [AccountId]: [" + accountModel.SystemId.ToString() + "] ";


                        _accountRepository.EditAccount(account, AccountFieldChanges(accountModel, comment), false);



                        resultMessage = resultMessage + "Account changes updated successfully.";

                        TempData["ResultMessage"] = resultMessage;

                        // if "Save and Continue", redirect to Edit page
                        if (submitAction == "Save And Continue")
                        { return RedirectToAction(nameof(Edit)); }
                        else
                        {  // if "Save" , go to Search page  
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    catch (Exception ex)
                    {
                        // log error
                        _log.Log((int)ActionTypes.Edit, "dbo.Account", id.ToString(), ex.Message.ToString());
                        throw;
                    }



                }
                else
                {

                    if (resultMessage == "")
                    {
                        TempData["ResultMessage"] = "No changes detected.";
                    }
                    else
                    {

                        TempData["ResultMessage"] = resultMessage;



                        if (newJobRecord || submitAction == "Save And Continue")
                        {
                            // Stay on edit page due to a new jobrecord created or user selected "Save and Continue"
                            return RedirectToAction(nameof(Edit));
                        }
                        else
                        {
                            // Redirect to search
                            return RedirectToAction(nameof(Index));
                        }

                    }
                }
            }

            // validation not successful and/or form is reloaded, retain all values

            ViewBag.FsfBuildingdata = await GetfsfBuildings();
            ViewBag.JobCodedata = await GetJobCodes();
            ViewBag.PersonnelCodedata = await GetPersonnelCodes();


            ViewBag.JobRecordBuildingCount = accountModel.JobRecord.Unit.Building.Buildings.Count();
            ViewBag.JobRecordCount = _jobRecordRepository.JobRecordCount(accountModel.SystemId);
            ViewBag.IsPHRST = accountModel.JobRecord != null ? accountModel.JobRecord.IsPHRST : null;
            //ViewBag.Employeedata = accountModel.Employee;

            //var employeeModel = new EmployeeModel();


            accountModel.AuditLogView = _accountRepository.GetAuditLog(accountModel.SystemId, accountModel.JobRecord.JobRecordId, accountModel.ADObjectGuid == null ? Guid.Empty : (Guid)accountModel.ADObjectGuid, accountModel.Name, accountModel.EmployeeId ?? 0);

            accountModel.Employee.Employees = (Dictionary<Int32, string>)(await _accountRepository.GetEmployees()).Items;
            accountModel.Employee.SelectedEmployeeId = accountModel.JobRecord.SubstituteForId;

            if (accountModel.EmployeeId != null)
            {
                accountModel.Employee.EmployeeJob.EmployeeJobs = await GetEmployeeJobs((int)accountModel.EmployeeId);

                accountModel.Employee.EmployeeJob.EmployeeJobs = RemoveMatches(accountModel, accountModel.Employee.EmployeeJob.EmployeeJobs);
                if (accountModel.Employee.EmployeeJob.EmployeeJobs.Count() > 0)
                {
                    string[] employeeJob = accountModel.Employee.EmployeeJob.EmployeeJobs.ElementAt(0).Value.Split('#');
                    accountModel.Employee.EmployeeJob.FsfBuildingId = employeeJob[0].ToString();
                    accountModel.Employee.EmployeeJob.PersonnelCode = employeeJob[1].ToString();
                    accountModel.Employee.EmployeeJob.Jobcode = int.Parse(employeeJob[2]);
                    accountModel.Employee.EmployeeJob.UnitId = int.Parse(employeeJob[3]);
                    accountModel.Employee.EmployeeJob.EffectiveDate = ((employeeJob[4] == null) ? null : DateTime.Parse(employeeJob[4].ToString()));
                }


            }


            if (linkedEmployeeInput != "")
            {
                accountModel.Employee.SelectedEmployee = linkedEmployeeInput;
            }
            else
            {
                accountModel.Employee.SelectedEmployee = accountModel.Employee.Employees.FirstOrDefault(e => e.Key == (accountModel.JobRecord.SubstituteForId ?? 0)).Value;

            }


            if (Convert.ToBoolean(Request.Query["Employee.IsSubstitute"]) || (accountModel.JobRecord.SubstituteForId ?? 0) > 0 || accountModel.AccountTypeId == (int)EnumAccountTypeDescription.LongTermSub)

            {
                accountModel.Employee.IsSubstitute = true;
            }





            if (ViewBag.SelectedEmployee != null && int.Parse(ViewBag.SelectedEmployee) > 0)
            {
                accountModel.EmployeeId = int.Parse(ViewBag.SelectedEmployee);
                ViewBag.DisableValidation = "true";
                ModelState.Clear();
            }




            if (command == "changelayout")
            {
                ViewBag.DisableValidation = "true";
                ModelState.Clear();
            }




            return View(accountModel);


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTitles()
        {


            MemoryStream stream = new MemoryStream();
            Request.Body.CopyToAsync(stream);
            stream.Position = 0;
            using StreamReader reader = new StreamReader(stream);
            int titleGroupId = Int32.Parse(reader.ReadToEnd());

            if (titleGroupId > 0)
            {

                IEnumerable<SelectListItem> titles = (GetTitles(titleGroupId).Result)
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = i.Text.ToString(),
                                               Value = i.Value.ToString()
                                           });


                return new JsonResult(titles);
            }
            return null;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBuildings()
        {


            MemoryStream stream = new MemoryStream();
            Request.Body.CopyToAsync(stream);
            stream.Position = 0;
            using StreamReader reader = new StreamReader(stream);
            int unitId = Int32.Parse(reader.ReadToEnd());

            if (unitId > 0)
            {

                IEnumerable<SelectListItem> buildings = (GetBuildings(unitId).Result)
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = i.Text.ToString(),
                                               Value = i.Value.ToString()
                                           });


                return new JsonResult(buildings);
            }
            return null;
        }

        public bool NeedsNewJobRecord(AccountModel accountModel)
        {

            bool needsNewJobRecord = false;

            if (    // there is any change to job record fields that define a job
                (
               (_appUser.EditAccountHrWorkLocation != accountModel.JobRecord.HrWorkLocation && _appUser.EditAccountHrWorkLocation != null) ||
               (_appUser.EditAccountHrPersonnelCode != accountModel.JobRecord.HrPersonnelCode && _appUser.EditAccountHrPersonnelCode != null) ||
               (_appUser.EditAccountHrJobCode != ((int)(accountModel.JobRecord.HrJobCode ?? 0)).ToString() && _appUser.EditAccountHrJobCode != "0") ||
                (_appUser.EditAccountTitleId != accountModel.JobRecord.TitleId && _appUser.EditAccountTitleId != 0) ||
               (_appUser.EditAccountBuildingId != accountModel.JobRecord.BuildingId && _appUser.EditAccountBuildingId != 0)
                )

                &&
                //  the startdate has passed at least a day 
                (DateTime.Now.AddDays(-1) > DateTime.Parse(_appUser.EditAccountStartDate ?? Settings.BeginDate.ToString()))

                )

            {

                needsNewJobRecord = true;

            }




            return needsNewJobRecord;
        }

        /// <summary>
        /// Compare values in AccountModel with Account values saved in session before POST, viewed using AppUser object
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool AccountChanged(AccountModel account)
        {

            _appUser.GetCurrentAccount(_httpContextAccessor.HttpContext);

            if (account.SystemId != _appUser.EditAccountSystemId) return true;
            if ((account.EmployeeId == null ? 0 : account.EmployeeId) != (_appUser.EditAccountEmployeeId == null ? 0 : _appUser.EditAccountEmployeeId)) return true;
            if ((account.AccountTypeId == null ? 0 : account.AccountTypeId) != (_appUser.EditAccountAccountTypeId == null ? 0 : _appUser.EditAccountAccountTypeId)) return true;
            if (account.ADObjectGuid.ToString() != _appUser.EditAccountADObjectGuid) return true;

            if (account.EditFirstName != _appUser.EditAccountFirstName) return true;
            if (account.EditMiddleName != _appUser.EditAccountMiddleName) return true;
            if (account.EditLastName != _appUser.EditAccountLastName) return true;
            if (account.EditSuffixName != _appUser.EditAccountSuffixName) return true;

            if (account.AlternateFirstName != _appUser.EditAccountAlternateFirstName) return true;
            if (account.AlternateLastName != _appUser.EditAccountAlternateLastName) return true;
            if (account.Nickname != _appUser.EditAccountNickname) return true;
            if (account.UsernameOverride != _appUser.EditAccountUsernameOverride) return true;

            //if (account.IsActive.ToString() != _appUser.EditAccountIsActive) return true;
            //if (account.Source != _appUser.EditAccountSource) return true;

            return false;
        }

        /// <summary>
        /// Build string of dbo.Account record field changes by comparing to saved data in session
        /// </summary>
        /// <param name="account"></param>
        /// <param name="comment"></param>
        /// <returns>Comment string </returns>
        public string AccountFieldChanges(AccountModel account, string comment)
        {

            if ((account.AccountTypeId ?? 0) != (_appUser.EditAccountAccountTypeId ?? 0))
                comment += "[" + GetFieldDisplayName(account, "AccountTypeId") + "]"
                + "[" + (account.AccountTypeId ?? 0).ToString() + "]" + " (" + (_appUser.EditAccountAccountTypeId ?? 0).ToString() + ") ";

            if ((account.EmployeeId ?? 0) != (_appUser.EditAccountEmployeeId ?? 0))
                comment += "[" + GetFieldDisplayName(account, "EmployeeId") + "]"
                + "[" + (account.EmployeeId ?? 0).ToString() + "]" + " (" + (_appUser.EditAccountEmployeeId ?? 0).ToString() + ") ";

            if ((account.ADObjectGuid == null ? "" : account.ADObjectGuid.ToString()) != (_appUser.EditAccountADObjectGuid ?? ""))
                comment += "[" + GetFieldDisplayName(account, "ADObjectGuid") + "]"
                + "[" + (account.ADObjectGuid == null ? "" : account.ADObjectGuid.ToString()) + "]" + " (" + _appUser.EditAccountADObjectGuid + ") ";

            if (account.EditFirstName != _appUser.EditAccountFirstName) comment += "[" + GetFieldDisplayName(account, "FirstName") + "]"
                    + "[" + account.EditFirstName.ToString() + "]" + " (" + _appUser.EditAccountFirstName + ") ";

            if (account.EditMiddleName != _appUser.EditAccountMiddleName) comment += "[" + GetFieldDisplayName(account, "MiddleName") + "]"
                  + "[" + account.EditMiddleName.ToString() + "]" + " (" + _appUser.EditAccountLastName + ") ";

            if (account.EditLastName != _appUser.EditAccountLastName) comment += "[" + GetFieldDisplayName(account, "LastName") + "]"
                   + "[" + account.EditLastName.ToString() + "]" + " (" + _appUser.EditAccountLastName + ") ";

            if (account.EditSuffixName != _appUser.EditAccountSuffixName) comment += "[" + GetFieldDisplayName(account, "SuffixName") + "]"
                  + "[" + account.EditSuffixName.ToString() + "]" + " (" + _appUser.EditAccountSuffixName + ") ";

            if (account.AlternateFirstName != _appUser.EditAccountAlternateFirstName) comment += "[" + GetFieldDisplayName(account, "AlternateFirstName") + "]"
                  + "[" + account.AlternateFirstName.ToString() + "]" + " (" + _appUser.EditAccountAlternateFirstName + ") ";

            if (account.Nickname != _appUser.EditAccountNickname) comment += "[" + GetFieldDisplayName(account, "Nickname") + "]"
                  + "[" + account.Nickname.ToString() + "]" + " (" + _appUser.EditAccountNickname + ") ";

            if (account.UsernameOverride != _appUser.EditAccountUsernameOverride) comment += "[" + GetFieldDisplayName(account, "UsernameOverride") + "]"
                 + "[" + account.UsernameOverride.ToString() + "]" + " (" + _appUser.EditAccountUsernameOverride + ") ";

            //if ((account.IsActive == null ? "" : account.IsActive.ToString()) != (_appUser.EditAccountIsActive ?? ""))
            //    comment += "[" + GetFieldDisplayName(account, "IsActive") + "]"
            //    + "[" + (account.IsActive == null ? "" : account.IsActive.ToString()) + "]" + " (" + _appUser.EditAccountIsActive  + ") ";

            //if (account.Source != _appUser.EditAccountSource) comment += "[" + GetFieldDisplayName(account, "Source") + "]"
            //    + "[" + account.Source.ToString() + "]" + " (" + _appUser.EditAccountSource + ") ";

            return comment;
        }

        /// <summary>
        /// Dynamically retrieve display name property in class object
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>

        public string GetFieldDisplayName(object objectName, string fieldName)
        {
            string fieldDisplayName;

            MemberInfo property = objectName.GetType().GetProperty(fieldName);
            fieldDisplayName = property.GetCustomAttribute<DisplayAttribute>()?.Name;

            return fieldName;
        }


        /// <summary>
        /// Build string of dbo.JobRecord record field changes by comparing to saved data in session
        /// </summary>
        /// <param name="account"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public string JobRecordFieldChanges(AccountModel account, string comment, bool expireJobOnly)
        {

            if (expireJobOnly != true)
            {

                if ((Convert.ToString(account.JobRecord.StartDate) ?? "") != (_appUser.EditAccountStartDate ?? "")) comment += "[" + GetFieldDisplayName(account.JobRecord, "StartDate") + "]"
                      + " [" + Convert.ToString(account.JobRecord.StartDate) + "] " + " (" + _appUser.EditAccountStartDate + ") ";


                if ((account.JobRecord.UnitId == null ? 0 : account.JobRecord.UnitId) != (_appUser.EditAccountUnitId == null ? 0 : _appUser.EditAccountUnitId))
                    comment += "[" + GetFieldDisplayName(account.JobRecord, "UnitId") + "]"
                     + " [" + account.JobRecord.Unit.Units.FirstOrDefault(e => e.Value == (Convert.ToString(account.JobRecord.UnitId))).Text
                     + "] " + " (" + (_appUser.EditAccountUnitId != null && _appUser.EditAccountUnitId != 0 ?
                   account.JobRecord.Unit.Units.FirstOrDefault(e => e.Value == _appUser.EditAccountUnitId.ToString()).Text : "") + ") ";

                if ((account.JobRecord.BuildingId == null ? 0 : account.JobRecord.BuildingId) != (_appUser.EditAccountBuildingId == null ? 0 : _appUser.EditAccountBuildingId))
                    comment += "[" + GetFieldDisplayName(account.JobRecord, "BuildingId") + "]"
                       + " [" + account.JobRecord.Unit.Building.Buildings.FirstOrDefault(e => e.Value == (Convert.ToString(account.JobRecord.BuildingId))).Text ?? ""
                       + "] " + " (" + (_appUser.EditAccountBuildingId != null && _appUser.EditAccountBuildingId != 0 ?
                   account.JobRecord.Unit.Building.AllBuildings.FirstOrDefault(e => e.Value == _appUser.EditAccountBuildingId.ToString()).Text ?? "" : "") + ") ";

                if ((account.JobRecord.SubstituteForId ?? 0) != (_appUser.EditAccountSubstituteForId ?? 0)) comment += "[" + GetFieldDisplayName(account.JobRecord, "SubstituteForId") + "]"
                              + " [" + account.Employee.Employees.FirstOrDefault(e => e.Key == (account.JobRecord.SubstituteForId ?? 0)).Value + "] "
                              + " (" + account.Employee.Employees.FirstOrDefault(e => e.Key == (_appUser.EditAccountSubstituteForId ?? 0)).Value + ") ";

                if (account.JobRecord.TitleGroupId != _appUser.EditAccountTitleGroupId) comment += "[" + GetFieldDisplayName(account.JobRecord, "TitleGroupId") + "]"
                           + " [" + account.JobRecord.TitleGroup.TitleGroups.FirstOrDefault(e => e.Value == (Convert.ToString(account.JobRecord.TitleGroupId))).Text + "] "
                           + " (" + (_appUser.EditAccountTitleGroupId != 0 ?
                           account.JobRecord.TitleGroup.TitleGroups.FirstOrDefault(e => e.Value == _appUser.EditAccountTitleGroupId.ToString()).Text : "") + ") ";

                if (account.JobRecord.TitleId != _appUser.EditAccountTitleId) comment += "[" + GetFieldDisplayName(account.JobRecord, "TitleId") + "]"
                         + " [" + account.JobRecord.Title.Titles.FirstOrDefault(e => e.Value == (Convert.ToString(account.JobRecord.TitleId))).Text + "] "
                           + " (" + (_appUser.EditAccountTitleId != 0 ?
                           account.JobRecord.Title.AllTitles.FirstOrDefault(e => e.Value == _appUser.EditAccountTitleId.ToString()).Text : "") + ") ";

                if (account.JobRecord.HrWorkLocation != _appUser.EditAccountHrWorkLocation) comment += "[" + GetFieldDisplayName(account.JobRecord, "HrWorkLocation") + "]"
                     + " [" + (account.JobRecord.HrWorkLocation == null ? "" : account.JobRecord.HrWorkLocation.ToString()) + "] " + " (" + (_appUser.EditAccountHrWorkLocation ?? "") + ") ";

                if (account.JobRecord.HrPersonnelCode != _appUser.EditAccountHrPersonnelCode) comment += "[" + GetFieldDisplayName(account.JobRecord, "HrPersonnelCode") + "]"
                     + " [" + (account.JobRecord.HrPersonnelCode == null ? "" : account.JobRecord.HrPersonnelCode.ToString()) + "] " + " (" + (_appUser.EditAccountHrPersonnelCode ?? "") + ") ";

                if ((account.JobRecord.HrJobCode == null ? "" : account.JobRecord.HrJobCode.ToString()) != (_appUser.EditAccountHrJobCode ?? "")) comment += "[" + GetFieldDisplayName(account.JobRecord, "HrJobCode") + "]"
                     + " [" + (account.JobRecord.HrJobCode == null ? "" : account.JobRecord.HrJobCode.ToString()) + "] " + " (" + (_appUser.EditAccountHrJobCode ?? "") + ") ";
            }

            //always check
            if (account.JobRecord.IsActive.ToString() != _appUser.EditAccountJobRecordIsActive) comment += "[" + GetFieldDisplayName(account.JobRecord, "IsActive") + "]"
                 + " [" + account.JobRecord.IsActive.ToString() + "] " + " (" + _appUser.EditAccountJobRecordIsActive + ") ";
            if (account.JobRecord.IsPrimary.ToString() != _appUser.EditAccountIsPrimary) comment += "[" + GetFieldDisplayName(account.JobRecord, "IsPrimary") + "]"
                   + " [" + account.JobRecord.IsPrimary.ToString() + "] " + " (" + _appUser.EditAccountIsPrimary + ") ";
            if ((Convert.ToString(account.JobRecord.EndDate) ?? "") != (_appUser.EditAccountEndDate ?? "")) comment += "[" + GetFieldDisplayName(account.JobRecord, "EndDate") + "]"
               + " [" + Convert.ToString(account.JobRecord.EndDate) + "] " + " (" + _appUser.EditAccountEndDate + ") ";



            return comment;
        }

        /// <summary>
        /// Compare current jobrecord values in AccountModel with Account current jobrecord values saved in session before POST, viewed using AppUser object
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool JobRecordChanged(AccountModel account)
        {

            _appUser.GetCurrentJobRecord(_httpContextAccessor.HttpContext);

            //if (account.JobRecord.JobRecordId != _appUser.EditAccountJobRecordId) return true;
            // if (account.JobRecord.IsPHRST.ToString() != _appUser.EditAccountIsPHRST) return true;
            if (account.JobRecord.IsActive.ToString() != _appUser.EditAccountJobRecordIsActive) return true;
            if (account.JobRecord.IsPrimary.ToString() != _appUser.EditAccountIsPrimary) return true;
            if (account.JobRecord.StartDate.ToString() != (_appUser.EditAccountStartDate != null ? _appUser.EditAccountStartDate : "")) return true;
            if (account.JobRecord.EndDate.ToString() != (_appUser.EditAccountEndDate != null ? _appUser.EditAccountEndDate : "")) return true;
            if ((account.JobRecord.UnitId == null ? 0 : account.JobRecord.UnitId) != (_appUser.EditAccountUnitId == null ? 0 : _appUser.EditAccountUnitId)) return true;
            if ((account.JobRecord.BuildingId == null ? 0 : account.JobRecord.BuildingId) != (_appUser.EditAccountBuildingId == null ? 0 : _appUser.EditAccountBuildingId)) return true;
            if ((account.JobRecord.SubstituteForId ?? 0) != (_appUser.EditAccountSubstituteForId ?? 0)) return true;
            if (account.JobRecord.TitleGroupId != _appUser.EditAccountTitleGroupId) return true;
            if (account.JobRecord.TitleId != _appUser.EditAccountTitleId) return true;
            if (account.JobRecord.HrWorkLocation != _appUser.EditAccountHrWorkLocation) return true;
            if (account.JobRecord.HrJobCode.ToString() != _appUser.EditAccountHrJobCode) return true;
            if (account.JobRecord.HrPersonnelCode != _appUser.EditAccountHrPersonnelCode) return true;
            if (account.JobRecord.HrPrimary.ToString() != _appUser.EditAccountHrPrimary) return true;


            return false;
        }


        public ActionResult HRMenu()
        {
            return View();
        }


        public ActionResult Close()
        {
            return View();
        }


        /// <summary>
        /// Keep track of values on "AssignEmployee" page and submit values to link a dbo.Account record to 
        /// and hr.employee record on "Views/Account/AssignEmployee.cshtml"
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="submitAction"></param>
        /// <param name="selectedAccount"></param>
        /// <param name="selectedEmployee"></param>
        /// <param name="accountsearchfirstname"></param>
        /// <param name="accountsearchlastname"></param>
        /// <param name="employeesearchfirstname"></param>
        /// <param name="employeesearchlastname"></param>
        /// <returns>AccountModel</returns>
        [DisplayName("Assign Employee")]
        public async Task<ActionResult> AssignEmployee(AccountModel accountModel, string submitAction, int selectedAccount, int selectedEmployee, string accountSearchFirstName, string accountSearchLastName, string employeeSearchFirstName, string employeeSearchLastName)
        {
            if (accountSearchFirstName != null)

            {
                accountSearchFirstName = accountSearchFirstName.TrimEnd();
                ViewData["AccountSearchFirstName"] = accountSearchFirstName;
                ViewData["EmployeeSearchFirstName"] = accountSearchFirstName; //set as default


            }

            if (accountSearchLastName != null)

            {
                accountSearchLastName = accountSearchLastName.TrimEnd();
                ViewData["AccountSearchLastName"] = accountSearchLastName;
                ViewData["EmployeeSearchLastName"] = accountSearchLastName;  //set as default
            }

            if (employeeSearchFirstName != null)
            {
                employeeSearchFirstName = employeeSearchFirstName.TrimEnd();
                ViewData["EmployeeSearchFirstName"] = employeeSearchFirstName;


            }

            if (employeeSearchLastName != null)
            {
                employeeSearchLastName = employeeSearchLastName.TrimEnd();
                ViewData["EmployeeSearchLastName"] = employeeSearchLastName;


            }



            int systemId;
            int employeeId;

            if (submitAction == "Select Account")
            {
                ViewBag.SelectedAccount = selectedAccount;
                systemId = selectedAccount;

            }
            else
            {
                systemId = accountModel.SystemId;
            }

            if (submitAction == "Select Employee")
            {
                ViewBag.SelectedEmployee = selectedEmployee;
                employeeId = selectedEmployee;

            }
            else
            {
                if (accountModel.EmployeeId == 0)
                { employeeId = ViewBag.SelectedEmployee != null ? ViewBag.SelectedEmployee : 0; }
                else
                {
                    employeeId = accountModel.EmployeeId != null ? (int)accountModel.EmployeeId : 0;
                }
            }




            if (ModelState.IsValid && submitAction == "Save")
            {




                // var employee = new Employee();
                var account = new Account();


                account.SystemId = systemId;
                account.EmployeeId = employeeId;

                account.LastUpdateUser = _appUser.CurrentUser;
                account.LastUpdate = DateTime.Now;


                try
                {

                    _accountRepository.EditAccountEmployee(account);



                    _log.Log((int)ActionTypes.Edit, "dbo.Account", systemId.ToString(), "Updating EmployeeId to " + employeeId.ToString());
                    TempData["ResultMessage"] = "Changes saved successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // log error
                    _log.Log((int)ActionTypes.Edit, "dbo.Account", systemId.ToString(), ex.Message.ToString());
                    throw;
                }

            };




            AccountModel model = new AccountModel();

            if (systemId != 0)
            {
                ViewBag.SelectedAccount = systemId;
                model = await _accountRepository.GetAccountByID(systemId, 0);

                if (accountModel.EmployeeId != null && accountModel.EmployeeId != 0)
                {
                    model.EmployeeId = accountModel.EmployeeId;
                }
                if (employeeId != 0)
                {
                    model.EmployeeId = employeeId;
                    ViewBag.SelectedEmployee = employeeId;
                }


            }



            return View(model);
        }





        /// <summary>
        /// GET action method - Load dropdown values for Account search page on "Views/Account/Index.cshtml".  Upon submission of form, 
        /// modify the account search based on the parameters entered/selected
        /// </summary>
        /// <param name="FirstName">search by firstname</param>
        /// <param name="LastName">search by lastname</param>
        /// <param name="Systemid">search by dbo.Account.SystemId </param>
        /// <param name="Unit">search by unit/department</param>
        /// <param name="Title">search by job title</param>
        /// <param name="JobCode">search by PHRST job code</param>
        /// <param name="PersonnelCode">search by PHRST personnel code</param>
        /// <param name="Building">search by building</param>
        /// <param name="submitAction"></param>
        /// <param name="sortOrder">column on grid selected to determine sort order</param>
        /// <param name="page">current page number </param>
        /// <param name="pageSize">current size of paging</param>
        /// <returns></returns>

        [DisplayName("Search")]
        // Index GET method is used for both the initial form load and posting form, as no data is updated.  
        public async Task<ViewResult> Index(string firstName, string lastName, string systemId, int unitId, int titleId, int jobCodeId, string personnelCode, int buildingId, string submitAction, string sortOrder, int? page, int? pageSize)
        {

            int _unitId = 0, _titleId = 0, _jobCodeId = 0, _buildingId = 0;
            string _firstName = "", _lastName = "", _personnelCode = "", _systemId = "";

            // save the current state of toggling between Ascending and Descending in gird column sorts 
            TempData["CurrentSort"] = sortOrder;
            TempData["JobtitleSort"] = (sortOrder == " Jobtitle_Asc_Sort") ? " Jobtitle_Desc_Sort" : " Jobtitle_Asc_Sort";
            TempData["TitleGroupSort"] = (sortOrder == " TitleGroup_Asc_Sort") ? " TitleGroup_Desc_Sort" : " TitleGroup_Asc_Sort";
            TempData["TitleSort"] = (sortOrder == " Title_Asc_Sort") ? " Title_Desc_Sort" : " Title_Asc_Sort";
            TempData["AccountTypeSort"] = (sortOrder == " AccountType_Asc_Sort") ? " AccountType_Desc_Sort" : " AccountType_Asc_Sort";
            TempData["FirstNameSort"] = (sortOrder == " FirstName_Asc_Sort") ? " FirstName_Desc_Sort" : " FirstName_Asc_Sort";
            TempData["LastNameSort"] = (sortOrder == " LastName_Asc_Sort") ? " LastName_Desc_Sort" : " LastName_Asc_Sort";

            // Read submitted search form 
            if (submitAction == "Search")
            {


                _firstName = firstName ?? "";
                _lastName = lastName ?? "";
                _systemId = systemId ?? "";
                _unitId = unitId;
                _titleId = titleId;
                _jobCodeId = jobCodeId;
                _personnelCode = personnelCode ?? "";
                _buildingId = buildingId;

                _appUser.SaveAccountSearch(_httpContextAccessor.HttpContext, _firstName, _lastName, _systemId, _unitId, _titleId, _jobCodeId, _personnelCode, _buildingId);

            }
            else
            {  // Read from saved search parameters

                _firstName = _appUser.AccountSearchFirstName ?? "";
                _lastName = _appUser.AccountSearchLastName ?? "";
                _systemId = _appUser.AccountSearchSystemId ?? "";
                _unitId = _appUser.AccountSearchUnitId != 0 ? _appUser.AccountSearchUnitId : _appUser.SelectedUnitId;
                _titleId = _appUser.AccountSearchTitleId;
                _jobCodeId = _appUser.AccountSearchJobCode;
                _personnelCode = _appUser.AccountSearchPersonnelCode ?? "";
                _buildingId = _appUser.AccountSearchBuildingId;

            }



            if (_firstName != null)
            {
                _firstName = _firstName.TrimEnd();
                TempData["FirstName"] = _firstName;
            }

            if (_lastName != null)
            {
                _lastName = _lastName.TrimEnd();
                TempData["LastName"] = _lastName;
            }


            if (_systemId != null)
            {
                _systemId = _systemId.TrimEnd();
                TempData["SystemId"] = _systemId;
            }


            // save dropdown values in viewbag with correct item selected
            ViewBag.Unitdata = SetSelected(await GetUnits(), _unitId.ToString());
            ViewBag.Titledata = SetSelected(await GetTitles(0), _titleId.ToString());
            ViewBag.Buildingdata = SetSelected(await GetBuildings(0), _buildingId.ToString());
            ViewBag.JobCodedata = SetSelected(await GetJobCodes(), _jobCodeId.ToString());
            ViewBag.PersonnelCodedata = SetSelected(await GetPersonnelCodes(), _personnelCode != null ? _personnelCode.ToString() : "");



            //Get grid results

            int pageNumber = (page ?? 1);

            var data = _accountRepository.GetAccounts(_firstName, _lastName, _systemId, _unitId, _titleId, _jobCodeId, _personnelCode, _buildingId, sortOrder, pageNumber, pageSize ?? 100);

            TempData["ResultCount"] = data.Count().ToString();
            TempData["TotalCount"] = data.TotalItemCount.ToString();
            TempData["PageSize"] = pageSize.ToString();

            string comment = "";

            if (submitAction == "Search")
            {
                if (_firstName != "") { comment += $"[ firstname : {_firstName}] "; }
                if (_lastName != "") { comment += $"[ lastname : {_lastName}] "; }
                if (_unitId != 0) { comment += $"[ department : {GetSelected(ViewBag.Unitdata, _unitId.ToString())}] "; }
                if (_titleId != 0) { comment += $"[ title : {GetSelected(ViewBag.Titledata, _titleId.ToString())}] "; }
                if (_jobCodeId != 0) { comment += $"[ jobcode : {GetSelected(ViewBag.JobCodedata, _jobCodeId.ToString())}] "; }
                if (_personnelCode != "") { comment += $"[ personnelcode : {GetSelected(ViewBag.PersonnelCodedata, _personnelCode)}] "; }
                if (_buildingId != 0) { comment += $"[ building : {GetSelected(ViewBag.Buildingdata, _buildingId.ToString())}] "; }

                _log.Log((int)ActionTypes.Search, "dbo.Account", "", comment);
            }


            return View(data);
        }

        /// <summary>
        /// GET action method to load custom list of contractors on "Views/Account/Contractors.cshtml"
        /// </summary>
        /// <returns>AccountModel list</returns>

        [DisplayName("Contractors")]
        public async Task<ViewResult> Contractors()
        {
            var data = await _accountRepository.GetContractors();

            ViewData["ResultCount"] = data.Count().ToString();


            var unitData = _accountRepository.GetUnits();
            ViewBag.SearchUnitData = unitData;

            return View(data);
        }


    }


}

