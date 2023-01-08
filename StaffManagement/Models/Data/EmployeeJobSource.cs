using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class EmployeeJobSource
    {
        public int EmployeeId { get; set; }
        public short JobId { get; set; }
        public bool? IsPrimary { get; set; }
        public string FsfBuildingId { get; set; }
        public string PersonnelCode { get; set; }
        public string Jobcode { get; set; }
    }
}
