using System;


namespace StaffManagement.Common
{
    public static class Extensions
    {

        public static string RemoveNull(this string value)
        {
            return value ?? "";
        }

        public static DateTime RemoveNull(this DateTime value)
        {

            return value != null ? value : DateTime.Now;
        }

        public static Boolean RemoveNull(this Boolean value, Boolean ifNull = false)
        {
            return value != null ? value : ifNull;
        }

        public static Boolean? RemoveNull(this Boolean? value, Boolean ifNull = false)
        {
            return value != null ? value : ifNull;
        }

        public static int RemoveNull(this int value)
        {

            return value != null ? value : 0;
        }

        public static int? RemoveNull(this int? value)
        {

            return value != null ? value : 0;
        }
    }
}
