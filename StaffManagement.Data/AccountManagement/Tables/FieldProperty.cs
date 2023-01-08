#nullable disable

namespace StaffManagement.Data
{
    public partial class FieldProperty
    {
        public int GroupId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string TableColumn { get; set; }
        public string StaticValue { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsList { get; set; }
        public bool IsDeleteFlag { get; set; }

        public virtual FieldGroup Group { get; set; }
    }
}
