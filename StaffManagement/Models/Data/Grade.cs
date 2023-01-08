using System;
using System.Collections.Generic;

#nullable disable

namespace StaffManagement.Data
{
    public partial class Grade
    {
        public Grade()
        {
            Students = new HashSet<Student>();
        }

        public string GradeId { get; set; }
        public string GradeLevel { get; set; }
        public string PasswordPolicy { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
