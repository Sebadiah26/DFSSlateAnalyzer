using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_StaffManagementView_Employees
    {
        public Select_StaffManagementView_Employees()
        {
         
        }
        public int SystemId { get; set; }
        public int EmployeeId { get; set; }
        public string ViewName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int UnitId { get; set; }
        public string Building { get; set; }
        public int jobcode { get; set; }
       
        
        public string JobTitle { get; set; }
      

      
    }
}
