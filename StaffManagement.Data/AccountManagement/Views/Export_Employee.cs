using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Export_Employee
    {
        public int SystemId { get; set; }
        public Guid? AdobjectGuid { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string Nickname { get; set; }
        public string JobTitle { get; set; }
        public string Role { get; set; }
        public string SchoologyUserId { get; set; }
        public int? UnitId { get; set; }
        public string UsernameOverride { get; set; }
        public string SanitizedFirstName { get; set; }
        public string SanitizedMiddleName { get; set; }
        public string SanitizedLastName { get; set; }
        public string SanitizedSuffixName { get; set; }
    }
}
