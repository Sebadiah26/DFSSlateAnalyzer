using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class UserGroupMembership
    {
        public int SystemId { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string NameAndPath { get; set; }
    }
}
