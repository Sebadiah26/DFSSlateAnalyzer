using System.ComponentModel.DataAnnotations;
#nullable disable

namespace StaffManagement.Models
{
    public partial class TitleGroupModel
    {
        public TitleGroupModel()
        {

        }

        [Key]
        public int TitleGroupId { get; set; }
        [Display(Name = "Title Group")]
        public string Description { get; set; }
        public bool Active { get; set; }

        public bool Selected { get; set; }

    }
}
