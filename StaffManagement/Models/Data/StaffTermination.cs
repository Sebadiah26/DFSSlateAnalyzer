using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class StaffTermination
    {
        public int SystemId { get; set; }
        public Guid? AdobjectGuid { get; set; }
        public int? EmployeeId { get; set; }
        public int? ContractorId { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? DeletionDate { get; set; }
    }
}
