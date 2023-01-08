using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class ContractAdditionalUnit
    {
        public int ContractorId { get; set; }
        public int ContractorJobId { get; set; }
        public int UnitId { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }

        public virtual Contract Contractor { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
