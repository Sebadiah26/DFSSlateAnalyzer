using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class UnitBuilding
    {
        
        public int UnitId { get; set; }
        
        public int BuildingId { get; set; }
        public bool Active { get; set; }

        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        [ForeignKey("BuildingId")]
        public virtual Building Building { get; set; }

    }
}
