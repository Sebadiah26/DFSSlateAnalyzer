using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models.CustomAttributes;
using StaffManagement.Models.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StaffManagement.Models
{

    public enum EnumAccountTypeDescription
    {
        UnAssigned = 0,
        Student = 1,
        Employee = 2,
        Contractor = 3,
        LongTermSub = 4,
        NonPerson = 5


    }


    /// <summary>
    /// Account object model with account properties and  related employee, user, jobrecord objects
    /// </summary>
    public class AccountModel : BaseViewModel
    {

        public AccountModel()
        {

            AccountType = new AccountTypeModel();
            PrimaryUnit = new UnitModel();
            Employee = new EmployeeModel();
            JobRecord = new JobRecordModel();
            JobRecords = new List<JobRecordModel>();
            User = new UserModel();
            AuditLogView = new List<AuditLogView>();
        }




        [EnumDataType(typeof(EnumAccountTypeDescription))]
        public EnumAccountTypeDescription AccountTypeDescription

        {
            get

            {
                return (EnumAccountTypeDescription)(AccountTypeId ?? 0);


                ;

            }


        }


        /// <summary>
        /// attached to page url to make it friendly
        /// </summary>
        public string Slug => DerivedFirstName?.Replace(' ', '-').ToLower() + '-' + DerivedLastName?.Replace(' ', '-').ToLower();

        public string Name => DerivedFirstName?.ToString() + ' ' + DerivedLastName?.ToString();


        private string _derivedFirstName = "";
        private string _derivedLastName = "";

        /// <summary>
        /// Checks three sources for first name data (hr.employee, dbo.account, ad.user)
        /// </summary>
        public string DerivedFirstName
        {
            get
            {
                if (_derivedFirstName == "")
                {

                    if ((Employee.FirstName ?? "").Length > 0)
                    {
                        _derivedFirstName += Employee.FirstName;
                    }
                    else
                    {
                        _derivedFirstName += (FirstName ?? (User.GivenName ?? ""));
                    }


                }


                return _derivedFirstName;

            }
        }

        /// <summary>
        /// Checks three sources for last name data (hr.employee, dbo.account, ad.user)
        /// </summary>
        public string DerivedLastName
        {
            get
            {
                if (_derivedLastName == "")
                {

                    if ((Employee.LastName ?? "").Length > 0)
                    {
                        _derivedLastName += Employee.LastName;
                    }
                    else
                    {
                        _derivedLastName += (LastName ?? (User.Surname ?? ""));
                    }


                }


                return _derivedLastName;

            }
        }



        private string _derivedFullName = "";

        /// <summary>
        /// Checks if Alt first or last name is entered, otherwise load derived first and last name
        /// </summary>
        public string DerivedFullName
        {
            get
            {
                if (_derivedFullName == "")
                {

                    if ((AlternateFirstName ?? "").Length > 0)
                    {
                        _derivedFullName += AlternateFirstName;
                    }
                    else
                    {
                        _derivedFullName += (DerivedFirstName ?? "");
                    }

                    if ((AlternateLastName ?? "").Length > 0)
                    {
                        _derivedFullName += " " + AlternateLastName;
                    }
                    else
                    {
                        _derivedFullName += " " + (DerivedLastName ?? "");
                    }

                }


                return _derivedFullName;

            }
        }

        private string _derivedSAMAcountName = "";

        /// <summary>
        /// Checks UsernameOverride first, then Derived first and last name to determine SAMAccountName, removing whitespace
        /// </summary>
        [Display(Name = "Computer Logon")]
        public string DerivedSAMAccountName
        {
            get
            {
                if (_derivedSAMAcountName == "")
                {

                    if (UsernameOverride != null && UsernameOverride.Length > 0)
                    {
                        _derivedSAMAcountName = UsernameOverride;
                    }
                    else
                    {

                        if ((AlternateFirstName ?? "") != "")
                        {
                            _derivedSAMAcountName += AlternateFirstName.RemoveWhitespace();
                        }
                        else
                        {
                            _derivedSAMAcountName += DerivedFirstName.RemoveWhitespace();
                        }


                        if ((AlternateLastName ?? "") != "")
                        {
                            _derivedSAMAcountName += "." + AlternateLastName.RemoveWhitespace();
                        }
                        else
                        {
                            _derivedSAMAcountName += "." + DerivedLastName.RemoveWhitespace();
                        }

                    }


                }

                _derivedSAMAcountName = _derivedSAMAcountName.Substring(0, (_derivedSAMAcountName.Length >= 20 ? 20 : _derivedSAMAcountName.Length));


                return _derivedSAMAcountName;
            }
        }

        public bool IsDerivedUserField
        {
            get
            {
                if (ADObjectGuid == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Dynamically set email address based on SAMAccountName and Email Domain from settings
        /// </summary>
        [Display(Name = "Email Logon")]
        public string DerivedEmail
        {
            get
            {

                return DerivedSAMAccountName + "@" + Settings.Domain;

            }
        }


        //[Required(ErrorMessage = "Please enter a systemid.")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SystemId { get; set; }

        [Display(Name = "Active Directory Guid")]
        public Nullable<Guid> ADObjectGuid { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}", ApplyFormatInEditMode = true)]
        public Nullable<int> EmployeeId { get; set; }



        public Nullable<int> ContractorId { get; set; }

        public Nullable<int> StudentId { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [RequiredIfActionMethod(RequiredActionMethod = "Add", CustomErrorMessage = "Please select a first name.")]
        //[Required(ErrorMessage = "Please select a first name.")]
        [Display(Name = "First Name")]
        public string EditFirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Middle Name")]
        public string EditMiddleName { get; set; }

        private string _LastName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName


        {
            get
            { return _LastName; }


            set
            {


                _LastName = value;
                RaisePropertyChanged();
                RaisePropertyChanged("NameIsCurrentWithPHRST");
            }

        }




        [RequiredIfActionMethod(RequiredActionMethod = "Add", CustomErrorMessage = "Please select a last name.")]
        [Display(Name = "Last Name")]
        public string EditLastName { get; set; }



        [Display(Name = "Suffix")]
        public string SuffixName { get; set; }

        [Display(Name = "Suffix")]
        public string EditSuffixName { get; set; }


        [Display(Name = "Alternate First Name")]
        public string AlternateFirstName { get; set; }

        [Display(Name = "Alternate Last Name")]
        public string AlternateLastName { get; set; }
        public string Nickname { get; set; }

        // [Display(Name = "Display Jobtitle")]
        // public string DisplayJobTitle { get; set; }
        public string JobTitle { get; set; }
        public string JobCode { get; set; }

        public string Description { get; set; }


        [Display(Name = "Department")]
        public Nullable<int> PrimaryUnitId { get; set; }

        [Display(Name = "Username Override")]
        public string UsernameOverride { get; set; }

        [Display(Name = "Status")]
        public Nullable<bool> IsActive { get; set; }

        //[Required(ErrorMessage = "Please select a source.")]
        public string Source { get; set; }

        [Display(Name = "Last Updated")]
        public Nullable<DateTime> LastUpdate { get; set; }

        [Display(Name = "Last Updated By")]
        public string LastUpdateUser { get; set; }

        private string _lastUpdateInfo { get; set; } = null;
        private DateTime? _lastUpdated { get; set; } = null;

        [Display(Name = "Last Updated Info")]
        public string LastUpdateInfo
        {
            get
            {
                if (AccountLastUpdate != null)
                {
                    _lastUpdated = AccountLastUpdate;
                    LastUpdate = _lastUpdated;
                    LastUpdateUser = AccountLastUpdateUser;
                }

                if (JobRecordLastUpdate != null && JobRecordLastUpdate > _lastUpdated)
                {
                    _lastUpdated = JobRecordLastUpdate;
                    LastUpdate = _lastUpdated;
                    LastUpdateUser = JobRecordLastUpdateUser;
                }

                if (_lastUpdated != null)
                { _lastUpdateInfo = "last updated " + _lastUpdated.ToString() + " by " + LastUpdateUser; }


                return _lastUpdateInfo;
            }

        }

        [Display(Name = "Last Updated")]
        public Nullable<DateTime> AccountLastUpdate { get; set; }

        [Display(Name = "Last Updated By")]
        public string AccountLastUpdateUser { get; set; }

        [Display(Name = "Last Updated")]
        public Nullable<DateTime> JobRecordLastUpdate { get; set; }

        [Display(Name = "Last Updated By")]
        public string JobRecordLastUpdateUser { get; set; }


        [Display(Name = "Date Created")]
        public Nullable<DateTime> CreateDate { get; set; }

        [Display(Name = "Created By")]
        public string CreateUser { get; set; }

        [Required(ErrorMessage = "Please select an Employee type.")]
        [Display(Name = "Employee Type")]
        public int? AccountTypeId { get; set; }

        //[Display(Name = "Building")]
        //public UnitModel PrimaryUnit { get; set; }


        [Display(Name = "Department")]
        public virtual UnitModel PrimaryUnit { get; set; }

        [ForeignKey("EmployeeId")]
        public EmployeeModel Employee { get; set; }
        [ForeignKey("StudentId")]
        public StudentModel Student { get; set; }

        [ForeignKey("ADObjectGuid")]
        public UserModel User { get; set; }

        [ForeignKey("SystemId")]
        public virtual AccountUnit AccountUnit { get; set; }



        public virtual AccountTypeModel AccountType { get; set; }

        [ValidIfLongTermSub]
        [ValidIfContractor]
        public virtual JobRecordModel JobRecord { get; set; }

        private List<JobRecordModel> _jobRecords { get; set; }
        public virtual List<JobRecordModel> JobRecords

        {
            get
            { return _jobRecords; }


            set
            {

                if (_jobRecords == value)
                    return;
                _jobRecords = value;
                RaisePropertyChanged();
                RaisePropertyChanged("JobIsCurrentWithPHRST");
            }

        }

        private bool? _isCurrent { get; set; } = null;

        public bool IsCurrentWithPHRST

        {
            get
            {
                if (_isCurrent == null)
                {
                    _isCurrent = true;

                    if ((NameIsCurrentWithPHRST == false || JobIsCurrentWithPHRST == false) && IsLinkedToPHRST == true)
                    {
                        _isCurrent = false;
                    }
                }

                return (bool)_isCurrent;
            }
        }

        private bool? _nameIsCurrent { get; set; } = null;

        public bool NameIsCurrentWithPHRST

        {
            get
            {

                _nameIsCurrent = true;

                if (FirstName != Employee.FirstName ||
                    MiddleName != Employee.MiddleName ||
                    LastName != Employee.LastName ||
                    SuffixName != Employee.SuffixName
                    )
                {
                    _nameIsCurrent = false;
                }




                return (bool)_nameIsCurrent;
            }
        }


        private bool? _jobIsCurrent { get; set; } = null;

        /// <summary>
        /// Check if a current job record matches current hr.employeejob data
        /// </summary>
        public bool JobIsCurrentWithPHRST

        {
            get
            {
                // default to true and set false below as matches are found 
                _jobIsCurrent = true;


                // not applicable, set to true
                if ((EmployeeId ?? 0) == 0)
                { _jobIsCurrent = true; }
                else
                {

                    // Set   MismatchedJobRecordId

                    // loop through PHRST records (hr.EmployeeJob)
                    foreach (EmployeeJobModel employeejob in Employee.EmployeeJobs)
                    {


                        // loop through job records, (dbo.JobRecord) to check for matches
                        foreach (JobRecordModel job in JobRecords)
                        {

                            // if info matches and active/current
                            if (job.HrWorkLocation == employeejob.FsfBuildingId &&
                               job.HrPersonnelCode == employeejob.PersonnelCode &&
                               job.HrJobCode == employeejob.Jobcode &&
                               job.IsActive == true &&
                               DateTime.Now > (job.StartDate ?? Settings.BeginDate) &&
                               DateTime.Now < (job.EndDate ?? Settings.EndDate))
                            {
                                // Match found, set Job Recoremployeejob.Jobcoded and Employee Job matched and current
                                job.IsMatchedWithEmployeeJob = true;
                                employeejob.IsCurrentWithPHRST = true;


                            }

                        }


                        if (employeejob.IsCurrentWithPHRST == false)
                        {
                            // Match for Employee Job in loop was not found, meaning that the account as a whole has one or more issues.
                            _jobIsCurrent = false;
                            MismatchedJobCode = employeejob.Jobcode;
                            MismatchedPersonnelCode = employeejob.PersonnelCode;
                            MismatchedWorkLocation = employeejob.FsfBuildingId;


                        }


                    }


                    foreach (JobRecordModel job in JobRecords)
                    {

                        if (job.IsMatchedWithEmployeeJob != true && (job.IsActive == true || JobRecords.Count == 1) &&
                       DateTime.Now > (job.StartDate ?? Settings.BeginDate) &&
                       DateTime.Now < (job.EndDate ?? Settings.EndDate))
                        {
                            // Sets first unmatched job found on the Account level
                            MismatchedJobRecordId = job.JobRecordId;

                        }

                    }


                    // // Set   Any missing EmployeeJob Records  

                    //// Loop through Employee and JobRecords   
                    //if ((MismatchedJobRecordId  == 0) && (_jobIsCurrent ?? true == false))
                    // {

                    // }


                }



                return (bool)_jobIsCurrent;
            }

        }

        public int MismatchedJobRecordId { get; set; }



        public string MismatchedPersonnelCode { get; set; }
        public Nullable<int> MismatchedJobCode { get; set; }
        public string MismatchedWorkLocation { get; set; }

        public bool WorkLocationChanged
        {
            get
            {
                if (JobRecord.HrWorkLocation == Employee.EmployeeJob.FsfBuildingId && JobRecord.UnitId == Employee.EmployeeJob.UnitId)
                {
                    return false;
                }
                else
                {
                    if (JobIsCurrentWithPHRST == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        public bool JobChanged
        {
            get
            {
                if (JobRecord.HrPersonnelCode == Employee.EmployeeJob.PersonnelCode
                    && JobRecord.HrJobCode == Employee.EmployeeJob.Jobcode)
                {
                    return false;
                }
                else
                {
                    if (JobIsCurrentWithPHRST == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }


        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }

        public int NewUnitId { get; set; }

        public int NewBuildingId { get; set; }


        // [ForeignKey("SystemId")]
        //    public virtual JobTitleAssignment JobTitleAssignment { get; set; }


        public virtual List<AuditLogView> AuditLogView { get; set; }



        private string _phrstChangesDescription;



        /// <summary>
        ///
        /// </summary>
        public string PHRSTChangesDescription
        {
            get
            {
                _phrstChangesDescription = "";

                if (JobIsCurrentWithPHRST == false && Employee.EmployeeJob.EmployeeJobs != null)
                {


                    string[] employeejob = Employee.EmployeeJob.EmployeeJobs.ElementAt(0).Value.Split('#');



                    if (Employee.EmployeeJob.EmployeeJobs.Count() == 1)
                    {
                        if (employeejob[0].ToString() != JobRecord.HrWorkLocation)
                        {
                            _phrstChangesDescription += "Location Change <br />";
                        }

                        if (employeejob[1].ToString() != JobRecord.HrPersonnelCode)
                        {
                            _phrstChangesDescription += "Personnel Code Change <br />";
                        }

                        if (Int32.Parse(employeejob[2]) != (int)(JobRecord.HrJobCode ?? 0))
                        {
                            _phrstChangesDescription += "Job Code Change <br />";
                        }

                        if (Employee.EmployeeJobs.Count == 1 && (int)(Employee.EmployeeJobs.ElementAt(0).UnitId ?? 0) != (int)(JobRecord.UnitId ?? 0))
                        {
                            _phrstChangesDescription += "Department Change <br />";
                        }


                    }


                }




                return _phrstChangesDescription;

            }
        }





        private string _currentJob;
        private string _pastJob;
        private string _futureJob;


        /// <summary>
        /// Get Current job(s) as string based on checking start and end dates of job records
        /// </summary>
        public string CurrentJob
        {
            get
            {
                _currentJob = "";


                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == true && (job.StartDate ?? Settings.BeginDate) < DateTime.Now && (job.EndDate ?? Settings.EndDate) > DateTime.Now)
                        {
                            _currentJob += "(PRIMARY) " + (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text ?? "";
                            _currentJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _currentJob += "<br />";
                            job.IsCurrentJobRecord = true;
                        }
                    }

                }

                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == false && (job.StartDate ?? Settings.BeginDate) < DateTime.Now && (job.EndDate ?? Settings.EndDate) > DateTime.Now)
                        {
                            _currentJob += (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text ?? "";
                            _currentJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _currentJob += "<br />";
                            job.IsCurrentJobRecord = true;
                        }
                    }

                }



                return _currentJob;

            }
        }


        /// <summary>
        ///  Get Future job(s) as string based on checking start and end dates of job records
        /// </summary>
        public string FutureJob
        {
            get
            {
                _futureJob = "";

                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == true && (job.StartDate ?? Settings.BeginDate) > DateTime.Now && (job.EndDate ?? Settings.EndDate) > DateTime.Now)
                        {
                            _futureJob += "(PRIMARY) " + (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text;
                            _futureJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _futureJob += "<br />";
                        }
                    }

                }

                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == false && (job.StartDate ?? Settings.BeginDate) > DateTime.Now && (job.EndDate ?? Settings.EndDate) > DateTime.Now)
                        {
                            _futureJob += (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text;
                            _futureJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _futureJob += "<br />";
                        }
                    }

                }



                return _futureJob;

            }
        }


        /// <summary>
        ///  Get Past job(s) as string based on checking start and end dates of job records
        /// </summary>
        public string PastJob
        {
            get
            {
                _pastJob = "";

                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == true && (job.StartDate ?? Settings.BeginDate) < DateTime.Now && (job.EndDate ?? Settings.EndDate) < DateTime.Now)
                        {
                            _pastJob += "(PRIMARY) " + (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text;
                            _pastJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _pastJob += "<br />";
                        }
                    }

                }

                foreach (JobRecordModel job in JobRecords)
                {
                    if (job.TitleId != 0 && ((job.UnitId ?? 0) != 0))
                    {
                        if (job.IsPrimary == false && (job.StartDate ?? Settings.BeginDate) < DateTime.Now && (job.EndDate ?? Settings.EndDate) < DateTime.Now)
                        {
                            _pastJob += (JobRecord.Title.AllTitles.Single(x => x.Value == job.TitleId.ToString()).Text ?? "") + "/ " +
                                  JobRecord.Unit.Units.Single(x => x.Value == job.UnitId.ToString()).Text;
                            _pastJob += " (" + string.Format("{0:MM/dd/yyyy}", job.StartDate) + " - " + string.Format("{0:MM/dd/yyyy}", job.EndDate) + ")";
                            _pastJob += "<br />";
                        }

                    }
                }



                return _pastJob;

            }
        }

        private string _status;
        /// <summary>
        /// Get "Active/Inactive" string value based on checking several status flags to determine Account status
        /// </summary>
        public string Status
        {

            get
            {
                _status = "";


                if (IsActive == false ||
                    User.IsDisabled == true ||
                    User.IsDeleted == true ||
                    Employee.IsDeleted == true ||
                    (User.AccountExpires ?? Settings.EndDate) < DateTime.Now == true)
                {
                    _status = "Inactive";
                }
                else
                {
                    _status = "Active";
                }


                return _status;

            }




        }

        private string _employeeStatus;

        [Display(Name = "Employee Status")]
        public string EmployeeStatus
        {

            get
            {
                _employeeStatus = "";


                if (Employee.IsDeleted == true)
                {
                    _employeeStatus = "Inactive";
                }
                else
                {
                    _employeeStatus = "Active";
                }


                return _employeeStatus;

            }




        }


        private string _userStatus;

        [Display(Name = "Employee Status")]
        public string UserStatus
        {

            get
            {
                _userStatus = "";


                if (User.IsDisabled == true || User.IsDeleted == true || ((User.AccountExpires ?? Settings.EndDate) < DateTime.Now == true))
                {
                    _userStatus = "Inactive";
                }
                else
                {
                    _userStatus = "Active";
                }


                return _userStatus;

            }




        }


        private string _accountStatus;
        /// <summary>
        /// Get "Active/Inactive" string value based on checking several status flags to determine Account status
        /// </summary>
        /// 

        [Display(Name = "Status")]
        public string AccountStatus
        {

            get
            {
                _accountStatus = "";


                if (IsActive == false ||
                    User.IsDisabled == true ||
                    User.IsDeleted == true ||

                    (User.AccountExpires ?? Settings.EndDate) < DateTime.Now == true)
                {
                    _accountStatus = "Inactive";
                }
                else
                {
                    _accountStatus = "Active";
                }


                return _accountStatus;

            }




        }


        private string _detailedStatus;
        /// <summary>
        /// Get "Active/Inactive" string value based on checking several status flags to determine Account status
        /// </summary>
        public string DetailedStatus
        {

            get
            {
                _detailedStatus = "";


                if (IsActive == false)
                {
                    _detailedStatus += " [Account is Inactive ] ";
                }
                else { _detailedStatus += " [ Account is Active ] "; }

                if (
                    User.IsDisabled == true)
                {
                    _detailedStatus += " [User is Disabled ] ";
                }
                else { _detailedStatus += " [ User is Not Disabled ] "; }

                if (
                    User.IsDeleted == true)
                {
                    _detailedStatus += " [User is Deleted ] ";
                }
                else { _detailedStatus += " [ User is Not Deleted ] "; }

                if (
                   Employee.IsDeleted == true)
                {
                    _detailedStatus += " [Employee is Deleted ] ";
                }
                else { _detailedStatus += " [ Employee is Not Deleted ] "; }

                if (
                   (User.AccountExpires ?? Settings.EndDate) < DateTime.Now == true)
                {
                    _detailedStatus += " [User is Expired ] ";
                }



                return _detailedStatus;

            }




        }



        private bool _accountIsActive = true;

        public bool AccountIsActive
        {
            get
            {
                _accountIsActive = (Status == "Active");

                return _accountIsActive;
            }
        }

        public bool IsLinkedToPHRST
        {
            get
            {
                return (EmployeeId ?? 0) > 0;
            }
        }

        public string NeedsReviewPersonnelCode { get; set; }

        public string NeedsReviewJobCode { get; set; }

        public string NeedsReviewBuilding { get; set; }

        public string NeedsReviewUnit { get; set; }

        private Nullable<int> _primaryCount = null;
        public int PrimaryCount
        {

            get
            {
                if (_primaryCount == null)
                {
                    _primaryCount = 0;

                    foreach (JobRecordModel job in JobRecords)
                    {

                        if ((job.IsPrimary == true) && ((job.StartDate ?? Settings.BeginDate) < DateTime.Now) && ((job.EndDate ?? Settings.EndDate) > DateTime.Now))
                        {
                            _primaryCount += 1;

                        }

                    }

                }

                return _primaryCount ?? 0;


            }






        }


        public AccountModel Load(Account account)
        {
            AccountModel accountmodel = new AccountModel();

            System.DateTime? lastupdate;


            if (account.LastUpdate.HasValue)
            { lastupdate = (DateTime)account.LastUpdate; }
            else
            {
                lastupdate = null;
            }

            // var building = new Building();
            // var unit = new Unit();
            //  var titlegroup = new TitleGroup();
            //  var title = new Title();
            var employeeModel = new EmployeeModel();

            // title.Text = "";
            //titlegroup.Description = "";

            //if (account.PrimaryUnitId != null)
            //{
            //    building = account.PrimaryUnit.Building;
            //    unit = account.PrimaryUnit;
            //    unit.Building = building;
            //}
            //else
            //{

            //    unit.Building = building;
            //    unit.Building.Name = "";
            //}

            if (account.EmployeeId > 0)
            {
                employeeModel.EmployeeId = (int)account.EmployeeId;
                // employeeModel.IsDeleted = account.Employee.IsDeleted;

            }


            accountmodel.SystemId = account.SystemId;
            accountmodel.EmployeeId = account.EmployeeId;
            accountmodel.ADObjectGuid = account.AdobjectGuid;
            accountmodel.FirstName = account.FirstName;
            accountmodel.MiddleName = account.MiddleName;
            accountmodel.LastName = account.LastName;
            accountmodel.SuffixName = account.SuffixName;
            accountmodel.AlternateFirstName = account.AlternateFirstName;
            accountmodel.AlternateLastName = account.AlternateLastName;
            accountmodel.Nickname = account.Nickname;

            accountmodel.JobTitle = account.JobTitle;
            accountmodel.Description = account.Description;

            accountmodel.UsernameOverride = account.UsernameOverride;
            accountmodel.IsActive = account.IsActive;
            accountmodel.Source = account.Source;
            accountmodel.LastUpdate = lastupdate;
            accountmodel.LastUpdateUser = account.LastUpdateUser;
            accountmodel.PrimaryUnitId = account.PrimaryUnitId;
            // accountmodel.PrimaryUnit = unit;

            //accountmodel.Title = account.Title != null ? account.Title : title;
            //accountmodel.TitleGroup = account.TitleGroup != null ? account.TitleGroup : titlegroup;
            //accountmodel.TitleId = account.Title != null ? account.Title.TitleId : 0;
            //accountmodel.TitleGroupId = account.TitleGroup != null ? account.TitleGroup.TitleGroupId : 0;

            accountmodel.CreateDate = account.CreateDate;
            accountmodel.CreateUser = account.CreateUser;
            accountmodel.Employee = employeeModel;


            return accountmodel;

        }






    }
}
