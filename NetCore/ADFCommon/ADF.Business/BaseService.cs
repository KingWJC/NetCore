using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Reflection;
using ADF.DataAccess.Simple;
using ADF.Utility;

namespace ADF.Business.Simple
{
    public class BaseService
    {
        #region 私有方法
        protected static PropertyInfo GetKeyProperty(Type type)
        {
            return GetKeyPropertys(type).FirstOrDefault();
        }
        protected static List<PropertyInfo> GetKeyPropertys(Type type)
        {
            var properties = type
                .GetProperties()
                .Where(x => x.GetCustomAttributes(true).Select(o => o.GetType().FullName).Contains(typeof(KeyAttribute).FullName))
                .ToList();

            return properties;
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
        #endregion

        #region 外部接口
        /// <summary>
        /// 从特定sequence中获取值
        /// </summary>
        /// <param name="seqName">序列名称</param>
        /// <returns>获取到的sequence值</returns>
        public int GetNextId(string seqName)
        {
            string strSQL = $"SELECT {seqName}.NEXTVAL MAXID  FROM DUAL";
            object obj = DbHelper.GetSingle(strSQL);
            return obj.ToString().ToInt();
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetEntitys<T>(Dictionary<string, object> queryDict = null)
        {
            Type type = typeof(T);

            StringBuilder columnsNames = new StringBuilder();
            type.GetProperties().ToList().ForEach(item =>
            {
                columnsNames.Append("\"" + item.Name + "\",");
            });
            string sql = $"SELECT {columnsNames.ToString().TrimEnd(',')} FROM {type.Name} T ";

            var (where, parameter) = GetWhereSql(queryDict);

            if (!string.IsNullOrEmpty(where))
            {
                sql += where;
            }

            DataTable dataTable = DbHelper.GetDataTable(sql, parameter);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 根据查询字典拼接Where语句
        /// </summary>
        /// <param name="queryDict">查询条件字典</param>
        /// <returns></returns>
        public (string, CusDbParameter[]) GetWhereSql(Dictionary<string, object> queryDict)
        {
            string where = string.Empty;
            CusDbParameter[] parameters = null;
            if (queryDict != null && queryDict.Count > 0)
            {
                parameters = new CusDbParameter[queryDict.Count];
                StringBuilder sb = new StringBuilder(" WHERE 1=1 ");
                sb.Append(" AND (");
                var enumerator = queryDict.GetEnumerator();
                int index = 0;
                while (enumerator.MoveNext())
                {
                    var query = enumerator.Current;
                    DateTime dt;

                    if (DateTime.TryParse(query.Value.ToString(), out dt))
                    {
                        sb.Append($" T.{query.Key} = @{query.Key} AND");
                    }
                    else
                    {
                        sb.Append($" T.{query.Key} = @{query.Key} AND");
                    }
                    parameters[index] = new CusDbParameter(query.Key, query.Value);
                    index++;
                }

                where = sb.Remove(sb.Length - 3, 3).Append(")").ToString();
            }
            return (where, parameters);
        }

        /// <summary>
        /// 更新实体指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyID"></param>
        /// <param name="changeValues"></param>
        /// <param name="message"></param>
        /// <param name="index"></param>
        public int UpdateEntity<T>(int keyID, Dictionary<string, object> changeValues) where T : new()
        {
            if (changeValues.IsEmpty())
                return 0;

            var obj = new T();
            var key = GetKeyProperty(typeof(T));

            Func<PropertyInfo, bool> where = (p =>
            {
                bool flag = changeValues.ContainsKey(p.Name);
                if (flag)
                    p.SetValue(obj, changeValues[p.Name], null);
                return flag;
            });

            var updateSql = UpdateSql<T>(obj, key.Name, where) + $" WHERE {key.Name} = '{keyID}'";
            return DbHelper.Modify(updateSql);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entityList">方案执行人对象</param>
        /// <returns></returns>
        public int UpdateEntityList<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0)
                return 0;

            var sqlList = new List<string>();
            var key = GetKeyProperty(typeof(T));
            entityList.ForEach(p =>
            {
                object value = key.GetValue(p, null);
                if (value != null && value.ToString() != "0")
                {
                    sqlList.Add(UpdateSql<T>(p, key.Name) + $" WHERE {key.Name} = '{value}'");
                }
                else
                {
                    sqlList.Add(InsertSql<T>(p, key.Name));
                }
            });

            return DbHelper.ModifyByTrans(sqlList);
        }

        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string InsertSql<T>(T t, string keyName, string clobField = null)
        {
            if (t == null)
            {
                return string.Empty;
            }

            ////拼装Insert语句
            string sql = string.Empty;
            string sqlColumns = string.Empty;
            string sqlVallues = string.Empty;
            var properties = typeof(T).GetProperties();
            foreach (PropertyInfo item in properties)
            {
                if (item.Name == "CREATE_DATE")
                    continue;

                ////拼装SQL
                if (item.Name == keyName)
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
        /// 获取更新SQL(Oracle的日期处理)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string UpdateSql<T>(T t, string keyName, Func<PropertyInfo, bool> where = null)
        {
            StringBuilder sb = new StringBuilder(string.Format("Update {0} SET", t.GetType().Name));
            var properties = typeof(T).GetProperties();
            foreach (PropertyInfo subItem in properties)
            {
                if (subItem.Name == "CREATE_DATE" || subItem.Name == keyName || (where != null && !where(subItem)))
                    continue;

                var subValue = subItem.GetValue(t, null);

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
        /// <param name="ids">删除的ID</param>
        /// <param name="isLogic">是否逻辑删除</param>
        /// <returns></returns>
        public int DeleteEntityByIDs<T>(List<string> ids, bool isLogic = true, int stateValue = 1)
        {
            Type type = typeof(T);

            int index = 0;

            var property = GetKeyProperty(typeof(T));
            if (property != null)
            {
                string sql = $"DELETE FROM {type.Name} WHERE {property.Name} IN {ids.TryToWhere()}";
                if (isLogic)
                {
                    sql = $"UPDATE {type.Name} SET STATE={stateValue} WHERE {property.Name} IN {ids.TryToWhere()}";
                }

                index = DbHelper.Modify(sql);
            }

            return index;
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int GetCount<T>(string where = null)
        {
            string strSQL = $"Select Count(*) From {typeof(T).Name} ";
            if (!string.IsNullOrEmpty(where))
            {
                strSQL += where;
            }

            return DbHelper.GetCount(strSQL);
        }
        #endregion
    }
}