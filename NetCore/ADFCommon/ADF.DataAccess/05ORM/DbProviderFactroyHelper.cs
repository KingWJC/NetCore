using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;

namespace ADF.DataAccess.ORM
{
    public class DbProviderFactroyHelper
    {
        public static DbProviderFactory GetProviderFactory(DatabaseType dbType)
        {
            DbProviderFactory dbProviderFactory = null;
            switch (dbType)
            {
                case DatabaseType.SqlServer: dbProviderFactory = SqlClientFactory.Instance; break;
                case DatabaseType.Oracle: dbProviderFactory = OracleClientFactory.Instance; break;
                default: throw new Exception("请传入有效的数据库");
            }
            return dbProviderFactory;
        }
    }
}