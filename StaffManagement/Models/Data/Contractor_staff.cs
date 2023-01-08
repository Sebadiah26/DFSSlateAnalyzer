using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Contractor_staff
    {
      //  public Contractor_staff()
       // {
      //     Contracts = new HashSet<Contract>();
      //  }
        [Key]
        public int ContractorId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string LastName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FirstName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MiddleName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SuffixName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Nickname { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public DateTime LastUpdate { get; set; }

        
      //  public virtual ICollection<Contract> Contracts { get; set; }
    }
}
