
using System;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Sis_Staff
    {

        public string StaffId { get; set; }
        public string Email { get; set; }
        public string ImsuserRecId { get; set; }


        private int? _employeeId;


        public int? EmployeeId

        {

            get
            {
                if (this.StaffId == null)
                { _employeeId = 0; }
                else
                { _employeeId = Convert.ToInt32(this.StaffId); }

                return _employeeId;
            }

            set
            { _employeeId = (int)value; }
        }


    }
}
