#nullable disable

namespace StaffManagement.Data
{
    public partial class EmployeeAdditionalUnit
    {
        public int EmployeeId { get; set; }
        public int UnitId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
