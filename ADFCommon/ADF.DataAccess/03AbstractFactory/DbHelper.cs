using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.AbstractFactory
{
    public static class DbHelper
    {
        #region 数据更新
        /*
         * @description: 获取影响的行数
         * @param {type} 
         * @return: 
         */
        public static int ExecuteNonQuery(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            using (IDbCommand command = factory.CreateCommand(connect, strSQL, parameters, commandType))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static int ExecuteNonQuery(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            {
                using (IDbCommand command = factory.CreateCommand(connect, string.Empty))
                {
                    int result = 0;
                    foreach (var item in sqlDict)
                    {
                        command.Parameters.Clear();
                        command.CommandText = item.Key;
                        if (item.Value != null && item.Value.Length > 0)
                        {
                            foreach (var param in item.Value)
                            {
                                command.Parameters.Add(factory.CreateDbParameter(param));
                            }
                        }
                        result += command.ExecuteNonQuery();
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
        public static int ExecuteNonQueryUseTrans(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            {
                IDbTransaction transaction = connect.BeginTransaction();
                using (IDbCommand command = factory.CreateCommand(connect, string.Empty, parameters, commandType, transaction))
                {
                    int result = 0;
                    try
                    {
                        result = command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
        public static int ExecuteNonQueryUseTrans(Dictionary<string, CusDbParameter[]> sqlDict)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            {
                IDbTransaction transaction = connect.BeginTransaction();
                using (IDbCommand command = factory.CreateCommand(connect, string.Empty, null, CommandType.Text, transaction))
                {
                    int result = 0;
                    try
                    {
                        foreach (var item in sqlDict)
                        {
                            command.Parameters.Clear();
                            command.CommandText = item.Key;
                            if (item.Value != null && item.Value.Length > 0)
                                foreach (var param in item.Value)
                                {
                                    command.Parameters.Add(factory.CreateDbParameter(param));
                                }
                            result += command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
        public static void ExecuteSQLBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (SqlBulkCopy BulkCopy = new SqlBulkCopy(factory.CreateConnection() as SqlConnection))
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

        private static int _bulkCount = 50000;

        /// <summary>
        /// 批量插入(事务，自定义列，不触发约束和触发器)
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">列的对映关系</param>       
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public static void ExecuteSQLBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (SqlConnection connect = factory.CreateConnection() as SqlConnection)
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
        public static int ExecuteCount(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            using (IDbCommand command = factory.CreateCommand(connect, strSQL, parameters, commandType))
            {
                int result = 0;
                IDataReader reader = command.ExecuteReader();
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
        public static object ExecuteScalar(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            using (IDbCommand command = factory.CreateCommand(connect, strSQL, parameters, commandType))
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
        public static DataSet ExecuteDataSet(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            using (IDbConnection connect = factory.CreateConnection())
            using (IDbCommand command = factory.CreateCommand(connect, strSQL, parameters, commandType))
            {
                IDbDataAdapter dataAdapter = factory.CreateDataAdapter();
                dataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
        }
        /*
         * @description: 获取数据表
         * @param {type} 
         * @return: 
         */
        public static DataTable ExecuteDataTable(string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            DataSet dataSet = ExecuteDataSet(strSQL, parameters, commandType);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;
            else
                return dataSet.Tables[0];
        }

        /*
         * @description: 获取数据表-并行-In条件
         * @param {type} 
         * @return: 
         */
        public static DataTable ExecuteDataTableParallel<T>(string strSQL, List<T> wheres)
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
        public static DataTable ExecPageProcSQL(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            DataTable dt = new DataTable();
            totalCount = 0;
            using (SqlConnection connect = factory.CreateConnection() as SqlConnection)
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
        public static DataTable ExecPageProcOracle(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount)
        {
            DbAbstractFactory factory = DbFactoryProvider.GetInstance().GetFactory();
            DataTable dt = new DataTable();
            totalCount = 0;
            using (OracleConnection connect = factory.CreateConnection() as OracleConnection)
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