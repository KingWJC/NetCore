using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace ADF.DataAccess.AbstractFactory
{
    public class OracleFactory : DbAbstractFactory
    {
        public OracleFactory(string connStr)
        : base(connStr)
        {
        }

        public override IDbDataParameter CreateDbParameter(CusDbParameter commParam)
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

            return param;
        }

        public override IDbConnection CreateConnection()
        {
            return new OracleConnection(connectionStr);
        }

        public override IDbCommand CreateCommand()
        {
            return new OracleCommand();
        }

        public override IDbDataAdapter CreateDataAdapter()
        {
            return new OracleDataAdapter();
        }
    }
}