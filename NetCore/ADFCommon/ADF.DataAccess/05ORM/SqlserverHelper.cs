using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ADF.DataAccess.ORM
{
    public class SqlserverHelper : DbHelper
    {
        private static int _bulkCount = 50000;

        public SqlserverHelper(string connectionStr)
        : base(DatabaseType.SqlServer, connectionStr)
        {

        }

        public override DbParameter CreateDbParameter(CusDbParameter commParam)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = commParam.ParameterName;
            param.Value = commParam.Value;
            // 修改nvarchar到varchar编码问题，底层在进行默认NVarchar  造成没法设置varchar
            if (!commParam.DbType.Equals(DbType.AnsiString) || commParam.Value is string)
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
            using (SqlBulkCopy BulkCopy = new SqlBulkCopy(Connection as SqlConnection))
            {
                BulkCopy.BulkCopyTimeout = timeOut;
                BulkCopy.DestinationTableName = destTableName;
                for (int i = 0; i < copyData.Columns.Count; i++)
                {
                    BulkCopy.ColumnMappings.Add(copyData.Columns[i].ColumnName, copyData.Columns[i].ColumnName);
                }
                BulkCopy.WriteToServer(copyData);
            }
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

            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@QueryStr", strSQL),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@PageCurrent", pageCurrent),
                    new SqlParameter("@FdShow", fdShow),
                    new SqlParameter("@FdOrder", fdOrder),
                    new SqlParameter("@Rows", SqlDbType.Int, 20) };
            parameters[5].Direction = ParameterDirection.Output;

            SqlCommand sqlCommand = Command as SqlCommand;
            sqlCommand.CommandText = "[dbo].[PagerShow]";
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandTimeout = 5 * 60;
            if (parameters?.Length > 0)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlCommand))
            {
                int result = sda.Fill(dt);
                object val = sqlCommand.Parameters["@Rows"].Value;
                if (val != null)
                {
                    totalCount = Convert.ToInt32(val);
                }
            }
            return dt;
        }
    }
}