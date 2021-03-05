using System;
using ADF.Utility;
using System.Data;
using System.Collections.Generic;

namespace ADF.DataAccess.Simple
{
    public class DbHelper
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
        public static string DbTypeToDbTypeStr(DatabaseTypeEnum dbType)
        {
            switch (dbType)
            {
                case DatabaseTypeEnum.SqlServer: return "SqlServer";
                case DatabaseTypeEnum.Oracle: return "Oracle";
                default: throw new Exception("请输入合法的数据库类型！");
            }
        }

        /// <summary>
        /// 将数据表中的日期列的默认值，转为DBNULL
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static DataTable DateTimeVal(DataTable dt)
        {
            List<string> LisColumnName = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.DataType == (new DateTime()).GetType())
                {
                    LisColumnName.Add(dc.ColumnName);
                }
            }
            if (LisColumnName.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (string str in LisColumnName)
                    {
                        if (dr[str] != DBNull.Value && dr[str].ToString() != "")
                        {
                            if (Convert.ToDateTime(dr[str]).Date == Convert.ToDateTime("1900-01-01"))
                            {
                                dr[str] = DBNull.Value;
                            }
                        }

                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static object GetSingle(string strSQL)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteScalar(strSQL);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteScalar(strSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 获取总行数
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int GetCount(string strSQL)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteCount(strSQL);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteCount(strSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSQL,CusDbParameter[] parameters)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteDataTable(strSQL,parameters);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteDataTable(strSQL,parameters);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string strSQL)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteDataSet(strSQL);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteDataSet(strSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 获取数据表分页
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDataTablePage(string sqlSQL, int currentPage, int pageSize)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteDataTable(sqlSQL, currentPage, pageSize);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteDataTable(sqlSQL, currentPage, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 获取数据表分页
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDataTablePage(string strSQL, int pageSize, int pageCurrent, string fdShow, string fdOrder, out int totalCount)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecPageProc(strSQL, pageSize, pageCurrent, fdShow, fdOrder, out totalCount);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecPageProc(strSQL, pageSize, pageCurrent, fdShow, fdOrder, out totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            totalCount = 0;
            return null;
        }

        /// <summary>
        /// 获取数据表，多条件，in的情况
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDataParal(string strSQL, List<string> wheres)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteDataTableParallel(strSQL, wheres);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteDataTableParallel(strSQL, wheres);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int Modify(string strSQL)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteNonQuery(strSQL);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteNonQuery(strSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int Modify(List<string> strSQL)
        {
            Dictionary<string, CusDbParameter[]> dicts = new Dictionary<string, CusDbParameter[]>();
            strSQL.ForEach(sql => dicts.Add(sql, null));
            return Modify(dicts);
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int Modify(Dictionary<string, CusDbParameter[]> dicts)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteNonQuery(dicts);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteNonQuery(dicts);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int ModifyByTrans(string strSQL)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        return DbFactory.SQLServer(ConnectionString).ExecuteNonQueryUseTrans(strSQL);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteNonQueryUseTrans(strSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// 数据更新（事务)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int ModifyByTrans(List<string> strSQL)
        {
            Dictionary<string, CusDbParameter[]> dicts = new Dictionary<string, CusDbParameter[]>();
            strSQL.ForEach(sql => dicts.Add(sql, null));
            return ModifyByTrans(dicts);
        }

        /// <summary>
        /// 数据更新(事务）)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static int ModifyByTrans(Dictionary<string, CusDbParameter[]> dicts)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:

                        return DbFactory.SQLServer(ConnectionString).ExecuteNonQueryUseTrans(dicts);
                    case DatabaseTypeEnum.Oracle:
                        return DbFactory.Oracle(ConnectionString).ExecuteNonQueryUseTrans(dicts);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// 数据更新（批量）
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns></returns>
        public static void BulkCopyData(string tableName, DataTable dtCopy)
        {
            try
            {
                switch (DbTypeStrToDbType(DatabaseType))
                {
                    case DatabaseTypeEnum.SqlServer:
                        DbFactory.SQLServer(ConnectionString).ExecuteBulkCopy(tableName, dtCopy);
                        break;
                    case DatabaseTypeEnum.Oracle:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}