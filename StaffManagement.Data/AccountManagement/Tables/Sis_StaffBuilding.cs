#nullable disable

namespace StaffManagement.Data
{
    public partial class Sis_StaffBuilding
    {
        public string StaffId { get; set; }
        public int SisBuildingId { get; set; }
        public bool IsPrimaryBuilding { get; set; }
        public bool IsCounselor { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsAdvisor { get; set; }

        public virtual sis_Building SisBuilding { get; set; }
    }
}
