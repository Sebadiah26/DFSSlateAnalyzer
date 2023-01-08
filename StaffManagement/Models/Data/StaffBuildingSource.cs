using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class StaffBuildingSource
    {
        public string StaffId { get; set; }
        public int SisBuildingId { get; set; }
        public int IsPrimaryBuilding { get; set; }
        public int IsCounselor { get; set; }
        public int IsTeacher { get; set; }
        public int IsAdvisor { get; set; }
    }
}
