using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class StudentAccount
    {
        public int SystemId { get; set; }
        public Guid? AdobjectGuid { get; set; }
        public int StudentId { get; set; }
        public int SisBuildingId { get; set; }
        public string Grade { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string AccountStatus { get; set; }
        public short? GraduationYear { get; set; }
        public string StateReportId { get; set; }
        public string Role { get; set; }
        public string UsernameOverride { get; set; }
        public string SanitizedFirstName { get; set; }
        public string SanitizedMiddleName { get; set; }
        public string SanitizedLastName { get; set; }
        public string SanitizedSuffixName { get; set; }
    }
}
