using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class TitleGroup
    {
        public TitleGroup()
        {
            JobRecords = new HashSet<JobRecord>();

        }

        [Key]
        public int TitleGroupId { get; set; }
        [Display(Name = "Title Group")]
        public string Description { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<JobRecord> JobRecords { get; set; }

        [NotMapped]
        public SelectList TitleGroups { get; set; }
    }
}
