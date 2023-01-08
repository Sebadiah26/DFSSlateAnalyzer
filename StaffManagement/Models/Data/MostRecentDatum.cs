using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class MostRecentDatum
    {
        public int ContractorId { get; set; }
        public int BestMatchContractorJobId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
