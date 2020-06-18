using System;
using ADF.Utility;

namespace ADF.DataAccess.SimpleFactory
{
    public class DbFactory
    {
        private static string DatabaseType
        {
            get => ConfigHelper.GetValue("DatabaseType");
        }

        private static string ConnectionString
        {
            get => ConfigHelper.GetConnectionStr(ConfigHelper.GetValue("ConnectionName"));
        }

        /// <summary>
        /// 将数据库类型字符串转换为对应的数据库类型的枚举
        /// </summary>
        /// <param name="dbTypeStr">数据库类型字符串</param>
        /// <returns></returns>
        public static DatabaseTypeEnum DbTypeStrToDbType(string dbTypeStr)
        {
            if (string.IsNullOrEmpty(dbTypeStr))
                throw new Exception("请输入数据库类型字符串！");
            else
            {
                switch (dbTypeStr.ToLower())
                {
                    case "sqlserver": return DatabaseTypeEnum.SqlServer;
                    case "oracle": return DatabaseTypeEnum.Oracle;
                    default: throw new Exception("请输入合法的数据库类型字符串！");
                }
            }
        }

        /// <summary>
        /// 将数据库类型的枚举转换为对应的数据库类型字符串
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public string DbTypeToDbTypeStr(DatabaseTypeEnum dbType)
        {
            switch (dbType)
            {
                case DatabaseTypeEnum.SqlServer: return "SqlServer";
                case DatabaseTypeEnum.Oracle: return "Oracle";
                default: throw new Exception("请输入合法的数据库类型！");
            }
        }

        public static IDbHelper CreateDbHelper()
        {
            IDbHelper dbHelper = null;
            switch (DbTypeStrToDbType(DatabaseType))
            {
                case DatabaseTypeEnum.SqlServer:
                    dbHelper = new SqlserverHelper(ConnectionString);
                    break;
                case DatabaseTypeEnum.Oracle:
                    dbHelper = new OracleHelper(ConnectionString);
                    break;
                default: throw new Exception("请传入有效的数据库！");
            }
            return dbHelper;
        }
    }
}