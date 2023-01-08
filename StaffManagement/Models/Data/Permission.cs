using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StaffManagement.Data
{
    public class Permission
    {
        [Key]
        public int PermissionID { get; set; }
        [ForeignKey("SystemId")]
        public int SystemId { get; set; }
        public int PermissionRoleID { get; set; }
        public bool Active { get; set; }

        


        [ForeignKey("SystemId")]
        public virtual Account Account { get; set; }

        [ForeignKey("PermissionRoleID")]
        public virtual PermissionRole PermissionRole { get; set; }

    }
}
