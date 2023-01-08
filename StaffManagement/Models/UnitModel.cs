using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffManagement.Models
{
    public class UnitModel
    {
        public UnitModel()
        {
            Building = new BuildingModel();



        }


        public int UnitId { get; set; }
        [Display(Name = "Building")]
        public int BuildingId { get; set; }
        [Display(Name = "Department")]
        public string Name { get; set; }
        public string ShortName { get; set; }
        [Display(Name = "Department")]
        public string Adname { get; set; }
        public bool UseGenericAdgroups { get; set; }
        public string HomeDirServer { get; set; }
        public string Comment { get; set; }
        public string HrWorkLocationMatch { get; set; }
        public string HrJobcodeMatch { get; set; }
        public int? SisBuildingMatch { get; set; }
        public string PhoneOffice { get; set; }
        public string PhoneFax { get; set; }
        public string PhoneGuidance { get; set; }
        public string PhoneNurse { get; set; }
        [Display(Name = "Building")]
        public string AdgroupName { get; set; }

        public virtual BuildingModel Building { get; set; }



        [NotMapped]
        public SelectList Units { get; set; }



    }
}
