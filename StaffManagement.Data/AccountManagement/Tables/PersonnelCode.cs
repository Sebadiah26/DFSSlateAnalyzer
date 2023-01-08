#nullable disable

namespace StaffManagement.Data
{
    public partial class PersonnelCode
    {
        public PersonnelCode()
        {

        }

        public string? Personnel_Code { get; set; }
        public string Description { get; set; }


        // public virtual JobRecord JobRecord { get; set; }
        public virtual Employee Employee { get; set; }

        public virtual EmployeeJob EmployeeJob { get; set; }

    }
}
