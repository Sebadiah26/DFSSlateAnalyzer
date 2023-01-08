using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_StaffGroupAssociation_Employees
    {
        public Select_StaffGroupAssociation_Employees()
        {
         
        }
        public int SystemId { get; set; }
     
        public string Name { get; set; }
        //public string TitleGroup { get; set; }
        //public string Title { get; set; }

        //public string Department { get; set; }
        //public string Building { get; set; }
       
        public string Matches { get; set; }
      

      
    }
}
