using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Building
    {
        public Building()
        {
            Units = new HashSet<Unit>();
        }

        public int BuildingId { get; set; }
        [Display(Name = "Building")]
        public string Name { get; set; }
        public int? SisBuildingId { get; set; }
        public string GradeBand { get; set; }
        public string FsfBuildingId { get; set; }
        public string ShortName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public virtual hr_Building FsfBuilding { get; set; }
        public virtual sis_Building SisBuilding { get; set; }

        //public virtual Employee Employee { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
    }
}
