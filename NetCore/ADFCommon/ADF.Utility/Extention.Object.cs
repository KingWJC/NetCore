using System;
using System.ComponentModel;

namespace ADF.Utility
{
    public static partial class Extention
    {
        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="obj">原对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeType_ByConvert(this object obj, Type targetType)
        {
            object resObj;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter converter = new NullableConverter(targetType);
                resObj = converter.ConvertFrom(obj);
            }
            else
            {
                resObj = Convert.ChangeType(obj, targetType);
            }

            return resObj;
        }

        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
            {
                return true;
            }
            else
            {
                return string.IsNullOrEmpty(obj.ToString());
            }
        }
    }
}