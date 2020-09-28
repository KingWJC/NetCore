using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ADF.DataAccess.AbstractFactory
{
    public class SqlserverFactory : DbAbstractFactory
    {
        public SqlserverFactory(string connStr) 
        : base(connStr)
        {
        }

        public override IDbDataParameter CreateDbParameter(CusDbParameter commParam)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = commParam.ParameterName;
            param.Value = commParam.Value;
            // 修改nvarchar到varchar编码问题，底层在进行默认NVarchar  造成没法设置varchar
            if (!commParam.DbType.Equals(DbType.AnsiString) || commParam.Value is string)
                param.DbType = commParam.DbType;
            if (commParam.Size > 0)
                param.Size = commParam.Size;
            return param;
        }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionStr);
        }

        public override IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public override IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }
    }
}