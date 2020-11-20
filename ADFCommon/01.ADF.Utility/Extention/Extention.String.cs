/*
 * @Author: KingWJC
 * @Date: 2020-06-09 09:34:15
 * @LastEditors: KingWJC
 * @LastEditTime: 2020-11-16 16:35:19
 * @Descripttion: 
 * @FilePath: \ADFCommon\01.ADF.Utility\Extention\Extention.String.cs
 */
using System;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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

        public static bool IsMatch(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        /*
         * @description: json字符串转为DataTable
         * @param {Json字符串}
         * @return {*}
         */
        public static DataTable ToDataTable(this string jsonStr)
        {
            return jsonStr.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<DataTable>(jsonStr);
        }
    }
}