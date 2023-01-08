using System;

namespace StaffManagement.Common
{
    public static class Settings
    {

        public static DateTime BeginDate => new DateTime(1900, 1, 1);

        public static DateTime EndDate => new DateTime(2099, 12, 31);

        public static DateTime SixMonthsAgo => (DateTime.Now).AddMonths(-6);

        public static string Domain => "bsd.k12.de.us";


    }
}
