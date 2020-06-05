using System;
using System.Data;
using ADF.Utility;

namespace ADF.DataAccess.Simple
{
    public class BaseService
    {
        #region 私有方法
        protected static string DatabaseType
        {
            get => ConfigHelper.GetValue("DatabaseType");
        }

        protected static string ConnectionString
        {
            get => ConfigHelper.GetConnectionStr(ConfigHelper.GetValue("ConnectionName"));
        }

        /// <summary>
        /// 获取数据库有效值
        /// </summary>
        /// <param name="propValue">属性值</param>
        /// <returns>有效值</returns>
        private static object GetValueObject(object propValue)
        {
            if (propValue == null)
            {
                return "NULL";
            }
            if (propValue is DBNull)
            {
                return "NULL";
            }
            else
            {
                if (propValue.GetType() == typeof(DateTime))
                {
                    if (Convert.ToDateTime(propValue) == DateTime.MinValue)
                    {
                        return "NULL";
                    }
                    else
                    {
                        return string.Format("TO_DATE('{0}','yyyy-mm-dd HH24:mi:ss')", Convert.ToDateTime(propValue).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                else if (propValue.GetType() == typeof(string))
                {
                    return string.Format("'{0}'", propValue.ToString().Replace("'", "''"));
                }
                else
                {
                    return propValue;
                }
            }
        }
        
                /// <summary>
        /// 将数据库类型字符串转换为对应的数据库类型
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
        /// 将数据库类型转换为对应的数据库类型字符串
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string DbTypeToDbTypeStr(DatabaseTypeEnum dbType)
        {
            if (dbType.IsNullOrEmpty())
                throw new Exception("请输入数据库类型！");
            else
            {
                switch (dbType)
                {
                    case DatabaseTypeEnum.SqlServer: return "SqlServer";
                    case DatabaseTypeEnum.Oracle: return "Oracle";
                    default: throw new Exception("请输入合法的数据库类型！");
                }
            }
        }
        #endregion

        #region sql执行相关方法
        /// <summary>
        /// 通过sql获取数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>结果集(Ssytem.Data.DataTable)</returns>
        public DataTable GetData(string sql)
        {
            DataSet ds = null;
            switch (DatabaseType)
            {
                case DatabaseTypeEnum.Oracle:
                    ds = DbFactory.Oracle(ConnectionString).ExecuteDataSet(sql);
                    break;
                case DatabaseTypeEnum.SqlServer:
                    ds = DbFactory.SQLServer(ConnectionString).ExecuteDataSet(sql);
            }
            ds = DbHelperUtil.Query(DBType.oracle, ConnectionString, sql);
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables.Count < 1)
            {
                return null;
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>实体</returns>
        public T GetEntity<T>(string sql)
        {
            T t = default(T);
            if (string.IsNullOrEmpty(sql))
            {
                return t;
            }
            DataTable dtSource = GetData(sql);
            if (dtSource == null)
            {
                return t;
            }
            if (dtSource.Rows.Count < 1)
            {
                return t;
            }
            t = JHICU.Utils.DataUtils.DataRowToItem<T>(dtSource.Rows[0]);
            return t;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns>实体集合</returns>
        public List<T> GetEntityList<T>(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }
            DataTable dtSource = GetData(sql);
            if (dtSource == null)
            {
                return null;
            }
            if (dtSource.Rows.Count < 1)
            {
                return null;
            }
            return JHICU.Utils.DataUtils.DataTableToList<T>(dtSource);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="t">实体对象，若返回值为true,则该参数为查询到的第一条数据</param>
        /// <returns>true:存在 false:不存在</returns>
        public bool IsExists<T>(string sql, out T t)
        {
            bool isExists = false;
            t = GetEntity<T>(sql);
            if (t != null)
            {
                isExists = true;
            }
            return isExists;
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <param name="confgiKey">key</param>
        /// <param name="deptCode">科室主键，若为空则默认获取当前科室主键</param>
        /// <returns></returns>
        public string GetSysConfig(string confgiKey, string deptCode)
        {
            string sql = string.Format(@"SELECT CONFIG_VALUE FROM JHICU_SYSTEM_CONFIG WHERE 
                                        CONFIG_KEY='{0}' AND 
                                        DEPT_CODE='{1}' AND 
                                        HOSPITAL_NO='{2}'", confgiKey, deptCode, JHSysInfo._HospitalNo);
            DataTable dt = GetData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToString(dt.Rows[0]["CONFIG_VALUE"]);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从特定sequence中获取值
        /// </summary>
        /// <param name="seqName">序列名称</param>
        /// <returns>获取到的sequence值</returns>
        public int GetNextId(string seqName)
        {
            int maxId = 0;
            string sql = "SELECT {0}.NEXTVAL MAXID  FROM DUAL";
            sql = string.Format(sql, seqName);
            DataTable dt = GetData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["MAXID"].ToString()))
                {
                    int.TryParse(dt.Rows[0]["MAXID"].ToString(), out maxId);
                }
            }
            return maxId;
        }
        /// <summary>
        /// 获取服务器当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetServiceDateTime()
        {
            DateTime currentDatetime = DateTime.Now;
            string sql = "select sysdate from dual";
            DataTable dt = GetData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                currentDatetime = DateTime.Parse(dt.Rows[0]["SYSDATE"].ToString());
            }
            return currentDatetime;
        }

        /// <summary>
        /// 获取下一个显示的数字
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetNextNumber(string tableName, string columnName)
        {
            int nextNumber = 1;
            string sql = "SELECT MAX({0}) MaxNumber FROM {1}";
            sql = string.Format(sql, columnName, tableName);
            DataTable dt = GetData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                nextNumber = int.Parse(dt.Rows[0]["MaxNumber"].ToString()) + 1;
            }
            return nextNumber;
        }

        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="sqls">sql语句集合</param>
        /// <param name="isTran">true:事务  false:非事务</param>
        /// <returns>受影响的行数</returns>
        public static int UpdateData(List<string> sqls, bool isTran = true)
        {
            return DbHelperUtil.ExecuteNonQueryList(ConnectionString, isTran, sqls);
        }

        /// <summary>
        /// 单个修改数据
        /// </summary>
        /// <param name="sql">sql/param>
        /// <returns>受影响的行数</returns>
        public static int UpdateData(string sql, out string message)
        {
            message = string.Empty;
            return DbHelperUtil.ExecuteNonQuery(ConnectionString, sql, out message);
        }

        /// <summary>
        /// 向数据库里插入blob格式的字段
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="blob">blob字节</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlUpdateBlob(string strSQL, byte[] blob)
        {
            return DbHelperUtil.ExecuteSqlUpdateBlob(strSQL, blob);
        }

        /// <summary>
        /// 向数据库里插入clob格式的字段
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="clob"></param>
        /// <returns></returns>
        public static int ExecuteSqlUpdateClob(string strSQL, string clob)
        {
            return DbHelperUtil.ExecuteSqlUpdateClob(strSQL, clob);
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseEntity GetEntityListT<T>(string where = null)
        {
            var result = ResponseEntity.GetSuccessEntity();
            var message = string.Empty;
            List<T> data = GetEntitys<T>(where);

            if (data != null && data.Count > 0)
            {
                result.DataSource = data;
                message = "获取成功!";
            }
            else
            {
                result.IsSuccess = false;
                message = "获取失败!";
            }
            result.MessageContent = message;
            return result;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetEntitys<T>(string where)
        {
            Type type = typeof(T);

            StringBuilder columnsNames = new StringBuilder();
            type.GetProperties().ToList().ForEach(item =>
            {
                if (item.Name != "PropertiesValueChanged")
                    columnsNames.Append("\"" + item.Name + "\",");
            });
            string sql = $"SELECT {columnsNames.ToString().TrimEnd(',')} FROM {type.Name} T ";
            if (!string.IsNullOrEmpty(where))
            {
                sql += where;
            }
            var data = GetEntityList<T>(sql);
            return data;
        }

        /// <summary>
        /// 根据查询字典拼接Where语句
        /// </summary>
        /// <param name="queryDict">查询条件字典</param>
        /// <returns></returns>
        public string GetWhereSql(Dictionary<string, object> queryDict)
        {
            string where = string.Empty;
            if (queryDict != null && queryDict.Count > 0)
            {
                where = $" WHERE 1=1 ";
                StringBuilder sb = new StringBuilder();
                sb.Append(" AND (");
                queryDict.ToList().ForEach(x =>
                {
                    DateTime dt;
                    if (DateTime.TryParse(x.Value.ToString(), out dt))
                    {
                        sb.Append($" T.{x.Key}={ToDateTimeString(dt)} AND");
                    }
                    else
                    {
                        sb.Append($" T.{x.Key}='{x.Value}' AND");
                    }
                });
                where += sb.ToString();
                where = where.Substring(0, where.Length - 3) + ")";
            }
            return where;
        }

        /// <summary>
        /// 更新实体的指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyID"></param>
        /// <param name="changeValues"></param>
        /// <returns></returns>
        public ResponseEntity UpdateEntity<T>(int keyID, Dictionary<string, object> changeValues) where T : new()
        {
            var result = ResponseEntity.GetSuccessEntity();
            var message = string.Empty;
            int index = UpdateEntity<T>(keyID, changeValues, out message);

            if (index > 0)
            {
                result.DataSource = index;
                message = "数据更新成功!";
            }
            else
            {
                result.IsSuccess = false;
                message = "数据更新失败!";
            }

            result.MessageContent = message;
            return result;
        }

        /// <summary>
        /// 更新实体指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyID"></param>
        /// <param name="changeValues"></param>
        /// <param name="message"></param>
        /// <param name="index"></param>
        public int UpdateEntity<T>(int keyID, Dictionary<string, object> changeValues, out string message) where T : new()
        {
            var obj = new T();

            var key = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(BaseAttribute.ID), true).Length > 0).FirstOrDefault();
            var propertys = typeof(T).GetProperties().Where(p =>
            {
                bool flag = changeValues.ContainsKey(p.Name);
                if (flag)
                    p.SetValue(obj, changeValues[p.Name], null);
                return flag;
            }).ToArray();

            var updateSql = UpdateSql<T>(obj, propertys) + $" WHERE {key.Name} = '{keyID}'";
            return UpdateData(updateSql, out message);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entityList">方案执行人对象</param>
        /// <returns></returns>
        public ResponseEntity UpdateEntytyList<T>(List<T> entityList)
        {
            var result = ResponseEntity.GetSuccessEntity();
            var message = string.Empty;

            var sqlList = new List<string>();
            var propertyInfos = typeof(T).GetProperties();
            var key = propertyInfos.Where(p => p.GetCustomAttributes(typeof(BaseAttribute.ID), true).Length > 0).FirstOrDefault();
            entityList.ForEach(p =>
            {
                object value = key.GetValue(p, null);
                if (value != null && value.ToString() != "0")
                {
                    sqlList.Add(UpdateSql<T>(p, propertyInfos) + $" WHERE {key.Name} = '{value}'");
                }
                else
                {
                    sqlList.Add(InsertSql<T>(p, propertyInfos));
                }
            });

            int index = UpdateData(sqlList, true);
            if (index > 0)
            {
                result.DataSource = entityList;
                message = "更新成功!";
            }
            else
            {
                result.IsSuccess = false;
                message = "更新失败!";
            }

            result.MessageContent = message;
            return result;
        }

        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string InsertSql<T>(T t, PropertyInfo[] properties, string clobField = null)
        {
            if (t == null)
            {
                return string.Empty;
            }

            ////拼装Insert语句
            string sql = string.Empty;
            string sqlColumns = string.Empty;
            string sqlVallues = string.Empty;

            foreach (PropertyInfo item in properties)
            {
                if (item.Name == "PropertiesValueChanged" || item.Name == "CREATE_DATE")
                    continue;
                //添加拼音码
                if (item.Name.Contains("PYM"))
                {
                    string key = item.Name.Substring(0, item.Name.LastIndexOf('_'));
                    item.SetValue(t, StringUtils.GetPinYinCode(typeof(T).GetProperty(key).GetValue(t, null).ToString()), null);
                }
                if (typeof(T).ToString() == "JHICU.Entity.JHERAS_DICT_USER" && item.Name == "LOGIN_PWD")
                {
                    ////添加新用户时，加密密码
                    item.SetValue(t, StringUtils.GetEncodingPassword(
                        typeof(T).GetProperty("LOGIN_NAME").GetValue(t, null).ToString(),
                        item.GetValue(t, null).ToString()), null);
                }
                ////拼装SQL
                if (item.GetCustomAttributes(typeof(BaseAttribute.ID), true).Length > 0)
                {
                    int id = GetNextId("SEQ_STATUS");
                    item.SetValue(t, id, null);
                    sqlVallues += (id + ",");
                }
                else if (item.Name == clobField)
                {
                    sqlVallues += ":fs" + ",";
                }
                else
                {
                    object obj = item.GetValue(t, null);
                    sqlVallues += GetValueObject(obj) + ",";
                }

                sqlColumns += "\"" + item.Name + "\",";
            }

            sql = string.Format("INSERT INTO {0}({1}) VALUES({2})", typeof(T).Name, sqlColumns.TrimEnd(','), sqlVallues.TrimEnd(','));

            return sql;
        }

        /// <summary>
        /// 获取更新SQL
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string UpdateSql<T>(T t, PropertyInfo[] properties)
        {
            StringBuilder sb = new StringBuilder(string.Format("Update {0} SET", t.GetType().Name));
            foreach (PropertyInfo subItem in properties)
            {
                if (subItem.Name == "PropertiesValueChanged" || subItem.Name == "CREATE_DATE" ||
                   subItem.GetCustomAttributes(typeof(BaseAttribute.ID), true).Length > 0)
                    continue;

                var subValue = subItem.GetValue(t, null);
                //添加拼音码
                if (subItem.Name.Contains("PYM"))
                {
                    string key = subItem.Name.Substring(0, subItem.Name.LastIndexOf('_'));
                    var temp = typeof(T).GetProperty(key).GetValue(t, null);
                    if (temp != null)
                    {
                        subItem.SetValue(t, StringUtils.GetPinYinCode(temp.ToString()), null);
                        subValue = subItem.GetValue(t, null);
                    }
                }
                if (subItem.PropertyType == typeof(string))
                {
                    string valueStr = subValue == null ? string.Empty : subValue.ToString().Trim();
                    sb.Append(string.Format(" {0} = '{1}' ", subItem.Name, valueStr));
                }
                else if (subItem.PropertyType == typeof(byte[]))
                {
                    sb.Append(string.Format(" {0} = :fs ", subItem.Name));
                }
                else if (subItem.PropertyType == typeof(DateTime))
                {
                    DateTime dt = DateTime.Parse(subValue.ToString());
                    sb.Append(string.Format(" {0} = to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", subItem.Name, dt.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    sb.Append(string.Format(" {0} = {1} ", subItem.Name, subValue.ToString().Trim()));
                }

                sb.Append(",");
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        /// <summary>
        /// 删除指定数据集
        /// </summary>
        /// <param name="mainID">关联主键</param>
        /// <returns></returns>
        public ResponseEntity DeleteEntityByIDs<T>(string ids, bool isLogic, int state = 1)
        {
            var result = ResponseEntity.GetSuccessEntity();
            var message = string.Empty;
            Type type = typeof(T);

            int index = 0;

            var property = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(BaseAttribute.ID), true).Length > 0).FirstOrDefault();
            if (property != null)
            {
                string sql = $"DELETE FROM {type.Name} WHERE {property.Name} IN ({string.Join(",", ids)})";
                if (isLogic)
                {
                    sql = $"UPDATE {type.Name} SET STATE={state} WHERE {property.Name} IN ({ids})";
                }

                index = UpdateData(sql, out message);
            }

            if (index > 0)
            {
                result.DataSource = index;
                result.MessageContent = "删除成功!";
            }
            else
            {
                result.IsSuccess = false;
                result.MessageContent = "删除失败!";
            }

            return result;
        }

        /// <summary>
        /// 获取添加Sql
        /// </summary>
        /// <returns></returns>
        public static string InsertRowSql(DataRow row, string tableName)
        {
            if (row == null)
            {
                return string.Empty; ;
            }
            //拼装Insert语句
            string sqlInsert = string.Empty;
            string sqlColumns = string.Empty;
            string sqlVallues = string.Empty;

            foreach (DataColumn item in row.Table.Columns)
            {
                //拼装SQL
                sqlColumns += "\"" + item.ColumnName + "\",";
                sqlVallues += GetValueObject(row[item]) + ",";
            }

            sqlInsert += string.Format("INSERT INTO {0}({1}) VALUES({2})",
                                      tableName,
                                      sqlColumns.TrimEnd(','),
                                      sqlVallues.TrimEnd(','));

            return sqlInsert;
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int GetCount<T>(string where = null)
        {
            string getSql = $"Select Count(*) From {typeof(T).Name} ";
            if (!string.IsNullOrEmpty(where))
            {
                getSql += where;
            }
            DataTable dt = GetData(getSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }
        #endregion

        #region 获取sql相关方法

        /// <summary>
        /// 获取实体中所有的字段，并拼接成sql查询列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetColumnNames<T>()
        {
            string columnsNames = string.Empty;
            //获取实体所有属性
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (var item in props)
            {
                if (item.Name == "PropertiesValueChanged")
                    continue;
                columnsNames = columnsNames + "\"" + item.Name + "\",";
            }

            columnsNames = columnsNames.TrimEnd(',');
            return columnsNames;
        }


        /// <summary>
        /// 获取实体中所有的字段，并拼接成sql查询列
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="exceptColumnNames">排除的列名</param>
        /// <returns></returns>
        public string GetColumnNames<T>(string tableName, params string[] exceptColumnNames)
        {
            string columnsNames = string.Empty;
            //获取实体所有属性
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (var item in props)
            {
                if (item.Name == "PropertiesValueChanged")
                {
                    continue;
                }
                if (exceptColumnNames != null && exceptColumnNames.Length > 0)
                {
                    if (exceptColumnNames.Contains(item.Name))
                    {
                        continue;
                    }
                }
                columnsNames = columnsNames + $"{tableName}.{item.Name},";
            }

            columnsNames = columnsNames.TrimEnd(',');
            return columnsNames;
        }
        #endregion

        #region 公共方法

        /// <summary>
        /// 将DateTime类型转换成Oracle转换字符串
        /// </summary>
        /// <param name="dateTimeValue">目标时间</param>
        /// <returns></returns>
        public string ToDateTimeString(DateTime dateTimeValue)
        {
            return $" TO_DATE('{dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-mm-dd HH24:mi:ss') ";
        }
        #endregion

        public int GetCount()
        {
            var server = DbFactory.Oracle("");
            return server.ExecuteCount("select count(1) from DICT_CRF");
        }
    }
}