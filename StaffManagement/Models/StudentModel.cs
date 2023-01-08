using System;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models
{
    public class StudentModel
    {
        [Key]
        public int StudentId { get; set; }
        public Nullable<int> SisBuildingId { get; set; }
        public string GradeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string CurrentStatus { get; set; }
        public Nullable<DateTime> WithdrawalDate { get; set; }
        public Nullable<short> GraduationYear { get; set; }
        public string StateReportId { get; set; }
        public Nullable<DateTime> LastModified { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
