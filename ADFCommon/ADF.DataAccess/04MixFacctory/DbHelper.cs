using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.MixFactory
{
    public abstract class DbHelper
    {
        private DbProviderFactory factory;
        private string connectionStr;
        private DbConnection connection;

        public DbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionStr;
                    connection.Open();
                }
                else if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                return connection;
            }
        }

        public DbHelper(DatabaseType dbType, string conStr)
        {
            factory = DbProviderFactroyHelper.GetProviderFactory(dbType);
            this.connectionStr = conStr;
        }

        public DbCommand CreateCommand(DbConnection connect, string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text, DbTransaction transaction = null, int timeOut = 600)
        {
            DbCommand dbCommand = connect.CreateCommand();
            dbCommand.CommandText = strSQL;
            dbCommand.CommandType = commandType;
            dbCommand.CommandTimeout = timeOut;
            if (parameters?.Count > 0)
            {
                foreach (var item in parameters)
                {
                    dbCommand.Parameters.Add(CreateDbParameter(item));
                }
            }
            if (transaction != null)
            {
                dbCommand.Transaction = transaction;
            }
            return dbCommand;
        }

        public abstract DbParameter CreateDbParameter(CusDbParameter parameter);
        #region 数据更新
        /*
         * @description: 获取影响的行数
         * @param {type} 
         * @return: 
         */
        public int ExecuteNonQuery(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(Dictionary<string, List<CusDbParameter>> sqlDict)
        {

            using (DbConnection connect = Connection)
            {
                using (DbCommand command = CreateCommand(connect, string.Empty))
                {
                    int result = 0;
                    foreach (var item in sqlDict)
                    {
                        command.Parameters.Clear();
                        command.CommandText = item.Key;
                        if (item.Value != null && item.Value.Count > 0)
                        {
                            foreach (var param in item.Value)
                            {
                                command.Parameters.Add(CreateDbParameter(param));
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
        public int ExecuteNonQueryUseTrans(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            {
                DbTransaction transaction = connect.BeginTransaction();
                using (DbCommand command = CreateCommand(connect, string.Empty, parameters, commandType, transaction))
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
        public int ExecuteNonQueryUseTrans(Dictionary<string, List<CusDbParameter>> sqlDict)
        {
            using (DbConnection connect = Connection)
            {
                DbTransaction transaction = connect.BeginTransaction();
                using (DbCommand command = CreateCommand(connect, string.Empty, null, CommandType.Text, transaction))
                {
                    int result = 0;
                    try
                    {
                        foreach (var item in sqlDict)
                        {
                            command.Parameters.Clear();
                            command.CommandText = item.Key;
                            if (item.Value != null && item.Value.Count > 0)
                            {
                                foreach (var param in item.Value)
                                {
                                    command.Parameters.Add(CreateDbParameter(param));
                                }
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
        public abstract void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60);

        /// <summary>
        /// 批量插入(事务，自定义列，不触发约束和触发器)
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="columns">列的对映关系</param>       
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public abstract void ExecuteBulkCopy(string destTableName, DataTable copyData, string[][] columns, int timeOut = 5 * 60);
        #endregion

        #region 数据获取
        /*
         * @description: 获取数据量总数
         * @param {type} 
         * @return: 
         */
        public int ExecuteCount(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                int result = 0;
                DbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
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
        public object ExecuteScalar(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {

            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
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
        public DataSet ExecuteDataSet(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            using (DbConnection connect = Connection)
            using (DbCommand command = CreateCommand(connect, strSQL, parameters, commandType))
            {
                DbDataAdapter dataAdapter = factory.CreateDataAdapter();
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
        public DataTable ExecuteDataTable(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            DataSet dataSet = ExecuteDataSet(strSQL, parameters, commandType);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;
            else
                return dataSet.Tables[0];
        }

        public DataRow ExecuteDataRow(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            using (DbConnection conn = Connection)
            using (DbCommand command = CreateCommand(conn, strSQL, parameters, commandType))
            {
                DbDataReader reader = command.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    DataTable dataTable = new DataTable();
                    object[] values = new object[reader.FieldCount];
                    int fieldCount = reader.GetValues(values);
                    dataTable.LoadDataRow(values, false);
                    return dataTable.Rows[0];
                }
                return null;
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
        public abstract DataTable ExecPageProc(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount);
        #endregion
    }
}