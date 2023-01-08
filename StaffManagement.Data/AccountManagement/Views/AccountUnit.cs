using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class AccountUnit
    {
        [Key]
        public int SystemId { get; set; }

        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        [ForeignKey("SystemId")]
        public virtual Account Account { get; set; }


    }
}
