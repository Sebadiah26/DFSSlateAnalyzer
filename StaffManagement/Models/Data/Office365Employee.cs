using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Office365Employee
    {
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
    }
}
