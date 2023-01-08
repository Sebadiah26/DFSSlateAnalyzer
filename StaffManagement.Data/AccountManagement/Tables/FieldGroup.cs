using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class FieldGroup
    {
        public FieldGroup()
        {
            FieldProperties = new HashSet<FieldProperty>();
        }

        public int GroupId { get; set; }
        public string Name { get; set; }
        public int LoadOrder { get; set; }
        public string ObjectType { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }

        public virtual ICollection<FieldProperty> FieldProperties { get; set; }
    }
}
