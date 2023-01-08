using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class BuildingSource
    {
        public string FsfBuildingId { get; set; }
        public string District { get; set; }
        public string WorkLocation { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
    }
}
