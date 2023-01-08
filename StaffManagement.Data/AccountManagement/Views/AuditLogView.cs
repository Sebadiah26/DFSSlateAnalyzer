using System;
#nullable disable

namespace StaffManagement.Data
{
    public partial class AuditLogView
    {
        public AuditLogView()
        {

        }
        public int LogId { get; set; }
        public DateTime LogDate { get; set; }
        public string ReferenceTable { get; set; }
        public string ReferenceID { get; set; }
        public string Employee { get; set; }

        public string ActionType { get; set; }
        public string ApplicationUser { get; set; }
        public string ImpersonatedUser { get; set; }
        public string Comment { get; set; }

        public string CommentHTML => Comment?.Replace(")", ")<br />");




    }
}
