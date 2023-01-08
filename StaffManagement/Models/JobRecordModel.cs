using StaffManagement.Data;
using StaffManagement.Models.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Models
{
    public partial class JobRecordModel
    {

        public JobRecordModel()
        {
            TitleGroup = new TitleGroup();
            Title = new Title();
            Unit = new UnitModel();
        }

        [Key]
        public int JobRecordId { get; set; }
        public int? SystemId { get; set; }
        [Display(Name = "Is Primary Location")]
        public bool IsPrimary { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a department."), Range(1, 999999)]
        [Display(Name = "Department")]
        public Nullable<int> UnitId { get; set; }

        [Required(ErrorMessage = "Please select a building.")]
        [Display(Name = "Building")]
        public Nullable<int> BuildingId { get; set; }

        [Display(Name = "Substituting For")]
        public Nullable<int> SubstituteForId { get; set; }

        public string SubstituteFor { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please select a title.")]
        public int TitleId { get; set; }

        [Display(Name = "Title Group")]
        [Required(ErrorMessage = "Please select a title group.")]
        public int TitleGroupId { get; set; }

        [ValidIfContractor]
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public Nullable<DateTime> StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public Nullable<DateTime> EndDate { get; set; }


        private string _displayEndDate = null;
        public string DisplayEndDate



        {
            get
            {
                if (_displayEndDate == null)
                {

                    if (EndDate == null)
                    {
                        _displayEndDate = "";
                    }
                    else
                    {
                        _displayEndDate = ((DateTime)EndDate).ToString("MM/dd/yyyy");
                    }


                }


                return _displayEndDate;

            }
        }



        private string _displayStartDate = null;
        public string DisplayStartDate



        {
            get
            {
                if (_displayStartDate == null)
                {

                    if (StartDate == null)
                    {
                        _displayStartDate = "";
                    }
                    else
                    {
                        _displayStartDate = ((DateTime)StartDate).ToString("MM/dd/yyyy");
                    }


                }


                return _displayStartDate;

            }
        }




        //[Required(ErrorMessage = "Please choose the employee type.")]
        [Display(Name = "Employee Type")]
        public Nullable<bool> IsPHRST { get; set; }

        //[Required(ErrorMessage = "Please choose the employee type.")]
        [Display(Name = "Employee Type")]
        public string[] EmployeeTypes = new[] { "Employee", "Contractor/Other" };

        [Display(Name = "Record Number")]
        public Nullable<int> HrRecordNumber { get; set; }

        [Display(Name = "Building")]
        public string HrWorkLocation { get; set; }
        [Display(Name = "Personnel Code")]
        public string HrPersonnelCode { get; set; }
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
        public virtual UnitModel Unit { get; set; }

        [ForeignKey("TitleId")]
        public virtual Title Title { get; set; }

        [Display(Name = "Title Group")]
        [ForeignKey("TitleGroupId")]
        public virtual TitleGroup TitleGroup { get; set; }

        [Display(Name = "Building")]
        public virtual BuildingModel Building { get; set; }

        [ForeignKey("SystemId")]
        public virtual AccountModel Account { get; set; }

        //public virtual Employee Employee { get; set; }
        public virtual Jobcode Jobcode { get; set; }
        public virtual PersonnelCode PersonnelCode { get; set; }
        //public virtual Unit Unit { get; set; }

        //related fields not in JobRecord table
        public string employeeJobTitle { get; set; }
        public bool employeejobIsPrimary { get; set; }
        public string employeejobHrPersonnelCode { get; set; }
        public string employeejobHrJobCode { get; set; }

        public bool IsCurrentJobRecord { get; set; } = false;

        public bool IsMatchedWithEmployeeJob { get; set; } = false;





    }
}
