#nullable disable

namespace StaffManagement.Data
{
    public partial class EmployeeSource
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string FsfBuildingId { get; set; }
        public string PersonnelCode { get; set; }
        public string Jobcode { get; set; }
        public string EmployeeStatus { get; set; }
    }
}
