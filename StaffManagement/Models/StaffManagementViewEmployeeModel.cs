using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models
{
    public class StaffManagementViewEmployeeModel : BaseViewModel
    {
        [Key]
        public int EmployeeId { get; set; }

        public int SystemId { get; set; }

        public string Name { get; set; }

        public string JobTitle { get; set; }

        public string JobCode { get; set; }


        public virtual GroupAssignmentModel GroupAssignmentModel { get; set; }
    }
}
