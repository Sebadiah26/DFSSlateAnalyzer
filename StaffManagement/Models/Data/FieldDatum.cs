using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class FieldDatum
    {
        public string GroupName { get; set; }
        public int LoadOrder { get; set; }
        public bool? IsList { get; set; }
        public bool IsEnabled { get; set; }
        public string ObjectType { get; set; }
        public string PropertyName { get; set; }
        public string StaticValue { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string TableColumn { get; set; }
        public int? FieldOrder { get; set; }
        public string DatabaseType { get; set; }
        public bool? IsNullable { get; set; }
        public bool? IsKey { get; set; }
        public bool? IsIndexField { get; set; }
        public bool IsListField { get; set; }
        public bool IsDeleteFlag { get; set; }
    }
}
