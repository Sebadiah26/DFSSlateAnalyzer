using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Phone
    {
        public Guid ObjectGuid { get; set; }
        public string Type { get; set; }
        public int RowNumber { get; set; }
        public string Number { get; set; }
        public bool IsDeleted { get; set; }
    }
}
