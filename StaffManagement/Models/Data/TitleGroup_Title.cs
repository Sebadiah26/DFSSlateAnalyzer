using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class TitleGroup_Title
    {
        
        public int TitleGroupId { get; set; }
        
        public int TitleId { get; set; }
        public bool Active { get; set; }

        [ForeignKey("TitleId")]
        public virtual Title Title { get; set; }

        [ForeignKey("TitleGroupId")]
        public virtual TitleGroup TitleGroup { get; set; }

    }
}
