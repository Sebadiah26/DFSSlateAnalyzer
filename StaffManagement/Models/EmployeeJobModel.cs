using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Models
{
    public partial class EmployeeJobModel
    {

        public EmployeeJobModel()
        {


            Units = new UnitModel();
            Buildings = new hr_Building();
            Jobcodes = new Jobcode();
            PersonnelCodes = new PersonnelCode();

        }

        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public string FsfBuildingId { get; set; }
        public bool IsPrimary { get; set; }
        public string PersonnelCode { get; set; }
        public int Jobcode { get; set; }
        public int? UnitId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeModel Employee { get; set; }
        public virtual Jobcode Jobcodes { get; set; }
        public virtual PersonnelCode PersonnelCodes { get; set; }
        public virtual UnitModel Units { get; set; }
        public virtual hr_Building Buildings { get; set; }


        [NotMapped]
        public SelectList EmployeeJobs { get; set; }

        public string SelectedEmployeeJob { get; set; }


        // Matched status for JobRecord that this employee job is matched with
        public bool IsCurrentWithPHRST { get; set; }


        public bool NeedsReview { get; set; }

    }
}


