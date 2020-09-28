using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace ADF.DataAccess.MixFactory
{
    public class OracleHelper : DbHelper
    {
        public OracleHelper(string connectionStr)
        : base(DatabaseType.Oracle, connectionStr)
        {

        }

        public override DbParameter CreateDbParameter(CusDbParameter commParam)
        {
            OracleParameter param = new OracleParameter();
            string paramName = commParam.ParameterName;
            if (commParam.ParameterName.Contains("@"))
            {
                paramName = commParam.ParameterName.Replace("@", ":");
            }
            param.ParameterName = paramName;
            param.Value = commParam.Value;
            if (!commParam.DbType.Equals(DbType.AnsiString))
                param.DbType = commParam.DbType;
            if (commParam.Size > 0)
                param.Size = commParam.Size;

            return param;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public override void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60)
        {
            throw new Exception("目前不支持");
        }

        /// <summary>
        /// 批量插入(事务，自定义列，不触发约束和触发器)
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">列的对映关系</param>       
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public override void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60)
        {
            throw new Exception("目前不支持");
        }

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
        public override DataTable ExecPageProc(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount)
        {

            DataTable dt = new DataTable();
            totalCount = 0;
            using (OracleConnection connect = Connection as OracleConnection)
            {
                OracleParameter[] parameters = new OracleParameter[] {
                    new OracleParameter("@QueryStr", strSQL),
                    new OracleParameter("@PageSize", pageSize),
                    new OracleParameter("@PageCurrent", pageCurrent),
                    new OracleParameter("@FdShow", fdShow),
                    new OracleParameter("@FdOrder", fdOrder),
                    new OracleParameter("@Rows", OracleType.Int32, 20) };
                parameters[5].Direction = ParameterDirection.Output;
                using (OracleCommand sqlCommand = connect.CreateCommand())
                {
                    sqlCommand.CommandText = "[dbo].[PagerShow]";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandTimeout = 5 * 60;
                    if (parameters?.Length > 0)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    using (OracleDataAdapter sda = new OracleDataAdapter(sqlCommand))
                    {
                        int result = sda.Fill(dt);
                        object val = sqlCommand.Parameters["@Rows"].Value;
                        if (val != null)
                        {
                            totalCount = Convert.ToInt32(val);
                        }
                    }
                }
            }
            return dt;
        }
    }
}