using System;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace StaffManagement.Models
{
    public partial class TitleModel
    {

        public TitleModel()
        {


        }

        public int TitleId { get; set; }
        public Nullable<int> ParentTitleId { get; set; }

        [Display(Name = "Title")]
        public string Text { get; set; }

        public string PrefixText { get; set; }
        public bool Active { get; set; }


        public bool Selected { get; set; }

        public int TitleCount { get; set; }





    }
}
