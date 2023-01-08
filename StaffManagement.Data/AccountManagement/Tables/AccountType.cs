using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    /// <summary>
    /// class for the Accounts dbSet in accountmanagementContext.cs
    /// </summary>
    public partial class AccountType
    {
        public AccountType()
        {

        }


        [Key]
        public int AccountTypeId { get; set; }

        public string Description { get; set; }
        public bool IsStaff { get; set; }

        [NotMapped]
        public SelectList AccountTypes { get; set; }
    }
}
