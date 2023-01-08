using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class StaffGroup
    {
        public StaffGroup()
        {
            StaffGroupAssociations = new HashSet<StaffGroupAssociation>();
        }
        [Key]
        public int StaffGroupId { get; set; }

        public Guid ADObjectGuid { get; set; }

        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public virtual Group Group { get; set; }




        [ForeignKey("StaffGroupId")]
        public virtual ICollection<StaffGroupAssociation> StaffGroupAssociations { get; set; }


    }
}
