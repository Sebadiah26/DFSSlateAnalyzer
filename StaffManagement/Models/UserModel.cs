using System;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models
{
    public class UserModel
    {

        public UserModel()
        {
            OrganizationalUnit = new OrganizationalUnitModel();



        }



        [Key]
        public Guid ObjectGuid { get; set; }
        public string DistinguishedName { get; set; }
        public string UserPrincipalName { get; set; }

        [Display(Name = "Computer Logon")]
        public string SamaccountName { get; set; }
        public bool IsDisabled { get; set; }

        [Display(Name = "Email Logon")]
        public string Mail { get; set; }
        public string Name { get; set; }
        public string CommonName { get; set; }
        public string DisplayName { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Initials { get; set; }
        public string MailNickname { get; set; }
        public string JobTitle { get; set; }
        public Nullable<Guid> ManagerGuid { get; set; }
        public Nullable<Guid> OrganizationalUnitGuid { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        public string Company { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string OfficeNumber { get; set; }
        public string FaxNumber { get; set; }
        public Nullable<DateTime> AccountExpires { get; set; }
        public string Description { get; set; }
        public string HomeDrive { get; set; }
        public string HomeDirectory { get; set; }
        public Nullable<Guid> PrimaryGroupGuid { get; set; }
        public string AdEmployeeId { get; set; }
        public string AdEmployeeNumber { get; set; }
        public string AdEmployeeType { get; set; }
        public long Usnchanged { get; set; }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool IsDeleted { get; set; }

        public virtual OrganizationalUnitModel OrganizationalUnit { get; set; }
        // [ForeignKey("ObjectGuid")]
        // public AccountModel Account { get; set; }


    }
}
