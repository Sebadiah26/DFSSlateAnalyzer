using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class NeedsReview
    {
      
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NeedsReviewId { get; set; }

        public int NeedsReviewTypeId { get; set; }

        [Display(Name = "Needs Review")]
        public Nullable<bool> needsReview { get; set; }
        public string ReferenceTable { get; set; }
        public int ReferenceId { get; set; }

       
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }

        public Nullable<DateTime> LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }

        public string Comment { get; set; }

        public virtual Employee Employee { get; set; }


    }
}
