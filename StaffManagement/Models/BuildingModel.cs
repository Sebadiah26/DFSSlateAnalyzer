using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Models
{
    public partial class BuildingModel
    {
        public BuildingModel()
        {
            Units = new HashSet<UnitModel>();
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


        public virtual ICollection<UnitModel> Units { get; set; }

        [NotMapped]
        public SelectList Buildings { get; set; }

        [NotMapped]
        public SelectList AllBuildings { get; set; }


    }
}
