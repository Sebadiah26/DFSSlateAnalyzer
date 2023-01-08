using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Models
{
    public partial class StaffGroupAssociationModel
    {


        public StaffGroupAssociationModel()
        {

            Unit = new UnitModel();
            Building = new BuildingModel();
            TitleGroup = new TitleGroup();
            Title = new Title();
            Employees = new List<EmployeeModel>();
            // StaffGroup = new StaffGroupModel();

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

        public int EmployeeCount { get; set; }

        [Display(Name = "Department")]
        public virtual UnitModel Unit { get; set; }

        [Display(Name = "Building")]
        public virtual BuildingModel Building { get; set; }

        [ForeignKey("TitleGroupId")]
        public virtual TitleGroup TitleGroup { get; set; }
        [ForeignKey("TitleId")]
        public virtual Title Title { get; set; }

        //  public virtual StaffGroupModel StaffGroup { get; set; }

        public virtual List<EmployeeModel> Employees { get; set; }

        public bool GroupHeader { get; set; }

        public int PreviousGroupCount { get; set; }

        public SelectList CalculatedMatches { get; set; }




    }
}
