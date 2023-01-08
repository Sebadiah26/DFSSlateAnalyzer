using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffManagement.Data
{
    public class StaffManagementView
    {
        [Key]
        public int ViewID { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
        public int SortOrder { get; set; }

        [ForeignKey("ViewID")]
        public virtual StaffManagementView_Permission StaffManagementView_Permission { get; set; }

        [ForeignKey("ViewID")]
        public virtual StaffManagementView_Unit StaffManagementView_Unit { get; set; }

    }
}
