using System.Data;
using System.Data.Common;

namespace ADF.DataAccess.AbstractFactory
{
    public abstract class DbAbstractFactory
    {
        protected string connectionStr;
        public DbAbstractFactory(string connectStr)
        {
            connectionStr = connectStr;
        }

        public abstract IDbConnection CreateConnection();

        public abstract IDbCommand CreateCommand();

        public abstract IDbDataParameter CreateDbParameter(CusDbParameter param);

        public IDbCommand CreateCommand(IDbConnection connect, string strSQL, CusDbParameter[] parameters = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null, int timeOut = 600)
        {
            IDbCommand dbCommand = connect.CreateCommand();
            dbCommand.CommandText = strSQL;
            dbCommand.CommandType = commandType;
            dbCommand.CommandTimeout = timeOut;
            if (parameters?.Length > 0)
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

        public abstract IDbDataAdapter CreateDataAdapter();
    }
}

