namespace StaffManagement.Models
{
    public class GroupAssignmentModel
    {
        public int SystemId { get; set; }
        public string Name { get; set; }
        public int? RuleId { get; set; }
        public byte AssignmentSource { get; set; }
        public bool Active { get; set; }

        // public virtual GroupAssignmentSource AssignmentSourceNavigation { get; set; }
        // public virtual GroupMembershipRule Rule { get; set; }
        // public virtual Account System { get; set; }
    }
}
