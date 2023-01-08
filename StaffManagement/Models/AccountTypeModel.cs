using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace StaffManagement.Data
{
    /// <summary>
    /// class for the Accounts dbSet in accountmanagementContext.cs
    /// </summary>
    public partial class AccountTypeModel
    {
        public AccountTypeModel()
        {

        }


        [Key]
        public int AccountTypeId { get; set; }

        [Display(Name = "Employee Type")]
        public string Description { get; set; }
        public bool IsStaff { get; set; }

        public SelectList AccountTypes { get; set; }
    }
}
