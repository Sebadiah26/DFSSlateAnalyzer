using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class hr_Building
    {
        public hr_Building()
        {
            Buildings = new HashSet<Building>();
          
        }
        [Key]
        public string FsfBuildingId { get; set; }
        public string District { get; set; }
        public string WorkLocation { get; set; }
        [Display(Name = "Building")]
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }

        [ForeignKey("FsfBuildingId")]
        public virtual Employee Employee { get; set; }

       

    }
}
