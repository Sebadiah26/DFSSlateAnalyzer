using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StaffManagement.Data
{
    public class PermissionLevel
    {
        [Key]
        public int PermissionLevelID { get; set; }

        public string PermissionLevelName { get; set; }
        public bool Active { get; set; }

        [ForeignKey("PermissionLevelID")]
        public virtual PermissionLevelRole PermissionLevelRole { get; set; }

    }
}
