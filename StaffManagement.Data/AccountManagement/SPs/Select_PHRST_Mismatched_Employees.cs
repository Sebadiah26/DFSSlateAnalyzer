using System;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_PHRST_Mismatched_Employees
    {
        public Select_PHRST_Mismatched_Employees()
        {

        }
        public Nullable<int> SystemId { get; set; }
        public Nullable<int> EmployeeId { get; set; }

        public Nullable<int> JobRecordId { get; set; }

        public string AccountFirstName { get; set; }
        public string AccountMiddleName { get; set; }
        public string AccountLastName { get; set; }
        public string AccountSuffixName { get; set; }

        public string EmployeeFirstName { get; set; }
        public string EmployeeMiddleName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeSuffixName { get; set; }


        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public Nullable<int> jrUnitId { get; set; }

        public Nullable<int> jrBuildingId { get; set; }

        public string jrHR_WorkLocation { get; set; }

        public string jrHR_PersonnelCode { get; set; }

        public Nullable<int> jrHR_JobCode { get; set; }

        public Nullable<bool> jrIsActive { get; set; }

        public Nullable<bool> jrHR_Primary { get; set; }



        public Nullable<int> ejUnitId { get; set; }

        public Nullable<int> ejBuildingId { get; set; }

        public string ejWorkLocation { get; set; }

        public string ejPersonnel_code { get; set; }

        public Nullable<int> ejJobcode { get; set; }



        public Nullable<bool> ejIsPrimary { get; set; }

        public string Reason { get; set; }

        public string Unit { get; set; }

        public string Building { get; set; }

        public string PersonnelCode { get; set; }

        public string JobCode { get; set; }

        public string JobTitle { get; set; }

        public DateTime EffectiveDate { get; set; }

        [NotMapped]
        public string Slug => EmployeeFirstName?.Replace(' ', '-').ToLower() + '-' + EmployeeLastName?.Replace(' ', '-').ToLower();

    }
}
