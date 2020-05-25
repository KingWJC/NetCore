using System;

namespace testconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string str = TestTryCatch();
            Console.WriteLine("\nResult:" + str);
        }

        /**
         * @description: Try-Catch-Finilly 中return的用法
         * @param {type} 
         * @return: string
         */
        public static string TestTryCatch()
        {
            var stringBuilder = new System.Text.StringBuilder();
            try
            {
                Console.WriteLine("1");

                stringBuilder.Append("\nprint Try!");

                // throw new Exception();

                return stringBuilder.ToString();
            }
            catch
            {
                Console.WriteLine("2");

                stringBuilder.Append("\nprint Catch!");

                return stringBuilder.ToString();
            }
            finally
            {
                /*finally不管什么情况都会执行，包括try catch 里面用了return 的情况。
                如果try catch 里面用了return，则finally的执行不影响返回结果。
                 一般用于数据库连接的关闭*/
                Console.WriteLine("3");

                stringBuilder.Append("\nprint Finally!");

                Console.Write("Finally:" + stringBuilder.ToString());

                //error: 控制不能离开 finally 子句主体
                //return stringBuilder.ToString();
            }

            //无法访问的代码
            // return stringBuilder.ToString();
        }
    }
}
