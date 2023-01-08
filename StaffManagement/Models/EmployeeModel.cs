using StaffManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffManagement.Models
{
    public class EmployeeModel : BaseViewModel
    {


        public EmployeeModel()
        {



            EmployeeJob = new EmployeeJobModel();
            EmployeeJobs = new List<EmployeeJobModel>();
        }

        public string Slug => FirstName?.Replace(' ', '-').ToLower() + '-' + LastName?.Replace(' ', '-').ToLower();


        [Key]
        public int EmployeeId { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Suffix Name")]
        public string SuffixName { get; set; }
        public string FsfBuildingId { get; set; }
        public string PersonnelCode { get; set; }
        public Nullable<int> Jobcode { get; set; }
        public string EmployeeStatus { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> UnitIdManual { get; set; }

        [Display(Name = "HR JobTitle")]
        public string JobTitle { get; set; }
        public string JobTitleManual { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

        [Display(Name = "Active")]
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<DateTime> LastUpdate { get; set; }

        [Display(Name = "Department")]
        public virtual Unit Unit { get; set; }

        public string EncryptedRouteId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Sis_Staff Sis_Staff { get; set; }

        [ForeignKey("FsfBuildingId")]
        [Display(Name = "Building")]
        public virtual Building Building { get; set; }

        [ForeignKey("FsfBuildingId")]
        public virtual hr_Building fsfBuilding { get; set; }

        //  [ForeignKey("EmployeeId")]
        // public virtual NeedsReview NeedsReview { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Account Account { get; set; }


        //public SelectList Employees { get; set; }

        public Nullable<int> SelectedEmployeeId { get; set; }
        public string SelectedEmployee { get; set; }

        public bool IsSubstitute { get; set; } = false;

        [Display(Name = "Employee")]
        public Dictionary<Int32, string> Employees { get; set; }

        public virtual EmployeeJobModel EmployeeJob { get; set; }

        public virtual List<EmployeeJobModel> EmployeeJobs { get; set; }



    }
}
