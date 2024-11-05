
using System.Globalization;

namespace PBT.Services
{
    public static class Jfunc
    {
        public static string Cstring(string value)
        {
            if (value == null)
                return "''";
            else
                return "'" + value.Replace("'", "''") + "'";
        }

        public static string Tstring(string value)
        {
            if (value == null)
                return "";
            else
                return value.Replace("'", "''");
        }


        public static string DBdate(DateTime? date)
        {
            if (date == null)
                return null;
            else
                return "'" + Convert.ToDateTime(date).ToString("yyyy-MM-dd") + "'";
        }

        //---------------------------------------------------------------
        // Get title case of a string (every word with leading upper case,
        //                             the rest is lower case)
        //    i.e: ABCD EFG -> Abcd Efg,
        //         john doe -> John Doe,
        //         miXEd CaSING - > Mixed Casing
        //---------------------------------------------------------------
        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
