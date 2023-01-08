using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffManagement.Models
{
    public class ContractModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ContractorId { get; set; }
        public int ContractorJobId { get; set; }
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string JobTitle { get; set; }
        public int PrimaryUnitId { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public virtual Contractor_staffModel Contractor { get; set; }
        public virtual UnitModel PrimaryUnit { get; set; }
    }
}
