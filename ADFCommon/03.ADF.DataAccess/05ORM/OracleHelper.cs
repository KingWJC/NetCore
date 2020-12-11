using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using ADF.Utility;

namespace ADF.DataAccess.ORM
{
    public class OracleHelper : DbHelper
    {
        public override string ParaPrefix => ":";

        public OracleHelper(string connectionStr)
        : base(DatabaseType.Oracle, connectionStr)
        {

        }

        public override IDataParameter[] ToIDbDataParameter(CusDbParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            OracleParameter[] result = new OracleParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new OracleParameter();
                sqlParameter.Size = parameter.Size == -1 ? 0 : parameter.Size;
                sqlParameter.ParameterName = parameter.ParameterName;
                // if (sqlParameter.ParameterName[0] == '@')
                // {
                //     sqlParameter.ParameterName = ':' + sqlParameter.ParameterName.Substring(1, sqlParameter.ParameterName.Length - 1);
                // }
                if (this.Command.CommandType == CommandType.StoredProcedure)
                {
                    sqlParameter.ParameterName = sqlParameter.ParameterName.TrimStart(':');
                }

                if (parameter.DbType == System.Data.DbType.Guid)
                {
                    sqlParameter.OracleType = OracleType.VarChar;
                    sqlParameter.Value = sqlParameter.Value.ToStringOrEmpty();
                }
                else if (parameter.DbType == System.Data.DbType.Boolean)
                {
                    sqlParameter.OracleType = OracleType.Int16;
                    if (parameter.Value == DBNull.Value)
                    {
                        parameter.Value = 0;
                    }
                    else
                    {
                        sqlParameter.Value = (bool)parameter.Value ? 1 : 0;
                    }
                }
                else
                {
                    if (parameter.Value != null && parameter.Value.GetType() == Constants.GuidType)
                    {
                        parameter.Value = parameter.Value.ToString();
                    }

                    if (parameter.DbType == System.Data.DbType.Binary)
                    {
                        sqlParameter.OracleType = OracleType.Blob;
                    }
                    else if(parameter.DbType==System.Data.DbType.String && parameter.ParameterName.Contains("CONTEXT"))
                    {
                        sqlParameter.OracleType = OracleType.Clob;
                    }
                    sqlParameter.Value = parameter.Value;
                }
                if (parameter.Direction != 0)
                    sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                ++index;
            }
            return result;
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
            OracleParameter[] parameters = new OracleParameter[] {
                    new OracleParameter("@QueryStr", strSQL),
                    new OracleParameter("@PageSize", pageSize),
                    new OracleParameter("@PageCurrent", pageCurrent),
                    new OracleParameter("@FdShow", fdShow),
                    new OracleParameter("@FdOrder", fdOrder),
                    new OracleParameter("@Rows", OracleType.Int32, 20) };
            parameters[5].Direction = ParameterDirection.Output;
            OracleCommand sqlCommand = Command as OracleCommand;
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
            return dt;
        }
    }
}