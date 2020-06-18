using System.Data;
using System.Data.Common;
using System.Collections.Generic;

/*
 * @Author: KingWJC
 * @Date: 2020-06-01 15:24:53
 * @LastEditors: KingWJC
 * @LastEditTime: 2020-06-15 18:01:37
 * @Descripttion: 数据库操作接口
 * @FilePath: \ADFCommon\ADF.DataAccess\SimpleFactory\IDbHelper.cs
 */
namespace ADF.DataAccess.SimpleFactory
{
    public interface IDbHelper
    {
        #region 查询数据

        /*
         * @description: 获取数据量总数
         * @param {type} 
         * @return: 
         */
        int ExecuteCount(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 获取首行首列
         * @param {type} 
         * @return: 
         */
        object ExecuteScalar(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 获取数据集
         * @param {type} 
         * @return: 
         */
        DataSet ExecuteDataSet(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 获取数据表
         * @param {type} 
         * @return: 
         */
        DataTable ExecuteDataTable(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 获取数据表-并行-In条件
         * @param {type} 
         * @return: 
         */
        DataTable ExecuteDataTableParallel<T>(string strSQL, List<T> wheres);
        /*
         * @description: 获取数据表-分页
         * @param {type} 
         * @return: 
         */
        DataTable ExecuteDataTable(string strSQL, int CurrentPage, int PageSize,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);

        /// <summary>
        /// 执行分页存储过程
        /// </summary>
        /// <param name="strSQL">表名、视图名、查询语句</param>
        /// <param name="pageSize">每页的大小(默认10)</param>
        /// <param name="pageCurrent">要显示的页</param>
        /// <param name="fdShow">要显示的字段列表(为空则查询出所有字段）</param>
        /// <param name="fdOrder">排序字段列表(多个字段之间用逗号分割，可为空)</param>
        /// <param name="totalCount">输出记录数</param>
        /// <returns>DataTable</returns>
        DataTable ExecPageProc(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount);

        #endregion

        #region 更新数据
        /**
         * @description: 执行命令，返回受影响的行数
         * @param {string} strSQL SQL语句
         * @return: 受影响的行数
         */
        int ExecuteNonQuery(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 获取影响的行数-多条语句
         * @param {type} 
         * @return: 
         */
        int ExecuteNonQuery(Dictionary<string,  CusDbParameter[]> sqlDict);
        /*
         * @description: 使用事务获取影响的行数
         * @param {type} 
         * @return: 
         */
        int ExecuteNonQueryUseTrans(string strSQL,  CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text);
        /*
         * @description: 使用事务获取影响的行数-多条语句
         * @param {type} 
         * @return: 
         */
        int ExecuteNonQueryUseTrans(Dictionary<string,  CusDbParameter[]> sqlDict);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60);

        /// <summary>
        /// 批量插入(事务，自定义列，不触发约束和触发器)
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">列的对映关系</param>       
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60);
        #endregion
    }
}