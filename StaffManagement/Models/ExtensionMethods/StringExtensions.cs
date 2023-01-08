using System;
using System.Linq;


namespace StaffManagement.Models.ExtensionMethods
{
    public static class StringExtensions
    {

        public static string RemoveWhitespace(this string input)
        {
            if (input != null)
            {
                return new string(input.ToCharArray()
                    .Where(c => !Char.IsWhiteSpace(c))
                    .ToArray());
            }
            else
                return new string("");
        }
    }
}
