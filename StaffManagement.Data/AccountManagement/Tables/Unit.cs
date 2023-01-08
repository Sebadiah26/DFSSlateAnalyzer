
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Unit
    {
        public Unit()
        {

            Accounts = new HashSet<Account>();
            AccountUnits = new HashSet<AccountUnit>();
            ContractAdditionalUnits = new HashSet<ContractAdditionalUnit>();
            Contracts = new HashSet<Contract>();
            Employees = new HashSet<Employee>();
            EmployeeAdditionalUnits = new HashSet<EmployeeAdditionalUnit>();
            EmployeeJobs = new HashSet<EmployeeJob>();
        }

        [Required(ErrorMessage = "Please select a department.")]
        public int UnitId { get; set; }
        [Display(Name = "Building")]
        public int BuildingId { get; set; }
        [Display(Name = "Department")]
        public string Name { get; set; }
        public string ShortName { get; set; }
        [Display(Name = "Department")]
        public string Adname { get; set; }
        public bool UseGenericAdgroups { get; set; }
        public string HomeDirServer { get; set; }
        public string Comment { get; set; }
        public string HrWorkLocationMatch { get; set; }
        public string HrJobcodeMatch { get; set; }
        public int? SisBuildingMatch { get; set; }
        public string PhoneOffice { get; set; }
        public string PhoneFax { get; set; }
        public string PhoneGuidance { get; set; }
        public string PhoneNurse { get; set; }
        [Display(Name = "Building")]
        public string AdgroupName { get; set; }

        public virtual Building Building { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }

        public virtual ICollection<AccountUnit> AccountUnits { get; set; }
        public virtual ICollection<ContractAdditionalUnit> ContractAdditionalUnits { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<EmployeeAdditionalUnit> EmployeeAdditionalUnits { get; set; }
        public virtual ICollection<EmployeeJob> EmployeeJobs { get; set; }

        //public List<SelectListItem> Units { get; set; }

        [NotMapped]
        public SelectList Units { get; set; }
    }
}
