using System;
using ADF.Utility;

namespace ADF.DataAccess.AbstractFactory
{
    public class DbFactoryProvider
    {
        private static DbFactoryProvider dbFactory = null;
        private static object sysObj = new object();

        private DbFactoryProvider() { }

        public static DbFactoryProvider GetInstance()
        {
            if (dbFactory == null)
            {
                lock (sysObj)
                {
                    if (dbFactory == null)
                    {
                        dbFactory = new DbFactoryProvider();
                    }
                }
            }
            return dbFactory;
        }

        private string DatabaseType
        {
            get => ConfigHelper.GetValue("DatabaseType");
        }

        private string ConnectionString
        {
            get => ConfigHelper.GetConnectionStr(ConfigHelper.GetValue("ConnectionName"));
        }

        /// <summary>
        /// 将数据库类型字符串转换为对应的数据库类型的枚举
        /// </summary>
        /// <param name="dbTypeStr">数据库类型字符串</param>
        /// <returns></returns>
        public DatabaseTypeEnum DbTypeStrToDbType(string dbTypeStr)
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

        public DbAbstractFactory GetFactory()
        {
            DbAbstractFactory dbAbsFactory = null;
            switch (DbTypeStrToDbType(DatabaseType))
            {
                case DatabaseTypeEnum.SqlServer:
                    dbAbsFactory = new SqlserverFactory(ConnectionString);
                    break;
                case DatabaseTypeEnum.Oracle:
                    dbAbsFactory = new OracleFactory(ConnectionString);
                    break;
                default: throw new Exception("请传入有效的数据库！");
            }
            return dbAbsFactory;
        }
    }
}