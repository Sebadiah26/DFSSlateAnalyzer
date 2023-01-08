using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace StaffManagement.Models
{
    public partial class StaffGroupModel
    {
        public StaffGroupModel()
        {
            // Group = new GroupModel();
            StaffGroupAssociations = new List<StaffGroupAssociationModel>();
        }

        [Key]
        public int StaffGroupId { get; set; }

        public Guid ADObjectGuid { get; set; }

        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        //[ForeignKey("ObjectGuid")]
        ///public virtual GroupModel Group{ get; set; }

        public virtual List<StaffGroupAssociationModel> StaffGroupAssociations { get; set; }

        public int EmployeeCount { get; set; }




    }
}
