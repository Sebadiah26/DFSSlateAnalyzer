using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Employee
    {
        public Employee()
        {

            EmployeeAdditionalUnits = new HashSet<EmployeeAdditionalUnit>();
            EmployeeJobs = new HashSet<EmployeeJob>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string FsfBuildingId { get; set; }
        public string PersonnelCode { get; set; }
        public int Jobcode { get; set; }
        public string EmployeeStatus { get; set; }
        public int? UnitId { get; set; }
        public int? UnitIdManual { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleManual { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime LastUpdate { get; set; }

        [ForeignKey("FsfBuildingId")]
        public virtual hr_Building FsfBuilding { get; set; }
        public virtual Jobcode Jobcodes { get; set; }
        public virtual PersonnelCode PersonnelCodes { get; set; }

        [Display(Name = "Department")]
        public virtual Unit Unit { get; set; }
        public virtual ICollection<EmployeeAdditionalUnit> EmployeeAdditionalUnits { get; set; }


        public virtual ICollection<EmployeeJob> EmployeeJobs { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Account Account { get; set; }

        //  [ForeignKey("EmployeeId")]
        //public virtual NeedsReview NeedsReview { get; set; }

        //  [ForeignKey("EmployeeId")]
        // public virtual EmployeeLatestJob EmployeeLatestJob { get; set; }


        //public virtual Sis_Staff Sis_Staff { get; set; }

        //[ForeignKey("FsfBuildingId")]
        //[Display(Name = "Building")]
        //public virtual Building Building { get; set; }



    }
}
