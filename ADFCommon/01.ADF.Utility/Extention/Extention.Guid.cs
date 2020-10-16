using System;

namespace ADF.Utility
{
    public partial class Extention
    {
        /// <summary>
        /// 转为有序的GUID
        /// 注：长度为50字符
        /// </summary>
        /// <param name="guid">新的GUID</param>
        /// <returns></returns>
        public static string ToSequentialGuid(this Guid guid)
        {
            string timeStr = (DateTime.Now.ToCstTime().Ticks / 1000).ToString("x8");
            string newGuid = $"{timeStr.PadLeft(13, '0')}-{guid}";
            return newGuid;
        }
    }
}