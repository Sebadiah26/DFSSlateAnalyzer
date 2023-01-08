using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace StaffManagement.Data
{
    public partial class Group
    {
        public Group()
        {
            GroupMemberships = new HashSet<GroupMembership>();
            // Users = new HashSet<User>();
            // OrganizationalUnit = new OrganizationalUnit();
        }

        [Key]
        public Guid? ObjectGuid { get; set; }
        public string? DistinguishedName { get; set; }
        public string? SamaccountName { get; set; }
        public string? Mail { get; set; }
        public string? MailNickname { get; set; }
        public string? Name { get; set; }
        public string? CommonName { get; set; }
        public string? Notes { get; set; }
        public Guid? ManagedByGuid { get; set; }

        public Guid? OrganizationalUnitGuid { get; set; }
        public string? Description { get; set; }
        public bool? IsSecurityGroup { get; set; }
        public string? ExtensionAttribute2 { get; set; }
        public long Usnchanged { get; set; }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
        // public virtual ICollection<User> Users { get; set; }


        public virtual StaffGroup StaffGroup { get; set; }



        public virtual OrganizationalUnit OrganizationalUnit { get; set; }
    }
}
