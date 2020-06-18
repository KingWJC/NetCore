using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using ADF.Utility;
using System.Threading.Tasks;

namespace ADF.DataAccess.SimpleFactory
{
    public class SqlserverHelper : IDbHelper
    {

        private string _connectionStr;
        private int _bulkCount = 50000;

        private SqlConnection _connection;
        /*
         * @description: 获取数据库链接
         * @param {type} 
         * @return: 
         */
        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_connectionStr);
                }
                else if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                else if (_connection.State == ConnectionState.Broken)
                {
                    _connection.Close();
                    _connection.Open();
                }

                return _connection;

            }
        }

        private SqlserverHelper() { }

        public SqlserverHelper(string connStr)
        {
            this._connectionStr = connStr;
        }

         #region 私有方法
        private SqlParameter[] GetSqlParamArr(CusDbParameter[] commParamArr)
        {
            SqlParameter[] paramArr = new SqlParameter[commParamArr.Length];
            int nRow = 0;
            foreach (CusDbParameter commParam in commParamArr)
            {
                SqlParameter param = new SqlParameter();
                param.ParameterName = commParam.ParameterName;
                param.Value = commParam.Value;
                // 修改nvarchar到varchar编码问题，底层在进行默认NVarchar  造成没法设置varchar
                if (!commParam.DbType.Equals(DbType.AnsiString) || commParam.Value is string)
                    param.DbType = commParam.DbType;
                if (commParam.Size > 0)
                    param.Size = commParam.Size;
                paramArr[nRow] = param;
                nRow++;
            }
            return paramArr;
        }

        private SqlCommand CreateCommand(SqlConnection connect, string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text, SqlTransaction transaction = null, int timeOut = 600)
        {
            SqlCommand sqlCommand = connect.CreateCommand();
            sqlCommand.CommandText = strSQL;
            sqlCommand.CommandType = commandType;
            sqlCommand.CommandTimeout = timeOut;
            if (parameters?.Length > 0)
            {
                sqlCommand.Parameters.AddRange(GetSqlParamArr(parameters));
            }
            if (transaction != null)
            {
                sqlCommand.Transaction = transaction;
            }
            return sqlCommand;
        }
        #endregion

        #region 数据更新
        /*
         * @description: 获取影响的行数
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQuery(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            using (SqlCommand sqlCommand = CreateCommand(connect, strSQL, parameters, commandType))
            {
                return sqlCommand.ExecuteNonQuery();
            }
        }
        public int ExecuteNonQuery(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            using (SqlConnection connect = Connection)
            {
                using (SqlCommand sqlCommand = CreateCommand(connect, string.Empty))
                {
                    int result = 0;
                    foreach (var item in sqlDict)
                    {
                        sqlCommand.Parameters.Clear();
                        sqlCommand.CommandText = item.Key;
                        if (item.Value != null && item.Value.Length > 0)
                            sqlCommand.Parameters.AddRange(GetSqlParamArr(item.Value));
                        result += sqlCommand.ExecuteNonQuery();
                    }
                    return result;
                }
            }
        }
        /*
         * @description: 使用事务获取影响的行数
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQueryUseTrans(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            {
                SqlTransaction sqlTransaction = connect.BeginTransaction();
                using (SqlCommand sqlCommand = CreateCommand(connect, string.Empty, parameters, commandType))
                {
                    int result = 0;
                    try
                    {
                        result = sqlCommand.ExecuteNonQuery();
                        sqlTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        throw (ex);
                    }
                    return result;
                }
            }
        }
        /*
         * @description: 使用事务获取影响的行数-多条语句
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQueryUseTrans(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            using (SqlConnection connect = Connection)
            {
                SqlTransaction sqlTransaction = connect.BeginTransaction();

                using (SqlCommand sqlCommand = CreateCommand(connect, string.Empty, null, CommandType.Text, sqlTransaction))
                {
                    int result = 0;
                    try
                    {
                        foreach (var item in sqlDict)
                        {
                            sqlCommand.Parameters.Clear();
                            sqlCommand.CommandText = item.Key;
                            if (item.Value != null && item.Value.Length > 0)
                                sqlCommand.Parameters.AddRange(GetSqlParamArr(item.Value));
                            result += sqlCommand.ExecuteNonQuery();
                        }

                        sqlTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        throw (ex);
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60)
        {
            using (SqlBulkCopy BulkCopy = new SqlBulkCopy(_connectionStr))
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
        /// 批量插入(事务，自定义列，不触发约束和触发器)
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">列的对映关系</param>       
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60)
        {
            using (SqlConnection connect = Connection)
            {
                SqlTransaction transaction = connect.BeginTransaction();
                //不执行触发器和约束 
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connect, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, transaction))
                {
                    bulkCopy.DestinationTableName = destTableName;
                    bulkCopy.BulkCopyTimeout = timeOut;
                    bulkCopy.BatchSize = copyData.Rows.Count > _bulkCount ? _bulkCount : copyData.Rows.Count;

                    try
                    {
                        if (columns != null && columns.Length > 1)
                        {
                            for (int i = 0; i < columns[0].Length; i++)
                            {
                                SqlBulkCopyColumnMapping scc = new SqlBulkCopyColumnMapping(columns[0][i].Trim(), columns[1][i].Trim());
                                bulkCopy.ColumnMappings.Add(scc);
                            }
                        }
                        bulkCopy.WriteToServer(copyData);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }

                }
            }
        }
        #endregion

        #region 数据获取
        /*
         * @description: 获取数据量总数
         * @param {type} 
         * @return: 
         */
        public int ExecuteCount(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            using (SqlCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                int result = 0;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                return result;
            }
        }
        /*
         * @description: 获取首行首列
         * @param {type} 
         * @return: 
         */
        public object ExecuteScalar(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            using (SqlCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                object result = command.ExecuteScalar();
                if (object.Equals(null, result) || object.Equals(DBNull.Value, result))
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }
        /*
         * @description: 获取数据集
         * @param {type} 
         * @return: 
         */
        public DataSet ExecuteDataSet(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            using (SqlCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                return dataSet;
            }
        }
        /*
         * @description: 获取数据表
         * @param {type} 
         * @return: 
         */
        public DataTable ExecuteDataTable(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            using (SqlCommand command = CreateCommand(connect, strSQL, parameters))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }
        /*
         * @description: 获取数据表-并行-In条件
         * @param {type} 
         * @return: 
         */
        public DataTable ExecuteDataTableParallel<T>(string strSQL, List<T> wheres)
        {
            DataTable tables = new DataTable();
            if (null == wheres && 0 == wheres.Count)
            {
                throw new Exception();
            }

            if (100 > wheres.Count)
            {
                string bodySql = string.Format("{0} {1}", strSQL, wheres.TryToWhere());
                return ExecuteDataTable(bodySql);
            }
            else
            {
                var batchValue = wheres.TryToBatchValue();
                Parallel.ForEach(batchValue, values =>
                {
                    string sqlBody = string.Format("{0} {1}", strSQL, values);
                    DataTable table = ExecuteDataTable(sqlBody);
                    tables.Merge(table, true);
                });
            }
            return tables;
        }
        /*
         * @description: 获取数据表-分页
         * @param {type} 
         * @return: 
         */
        public DataTable ExecuteDataTable(string strSQL, int CurrentPage, int PageSize, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connect = Connection)
            {
                using (SqlCommand Command = CreateCommand(connect, strSQL, parameters, commandType))
                {
                    DataTable DataTable = new DataTable();
                    SqlDataAdapter Adapter = new SqlDataAdapter(Command);
                    Adapter.Fill((CurrentPage - 1) * PageSize, PageSize, DataTable);
                    return DataTable;
                }
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
        public DataTable ExecPageProc(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount)
        {
            DataTable dt = new DataTable();
            totalCount = 0;
            using (SqlConnection connect = Connection)
            {
                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@QueryStr", strSQL),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@PageCurrent", pageCurrent),
                    new SqlParameter("@FdShow", fdShow),
                    new SqlParameter("@FdOrder", fdOrder),
                    new SqlParameter("@Rows", SqlDbType.Int, 20) };
                parameters[5].Direction = ParameterDirection.Output;

                using (SqlCommand sqlCommand = connect.CreateCommand())
                {
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
                }
            }
            return dt;
        }
        #endregion
    }
}