using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_Available_JobTitles
    {
        
       
        public string JobTitle { get; set; }
        public string AdditionalText { get; set; }
        public string SortCoding { get; set; }

        public int jobcode { get; set; }

        public string personnel_code { get; set; }

        //public List<SelectListItem> JobTitles { set; get; }
      
      
       

       
    }
}
