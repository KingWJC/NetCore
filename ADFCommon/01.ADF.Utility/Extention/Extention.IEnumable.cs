using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace ADF.Utility
{
    public static partial class Extention
    {
        /// <summary>
        /// 给IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="iEnumberable">数据源</param>
        /// <param name="func">方法</param>
        public static void ForEach<T>(this IEnumerable<T> iEnumberable, Action<T> func)
        {
            foreach (var item in iEnumberable)
            {
                func(item);
            }
        }
        /*
         * @description: 获取链表针对处理器数*2 所划分的条数
         * @param {type} 
         * @return: 
         */
        public static int ReserveProcessor<T>(this IEnumerable<T> source)
        {
            var processCount = Environment.ProcessorCount;
            return source.Count() / (processCount * 2);
        }

        /*
         * @description: 枚举集合转字符串（SQL-IN条件）
         * @param {type} 
         * @return: 
         */
        public static string TryToWhere<T>(this IEnumerable<T> wheres)
        {
            return new
                   StringBuilder("('" + string.Join("','", wheres) + "')").ToString();
        }

        /*
         * @description: 枚举集合转字符串集合（SQL-IN条件）
         * @param {type} 
         * @return: 
         */
        public static List<string> TryToBatchValue<T>(this IEnumerable<T> bucket)
        {
            var proce = bucket.ReserveProcessor();
            var batch = bucket.Count() / proce;
            var mores = bucket.Count() % proce;
            var sizes = mores == 0
                      ? batch : batch + 1;

            var temps = new List<string>();

            for (int i = 0; i < sizes; i++)
            {
                if (i == sizes - 1 && bucket.Take(mores).Any())
                {
                    temps.Add(bucket.Take(mores).TryToWhere());
                }
                else
                {
                    temps.Add(bucket.Take(proce).TryToWhere());
                    bucket = bucket.Skip(proce);
                }
            }
            return temps;
        }
        /*
         * @description: 判断集合是否为空
         * @param {type} 
         * @return: 
         */
        public static Boolean IsEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            return list.ToJson().ToDataTable();
        }
    }
}
