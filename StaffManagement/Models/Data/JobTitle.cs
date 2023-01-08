using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class JobTitle
    {
        public int EntryId { get; set; }
        public string PersonnelCode { get; set; }
        public int? Jobcode { get; set; }
        public string PersonnelCodeText { get; set; }
        public string JobtitleText { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string SortCoding { get; set; }
    }
}
