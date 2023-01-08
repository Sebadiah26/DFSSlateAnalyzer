using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class GroupMembership
    {
        public Guid GroupId { get; set; }
        public Guid MemberId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Group Group { get; set; }
    }
}
