using System;

namespace StaffManagement.Models
{
    public class Contractor_staffModel
    {
        public int ContractorId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SuffixName { get; set; }
        public string Nickname { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
