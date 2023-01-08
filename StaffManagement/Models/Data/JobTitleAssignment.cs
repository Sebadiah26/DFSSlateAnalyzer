using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class JobTitleAssignment
    {
        public int JobTitleAssignmentId { get; set; }
        public int SystemId { get; set; }

        public int EmployeeId { get; set; }
        public string personnel_code { get; set; }
        public string jobcode { get; set; }

        public string JobTitleManual { get; set; }
        public string MatchedJobTitle { get; set; }
        public int MatchSourceID { get; set; }

        //public virtual JobTitleAssignmentMatchSource JobTitleAssignmentMatchSource { get; set; }

        [ForeignKey("SystemId")]
        public virtual Account Account { get; set; }
    }
}
