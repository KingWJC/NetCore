using System;
using ADF.Utility;

namespace ADF.DataAccess.MixFactory
{
    public class DbHelperFactory
    {
        public static DbHelper GetDbHelper(DatabaseType? dbType = null, string constr = "")
        {
            constr = constr.IsNullOrEmpty() ? ConfigHelper.GetConnectionStr(ConfigHelper.GetValue("ConnectionName")) : constr;
            dbType = dbType.IsNullOrEmpty() ? DbTypeStrToDbType(ConfigHelper.GetValue("DatabaseType")) : dbType;

            switch (dbType)
            {
                case DatabaseType.Oracle: return new OracleHelper(constr);
                case DatabaseType.SqlServer: return new SqlserverHelper(constr);
                default: throw new Exception("暂不支持");
            }
        }

        public static DbHelper GetDbHelperRlection(DatabaseType? dbType = null, string constr = "")
        {
            constr = constr.IsNullOrEmpty() ? ConfigHelper.GetConnectionStr(ConfigHelper.GetValue("ConnectionName")) : constr;
            string dataBaseType = dbType.IsNullOrEmpty() ? ConfigHelper.GetValue("DatabaseType") : DbTypeToDbTypeStr(dbType.Value);

            Type helperType = Type.GetType("ADF.DataAccess.MixFactory." + dataBaseType + "Helper");
            return Activator.CreateInstance(helperType, new object[] { constr }) as DbHelper;
        }

        /// <summary>
        /// 将数据库类型字符串转换为对应的数据库类型的枚举
        /// </summary>
        /// <param name="dbTypeStr">数据库类型字符串</param>
        /// <returns></returns>
        public static DatabaseType DbTypeStrToDbType(string dbTypeStr)
        {
            if (string.IsNullOrEmpty(dbTypeStr))
                throw new Exception("请输入数据库类型字符串！");
            else
            {
                switch (dbTypeStr.ToLower())
                {
                    case "sqlserver": return DatabaseType.SqlServer;
                    case "oracle": return DatabaseType.Oracle;
                    default: throw new Exception("请输入合法的数据库类型字符串！");
                }
            }
        }

        /// <summary>
        /// 将数据库类型的枚举转换为对应的数据库类型字符串
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string DbTypeToDbTypeStr(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer: return "SqlServer";
                case DatabaseType.Oracle: return "Oracle";
                default: throw new Exception("请输入合法的数据库类型！");
            }
        }
    }
}