using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StaffManagement.Data
{
    public class PermissionRole
    {
       


        [Key]
        public int PermissionRoleID { get; set; }
       
        public string PermissionRoleName { get; set; }
        public bool Active { get; set; }


        public virtual PermissionLevelRole PermissionLevelRole { get; set; }



    }
}
