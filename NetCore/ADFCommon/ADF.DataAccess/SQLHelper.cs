using System;
using System.Data;
using System.Data.SqlClient;

namespace ADF.DataAccess
{
    public class SQLHelper
    {
        private DatabaseType dbType;

        private string connectionStr;

        private SqlConnection connection;
        public SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqlConnection(connectionStr);
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

        public SQLHelper() { }

        public SQLHelper(string connStr)
        {
            this.connectionStr = connStr;
        }

        public int ExecteNonQuery(string strSQL, SqlParameter[] parameters, CommandType commandType)
        {
            int result = -1;
            using (SqlConnection connect = Connection)
            {
                using (SqlCommand sqlCommand = new SqlCommand(strSQL, connect))
                {
                    sqlCommand.CommandType = commandType;
                    if (parameters?.Length > 0)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    result = sqlCommand.ExecuteNonQuery();
                }
            }

            return result;
        }


    }
}
