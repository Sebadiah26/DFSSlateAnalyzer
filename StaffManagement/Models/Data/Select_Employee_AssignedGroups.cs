using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_Employee_AssignedGroups
    {
        public Select_Employee_AssignedGroups()
        {
         
        }
        public int SystemId { get; set; }
        public string OrganizationalUnitName { get; set; }
        public string GroupName  { get; set; }

        public int StaffGroupId { get; set; }

        public int StaffGroupAssociationId { get; set; }




        //public byte AssignmentSourceID { get; set; }
        //public string AssignmentSource { get; set; }
        //public string Rule { get; set; }

        //public string RuleSQL { get; set; }



    }
}
