#nullable disable

namespace StaffManagement.Data
{
    public partial class Jobcode
    {
        public Jobcode()
        {

        }

        public int JobcodeId { get; set; }
        public string OfficialDescription { get; set; }
        public string BsdDescription { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }

        public virtual JobRecord JobRecord { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual EmployeeJob EmployeeJob { get; set; }
    }
}
