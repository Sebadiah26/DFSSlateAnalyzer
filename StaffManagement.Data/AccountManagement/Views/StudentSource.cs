using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class StudentSource
    {
        public int? StudentId { get; set; }
        public int SisBuildingId { get; set; }
        public string GradeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public string StateReportId { get; set; }
        public short? GraduationYear { get; set; }
    }
}
