using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models
{
    public class StaffManagementViewModel : BaseViewModel
    {
        [Key]
        public int ViewID { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }


        public int SortOrder { get; set; }
    }
}
