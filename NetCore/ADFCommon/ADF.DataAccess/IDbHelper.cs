using System.Data;
using System.Data.SqlClient;

/*
 * @Author: KingWJC
 * @Date: 2020-06-01 15:24:53
 * @LastEditors: KingWJC
 * @LastEditTime: 2020-06-01 16:57:04
 * @Descripttion: 数据库操作接口
 * @FilePath: \ADFCommon\ADF.DataAccess\IDbHelper.cs
 */
namespace ADF.DataAccess
{
    //接口的方法和属性都是公共的,(在 C# 7.3 中，修饰符 "public" 对此项无效).
     interface IDbHelper
    {
        /**
         * @description: 执行命令，返回受影响的行数
         * @param {string} strSQL SQL语句
         * @return: 受影响的行数
         */
         int ExecuteNonQuery(string strSQL);

        /**
         * @description: 执行命令，返回首行首列值
         * @param {string} strSQL SQL语句
         * @return: 首行首列值
         */
         object ExecuteSingel(string strSQL);

        /**
         * @description: 执行命令，返回数据集
         * @param {string} strSQL SQL语句
         * @return: 数据集
         */
         DataSet ExecteDataSet(string strSQL);

        /**
         * @description: 执行命令，返回数据表
         * @param {string} strSQL SQL语句
         * @return: 数据表
         */
         DataTable ExecteDataTable(string strSQL);
    }
}