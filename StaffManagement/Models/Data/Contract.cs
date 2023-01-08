using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Contract
    {
        public Contract()
        {
            ContractAdditionalUnits = new HashSet<ContractAdditionalUnit>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ContractorId { get; set; }
        public int ContractorJobId { get; set; }

        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<DateTime> EndDate { get; set; }
        public string JobTitle { get; set; }

        [DisplayName("Building")]
        public int PrimaryUnitId { get; set; }

        [DisplayName("Last Updated")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime LastUpdate { get; set; }

        [DisplayName("Last Updated User")]
        public string LastUpdateUser { get; set; }

        [DisplayName("Active")]
        public Nullable<bool> IsActive { get; set; }

        [ForeignKey("ContractorId")]
        public virtual Contractor_staff Contractor { get; set; }
        public virtual Unit PrimaryUnit { get; set; }
        public virtual ICollection<ContractAdditionalUnit> ContractAdditionalUnits { get; set; }
    }
}
