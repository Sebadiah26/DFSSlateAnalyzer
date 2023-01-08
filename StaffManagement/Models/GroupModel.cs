using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Data;
using System;
using System.Collections.Generic;
#nullable disable

namespace StaffManagement.Models
{
    public partial class GroupModel
    {
        public GroupModel()
        {
            GroupMemberships = new HashSet<GroupMembership>();
            Users = new HashSet<User>();
            StaffGroup = new StaffGroupModel();
            OrganizationalUnit = new OrganizationalUnitModel();
        }

        public Guid ObjectGuid { get; set; }
        public string DistinguishedName { get; set; }
        public string SamaccountName { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public string Name { get; set; }
        public string CommonName { get; set; }
        public string Notes { get; set; }
        public Guid? ManagedByGuid { get; set; }
        public Guid? OrganizationalUnitGuid { get; set; }
        public string Description { get; set; }
        public bool IsSecurityGroup { get; set; }
        public string ExtensionAttribute2 { get; set; }
        public long Usnchanged { get; set; }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public virtual StaffGroupModel StaffGroup { get; set; }

        public virtual OrganizationalUnitModel OrganizationalUnit { get; set; }

        public SelectList ActualMatches { get; set; }

        public int ActualMatchesCount { get; set; }


    }
}
