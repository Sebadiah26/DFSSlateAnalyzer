using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Data
{
    public class StaffManagementView_Permission
    {
        [Key]
        public int ViewPermissionID { get; set; }
        public int ViewID { get; set; }

        public int PermissionRoleID { get; set; }

        public bool Active { get; set; }

       
    }
}
