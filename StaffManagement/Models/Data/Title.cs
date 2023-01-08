using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Title
    {

        public Title()
        {
            JobRecords = new HashSet<JobRecord>();

        }
        [Key]
        public int TitleId { get; set; }
        public Nullable<int> ParentTitleId { get; set; }
        [Display(Name = "Title")]
        public string Text { get; set; }
       
        public string PrefixText { get; set; }
        public bool Active { get; set; }

        
        public virtual ICollection<JobRecord> JobRecords { get; set; }


       [NotMapped]
       public SelectList Titles { get; set; }


        [NotMapped]
        public SelectList AllTitles { get; set; }




    }
}
