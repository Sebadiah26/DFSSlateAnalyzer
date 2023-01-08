using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class sis_Building
    {
        public sis_Building()
        {
            Buildings = new HashSet<Building>();
            StaffBuilding1s = new HashSet<Sis_StaffBuilding>();
            Students = new HashSet<Student>();
        }

        public int SisBuildingId { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<Sis_StaffBuilding> StaffBuilding1s { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
