using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Data
{
    public class StaffManagementView_Unit
    {
        [Key]
        public int ViewUnitID { get; set; }
        public int ViewID { get; set; }
        public int UnitID { get; set; }
        public int ViewFilterTypeID { get; set; }


        public bool Active { get; set; }

       
    }
}
