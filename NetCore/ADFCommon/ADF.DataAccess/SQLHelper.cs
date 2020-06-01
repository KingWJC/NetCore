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

            }
        }

        public SQLHelper() { }

        public SQLHelper(string connStr)
        {
            this.connectionStr = connStr;
        }


    }
}
