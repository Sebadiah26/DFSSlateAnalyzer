using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Student
    {
        [Key]
        public int StudentId { get; set; }
        public int SisBuildingId { get; set; }
        public string GradeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public short? GraduationYear { get; set; }
        public string StateReportId { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Grade Grade { get; set; }
        public virtual sis_Building SisBuilding { get; set; }
    }
}
