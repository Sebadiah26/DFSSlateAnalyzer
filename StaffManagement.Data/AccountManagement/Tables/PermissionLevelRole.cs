using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StaffManagement.Data
{
    public class PermissionLevelRole
    {



        [Key]
        public int PermissionLevelRoleID { get; set; }

        public int PermissionRoleID { get; set; }

        public int PermissionLevelID { get; set; }


        public bool Active { get; set; }


        [ForeignKey("PermissionLevelID")]
        public virtual PermissionLevel PermissionLevel { get; set; }

        [ForeignKey("PermissionRoleID")]
        public virtual PermissionRole PermissionRole { get; set; }


    }
}
