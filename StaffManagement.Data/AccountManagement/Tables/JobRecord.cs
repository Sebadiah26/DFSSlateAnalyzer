using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class JobRecord
    {
        public string Slug => Account.FirstName?.Replace(' ', '-').ToLower() + '-' + Account.LastName?.Replace(' ', '-').ToLower();


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int JobRecordId { get; set; }
        public int? SystemId { get; set; }
        [Display(Name = "Is Primary Location")]
        public bool IsPrimary { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


        [Display(Name = "Department")]
        public Nullable<int> UnitId { get; set; }

        [Display(Name = "Building")]
        public Nullable<int> BuildingId { get; set; }

        [Display(Name = "Substituting For")]
        public Nullable<int> SubstituteForId { get; set; }

        public Nullable<int> TitleId { get; set; }

        public Nullable<int> TitleGroupId { get; set; }

        [ForeignKey("TitleId")]
        public virtual Title Title { get; set; }

        [ForeignKey("TitleGroupId")]
        public virtual TitleGroup TitleGroup { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<DateTime> StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<DateTime> EndDate { get; set; }

        [Required(ErrorMessage = "Please choose the employee type.")]
        [Display(Name = "Employee Type")]
        public bool IsPHRST { get; set; }

        [Required(ErrorMessage = "Please choose the employee type.")]
        [Display(Name = "Employee Type")]
        public string[] EmployeeTypes = new[] { "Employee", "Contractor/Other" };

        [Display(Name = "Record Number")]
        public Nullable<int> HrRecordNumber { get; set; }

        [Display(Name = "Building")]
        public string? HrWorkLocation { get; set; }
        [Display(Name = "Personnel Code")]
        public string? HrPersonnelCode { get; set; }
        [Display(Name = "Job Code")]
        public Nullable<int> HrJobCode { get; set; }
        [Display(Name = "Is Primary")]
        public Nullable<bool> HrPrimary { get; set; }
        [Display(Name = "Is Matched")]
        public Nullable<bool> HrMatched { get; set; }
        [Display(Name = "Needs Review")]
        public bool NeedsReview { get; set; }


        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }

        public Nullable<DateTime> LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }


        [Display(Name = "Department")]
        public virtual Unit Unit { get; set; }

        [Display(Name = "Building")]
        public virtual Building Building { get; set; }


        [ForeignKey("SystemId")]
        public virtual Account Account { get; set; }


        public virtual Jobcode Jobcode { get; set; }

        [NotMapped]
        public virtual PersonnelCode PersonnelCode { get; set; }

    }
}
