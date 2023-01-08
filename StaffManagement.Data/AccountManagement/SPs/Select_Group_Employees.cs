using System;
#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_Group_Employees
    {
        public Select_Group_Employees()
        {

        }
        public int SystemId { get; set; }

        public Guid ObjectGuid { get; set; }

        public string Name { get; set; }

        public string CommonName { get; set; }

        public string TitleGroup { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Building { get; set; }







    }
}
