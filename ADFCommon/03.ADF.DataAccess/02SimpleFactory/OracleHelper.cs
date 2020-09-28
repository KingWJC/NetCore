using System;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.SimpleFactory
{
    public class OracleHelper : IDbHelper
    {
        private string _connectionStr;

        private OracleConnection _connection;
        /*
         * @description: 数据库连接
         * @param {type} 
         * @return: 
         */
        public OracleConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new OracleConnection(_connectionStr);
                    _connection.Open();
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

        private OracleHelper() { }

        public OracleHelper(string connStr)
        {
            this._connectionStr = connStr;
        }

        #region 私有方法
        private OracleParameter[] GetOracleParamArr(CusDbParameter[] commParamArr)
        {
            OracleParameter[] paramArr = new OracleParameter[commParamArr.Length];
            int nRow = 0;
            foreach (CusDbParameter commParam in commParamArr)
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
                paramArr[nRow] = param;
                nRow++;
            }
            return paramArr;
        }

        /*
         * @description: 构建Command对象
         * @param {type} 
         * @return: 
         */
        private OracleCommand CreateCommand(OracleConnection connect, string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text, OracleTransaction transaction = null, int timeOut = 600)
        {
            OracleCommand sqlCommand = connect.CreateCommand();
            sqlCommand.CommandText = strSQL;
            sqlCommand.CommandType = commandType;
            sqlCommand.CommandTimeout = timeOut;
            if (parameters?.Length > 0)
            {
                sqlCommand.Parameters.AddRange(GetOracleParamArr(parameters));
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
            using (OracleConnection connect = Connection)
            using (OracleCommand sqlCommand = CreateCommand(connect, strSQL, parameters, commandType))
            {
                return sqlCommand.ExecuteNonQuery();
            }
        }
        public int ExecuteNonQuery(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            using (OracleConnection connect = Connection)
            {
                using (OracleCommand sqlCommand = CreateCommand(connect, string.Empty))
                {
                    int result = 0;
                    foreach (var item in sqlDict)
                    {
                        sqlCommand.Parameters.Clear();
                        sqlCommand.CommandText = item.Key;
                        if (item.Value != null && item.Value.Length > 0)
                            sqlCommand.Parameters.AddRange(GetOracleParamArr(item.Value));
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
            using (OracleConnection connect = Connection)
            {
                OracleTransaction sqlTransaction = connect.BeginTransaction();
                using (OracleCommand sqlCommand = CreateCommand(connect, string.Empty, parameters, commandType))
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
         * @description: 使用事务执行多条SQL语句，并返回影响的行数
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQueryUseTrans(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            using (OracleConnection connect = Connection)
            {
                OracleTransaction sqlTransaction = connect.BeginTransaction();

                using (OracleCommand sqlCommand = CreateCommand(connect, string.Empty, null, CommandType.Text, sqlTransaction))
                {
                    int result = 0;
                    try
                    {
                        foreach (var item in sqlDict)
                        {
                            sqlCommand.Parameters.Clear();
                            sqlCommand.CommandText = item.Key;
                            if (item.Value != null && item.Value.Length > 0)
                                sqlCommand.Parameters.AddRange(GetOracleParamArr(item.Value));
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

        public void ExecuteBulkCopy(string deskName, DataTable dataTable, int timeOut = 5 * 60)
        {
           throw new Exception("目前不支持批量插入！");
        }

        public void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60)
        {
            throw new Exception("目前不支持批量插入！");
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
            using (OracleConnection connect = Connection)
            using (OracleCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                int result = 0;
                try
                {
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
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
            using (OracleConnection connect = Connection)
            using (OracleCommand command = CreateCommand(connect, strSQL, parameters, commandType))
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
            using (OracleConnection connect = Connection)
            using (OracleCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter(command);
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
            using (OracleConnection connect = Connection)

            using (OracleCommand command = CreateCommand(connect, strSQL, parameters))
            {
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }
        /*
         * @description: 获取数据表-分页
         * @param {type} 
         * @return: 
         */
        public DataTable ExecuteDataTable(string strSQL, int CurrentPage, int PageSize, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (OracleConnection connect = Connection)
            using (OracleCommand Command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                DataTable DataTable = new DataTable();
                OracleDataAdapter Adapter = new OracleDataAdapter(Command);
                Adapter.Fill((CurrentPage - 1) * PageSize, PageSize, DataTable);
                return DataTable;
            }
        }
        /*
         * @description: 获取数据表-并行
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
            using (OracleConnection connect = Connection)
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
        #endregion
    }
}