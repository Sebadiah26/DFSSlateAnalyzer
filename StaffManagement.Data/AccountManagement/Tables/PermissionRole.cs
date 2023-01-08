using System.ComponentModel.DataAnnotations;
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
