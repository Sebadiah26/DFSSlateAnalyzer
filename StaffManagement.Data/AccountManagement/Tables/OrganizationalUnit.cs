using System;
using System.ComponentModel.DataAnnotations;


namespace StaffManagement.Data
{
    public partial class OrganizationalUnit
    {
        public OrganizationalUnit()
        {
            // InverseParentOrganizationalUnitGu = new HashSet<OrganizationalUnit>();
            // User = new User();
        }

        [Key]
        public Guid? ObjectGuid { get; set; }
        public string? DistinguishedName { get; set; }
        public string? CanonicalName { get; set; }
        public string? OrganizationalUnitName { get; set; }
        public string? Description { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public Guid? ManagedByGuid { get; set; }
        public Guid? ParentOrganizationalUnitGuid { get; set; }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Group Group { get; set; }


        // public virtual User ManagedByGu { get; set; }
        // public virtual OrganizationalUnit ParentOrganizationalUnitGu { get; set; }
        //  public virtual ICollection<OrganizationalUnit> InverseParentOrganizationalUnitGu { get; set; }
        public virtual User User { get; set; }
    }
}
