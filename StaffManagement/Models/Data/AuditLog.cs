using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class AuditLog
    {
        public int LogId { get; set; }
        public DateTime LogDate { get; set; }
        public string ReferenceTable { get; set; }
        public string ReferenceId { get; set; }
        public int ActionTypeId { get; set; }
        public int AppUser_SystemId { get; set; }

        public int AppUser_ImpersonatedUserId { get; set; }


        public string Comment { get; set; }
    }
}
