#nullable disable

namespace StaffManagement.Data
{
    public partial class GroupExclusionList
    {
        public int FlaggedRuleId { get; set; }
        public int ExludedByRuleId { get; set; }

        public virtual GroupMembershipRule ExludedByRule { get; set; }
        public virtual GroupMembershipRule FlaggedRule { get; set; }
    }
}
