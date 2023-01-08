using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class GroupMembershipRule
    {
        public GroupMembershipRule()
        {
            GroupExclusionListExludedByRules = new HashSet<GroupExclusionList>();
            GroupExclusionListFlaggedRules = new HashSet<GroupExclusionList>();
        }

        public int RuleId { get; set; }
        public int Tier { get; set; }
        public string Description { get; set; }
        public string DirectNameRule { get; set; }
        public string AutomationSql { get; set; }

        public virtual ICollection<GroupExclusionList> GroupExclusionListExludedByRules { get; set; }
        public virtual ICollection<GroupExclusionList> GroupExclusionListFlaggedRules { get; set; }
    }
}
