using System.Data;
using System.Data.Common;

/*
 * @Author: KingWJC
 * @Date: 2020-06-01 15:24:53
 * @LastEditors: KingWJC
 * @LastEditTime: 2020-06-04 16:03:57
 * @Descripttion: 数据库操作接口
 * @FilePath: \ADFCommon\ADF.DataAccess\IDbHelper.cs
 */
namespace ADF.DataAccess
{
    interface IDbHelper
    {
        /**
         * @description: 执行命令，返回受影响的行数
         * @param {string} strSQL SQL语句
         * @return: 受影响的行数
         */
        int ExecuteNonQueryUseTrans(string strSQL, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text);

        // /**
        //  * @description: 执行命令，返回首行首列值
        //  * @param {string} strSQL SQL语句
        //  * @return: 首行首列值
        //  */
        // object ExecuteScalar(string strSQL, DbParameter[] parameters = null);

        // /**
        //  * @description: 执行命令，返回数据集
        //  * @param {string} strSQL SQL语句
        //  * @return: 数据集
        //  */
        // DataSet ExecuteDataSet(string strSQL, DbParameter[] parameters = null);

        // /**
        //  * @description: 执行命令，返回数据表
        //  * @param {string} strSQL SQL语句
        //  * @return: 数据表
        //  */
        // DataTable ExecuteDataTable(string strSQL, DbParameter[] parameters = null);
    }
}