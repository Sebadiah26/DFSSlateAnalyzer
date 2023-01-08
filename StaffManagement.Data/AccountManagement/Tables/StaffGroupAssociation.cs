using System;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace StaffManagement.Data
{
    public partial class StaffGroupAssociation
    {


        public StaffGroupAssociation()
        {

        }


        [Key]
        public int StaffGroupAssociationId { get; set; }
        public int StaffGroupId { get; set; }

        [Display(Name = "Department")]
        public Nullable<int> UnitId { get; set; }

        [Display(Name = "Building")]
        public Nullable<int> BuildingId { get; set; }

        [Display(Name = "Title Group")]
        public Nullable<int> TitleGroupId { get; set; }

        [Display(Name = "Title")]
        public Nullable<int> TitleId { get; set; }

        public bool OnlyDirectHire { get; set; }


        [Display(Name = "Department")]
        public virtual Unit Unit { get; set; }

        [Display(Name = "Building")]
        public virtual Building Building { get; set; }


        public virtual TitleGroup TitleGroup { get; set; }

        public virtual Title Title { get; set; }


        public virtual StaffGroup StaffGroup { get; set; }







    }
}
