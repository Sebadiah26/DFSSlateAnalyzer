using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class EmployeeJob
    {
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public string FsfBuildingId { get; set; }
        public bool IsPrimary { get; set; }
        public string PersonnelCode { get; set; }
        public int Jobcode { get; set; }
        public int? UnitId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        public virtual Jobcode Jobcodes { get; set; }

        public virtual PersonnelCode PersonnelCodes { get; set; }

        public virtual Unit Unit { get; set; }

        [ForeignKey("FsfBuildingId")]
        public virtual hr_Building Building { get; set; }


    }
}
