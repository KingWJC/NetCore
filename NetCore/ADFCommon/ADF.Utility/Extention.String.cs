using System;

namespace ADF.Utility
{
    public static partial class Extention
    {
        public static int ToInt(this string value)
        {
            int result;
            if (Int32.TryParse(value, out result))
                return result;
            else
                return 0;
        }
    }
}