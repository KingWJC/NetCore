using System;

namespace testconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine(Test());
        }

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
                Console.WriteLine("3");

                stringBuilder.Append("\nprint Finally!");

                //error: 控制不能离开 finally 子句主体
                //return stringBuilder.ToString();
            }
            // return stringBuilder.ToString();
        }

        static int Test()
        {
            int num = 0;
            try
            {
                num = 1;
                throw new Exception("手动控制抛出异常");
                // return num;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                num++;
                // return num;
            }
            finally
            {
                num++;
                Console.WriteLine("我在Finally中num={0}", num);
            }
            return num;
        }
    }
}
