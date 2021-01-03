using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo
{
    internal class EntityDefault
    {
        internal static readonly int FieldsLength100 = 100;
        internal static readonly int FieldsLength250 = 250;
        internal static readonly int FieldsLength50 = 50;
        internal static readonly int FieldsLength2000 = 2000;
        internal static readonly int FieldsLength20 = 20;
    }


    public static class StringExtend
    {
        public static string TruncateWithPostfix(this string str, int length)
        {
            if (str.Length > length)
            {
                return str.Substring(0, length);
            }
            else
                return str;
        }
    }
}
