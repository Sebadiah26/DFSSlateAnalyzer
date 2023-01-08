using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class ExchangeEmail
    {
        public Guid ObjectGuid { get; set; }
        public int RowNumber { get; set; }
        public string ValueRaw { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
