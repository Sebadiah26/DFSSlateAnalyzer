using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    /// <summary>
    /// class for the Accounts dbSet in accountmanagementContext.cs
    /// </summary>
    public partial class Account
    {
        public Account()
        {


            JobRecords = new HashSet<JobRecord>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SystemId { get; set; }

        [NotMapped]
        public string Name => FirstName?.ToString() + ' ' + LastName?.ToString();

        public Nullable<Guid> AdobjectGuid { get; set; }

        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> ESchoolStaffId { get; set; }
        public Nullable<int> ContractorId { get; set; }
        public Nullable<int> StudentId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SuffixName { get; set; }
        public string AlternateFirstName { get; set; }
        public string AlternateLastName { get; set; }
        public string Nickname { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }


        [Display(Name = "Building")]
        public Nullable<int> PrimaryUnitId { get; set; }
        public string UsernameOverride { get; set; }
        public bool? IsActive { get; set; }
        public string Source { get; set; }
        public Nullable<DateTime> LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }




        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }

        public int? AccountTypeId { get; set; }

        [Display(Name = "Building")]
        public virtual Unit PrimaryUnit { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        //   public virtual Contract Contract { get; set; }
        [ForeignKey("AdobjectGuid")]
        public virtual User User { get; set; }

        [ForeignKey("SystemId")]
        public virtual Permission Permission { get; set; }

        // [ForeignKey("SystemID")]
        //  public virtual Export_Employee Export_Employee { get; set; }

        [ForeignKey("SystemId")]
        public virtual AccountUnit AccountUnit { get; set; }

        [ForeignKey("AccountTypeId")]
        public virtual AccountType AccountType { get; set; }

        // [ForeignKey("SystemId")]
        // public virtual Select_Available_JobTitles JobTitles { get; set; }




        public virtual ICollection<JobRecord> JobRecords { get; set; }

        // [ForeignKey("SystemId")]
        // public virtual JobTitleAssignment JobTitleAssignment { get; set; }

    }
}
