using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADF.Utility;

namespace ADF.DataAccess.ORM
{
    public abstract class DbHelper : IDisposable
    {
        private DbProviderFactory factory;
        private string connectionStr;
        private DbConnection connection;
        private DbCommand command;

        protected bool Disposed { get; set; }

        public DbConnection Connection
        {
            get
            {
                if (Disposed || connection == null)
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionStr;
                    connection.Open();
                    Disposed = false;
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

        public DbCommand Command
        {
            get
            {
                if (Disposed || command == null)
                {
                    command = Connection.CreateCommand();
                }
                return command;
            }
        }

        public DbHelper(DatabaseType dbType, string conStr)
        {
            factory = DbProviderFactroyHelper.GetProviderFactory(dbType);
            this.connectionStr = conStr;
        }

        public DbTransaction UseTransation(IsolationLevel? level = null)
        {
            DbTransaction transaction = null;
            if (level.HasValue)
                transaction = Connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(level.Value);
            Command.Transaction = transaction;
            return transaction;
        }

        public void SetCommand(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text, int timeOut = 600)
        {
            Command.CommandText = strSQL;
            Command.CommandType = commandType;
            Command.CommandTimeout = timeOut;
            Command.Parameters.Clear();
            if (parameters?.Count > 0)
            {
                foreach (var item in parameters)
                {
                    Command.Parameters.Add(CreateDbParameter(item));
                }
            }
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
            SetCommand(strSQL, parameters, commandType);
            return Command.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destTableName">服务器上目标表的名称</param>
        /// <param name="copyData">DataTable</param>
        /// <param name="timeOut">属性的整数值。默认值为 300 秒。值 0 指示没有限制；批量复制将无限期等待。</param>
        public abstract void ExecuteBulkCopy(string destTableName, DataTable copyData, int timeOut = 5 * 60);
        #endregion

        #region 数据获取
        /*
         * @description: 获取数据量总数
         * @param {type} 
         * @return: 
         */
        public int ExecuteCount(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            SetCommand(strSQL, parameters, commandType);
            int result = 0;
            DbDataReader reader = Command.ExecuteReader();
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
        /*
         * @description: 获取首行首列
         * @param {type} 
         * @return: 
         */
        public object ExecuteScalar(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            SetCommand(strSQL, parameters, commandType);
            object result = Command.ExecuteScalar();
            if (object.Equals(null, result) || object.Equals(DBNull.Value, result))
            {
                return null;
            }
            else
            {
                return result;
            }

        }
        /*
         * @description: 获取数据集
         * @param {type} 
         * @return: 
         */
        public DataSet ExecuteDataSet(string strSQL, List<CusDbParameter> parameters = null, CommandType commandType = CommandType.Text)
        {
            SetCommand(strSQL, parameters, commandType);
            DbDataAdapter dataAdapter = factory.CreateDataAdapter();
            dataAdapter.SelectCommand = Command;
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            return dataSet;
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
            SetCommand(strSQL, parameters, commandType);
            DbDataReader reader = Command.ExecuteReader();
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

        #region  Dispose
        protected void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                connection?.Dispose();
                command?.Dispose();
            }

            Disposed = true;
        }

        ~DbHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}