using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADF.Utility;

namespace ADF.DataAccess.ORM
{
    public class DbContext : ITransaction, IDisposable
    {
        private DbTransaction dbTransaction;
        private DatabaseType? databaseType;
        private string connectionStr;
        private DbHelper _db;
        private Action transactionHandler;
        protected bool Disposed = false;
        protected bool OpenTransaction = false;
        public DbHelper Db
        {
            get
            {
                if (Disposed || _db == null)
                {
                    _db = DbHelperFactory.GetDbHelper(databaseType, connectionStr);
                }
                return _db;
            }
        }

        public DbContext(DatabaseType? dbType, string conStr)
        {
            this.databaseType = dbType;
            this.connectionStr = conStr;
        }

        #region 私有方法
        protected PropertyInfo GetKeyProperty(Type type)
        {
            return GetKeyPropertys(type).FirstOrDefault();
        }
        protected List<PropertyInfo> GetKeyPropertys(Type type)
        {
            var properties = type
                .GetProperties()
                .Where(x => x.GetCustomAttributes(true).Select(o => o.GetType().FullName).Contains(typeof(KeyAttribute).FullName))
                .ToList();

            return properties;
        }

        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected (string, List<CusDbParameter>) GenerateInsertSQL<T>(T t, string keyName)
        {
            if (t == null)
            {
                return (null, null);
            }

            Type type = typeof(T);
            //拼装Insert语句
            StringBuilder sqlColumns = new StringBuilder(), sqlVallues = new StringBuilder();
            List<CusDbParameter> parameters = new List<CusDbParameter>();
            var properties = type.GetProperties();
            foreach (PropertyInfo item in properties)
            {
                if (item.Name == "CREATE_DATE")
                    continue;

                CusDbParameter cusDbParameter = null;
                object obj = item.GetValue(t, null);
                if (item.Name == keyName && obj.IsNullOrEmpty())
                {
                    string id = GetNextId("SEQ_STATUS");
                    item.SetValue(t, id, null);
                    cusDbParameter = new CusDbParameter($"{Db.ParaPrefix + item.Name}", id, DbType.Int32);
                }
                else
                {
                    cusDbParameter = new CusDbParameter($"{Db.ParaPrefix + item.Name}", obj ?? DBNull.Value, GetDataType(item.PropertyType));
                }

                parameters.Add(cusDbParameter);
                sqlColumns.Append($"\"{item.Name}\",");
                sqlVallues.Append($"{Db.ParaPrefix + item.Name},");
            }

            string insertSQL = string.Format("INSERT INTO {0}({1}) VALUES({2})", GetTableName(type), sqlColumns.ToString().TrimEnd(','), sqlVallues.ToString().TrimEnd(','));

            return (insertSQL, parameters);
        }

        private string GetTableName(Type type)
        {
            string tableName = string.Empty;
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
                tableName = tableAttribute.Name;
            else
                tableName = type.Name;
            return tableName;
        }

        private DbType GetDataType(Type type)
        {
            if (type == Constants.ByteArrayType)
            {
                return System.Data.DbType.Binary;
            }
            else if (type == Constants.GuidType)
            {
                return System.Data.DbType.Guid;
            }
            else if (type == Constants.IntType)
            {
                return System.Data.DbType.Int32;
            }
            else if (type == Constants.ShortType)
            {
                return System.Data.DbType.Int16;
            }
            else if (type == Constants.LongType)
            {
                return System.Data.DbType.Int64;
            }
            else if (type == Constants.DateType)
            {
                return System.Data.DbType.DateTime;
            }
            else if (type == Constants.DobType)
            {
                return System.Data.DbType.Double;
            }
            else if (type == Constants.DecType)
            {
                return System.Data.DbType.Decimal;
            }
            else if (type == Constants.ByteType)
            {
                return System.Data.DbType.Byte;
            }
            else if (type == Constants.FloatType)
            {
                return System.Data.DbType.Single;
            }
            else if (type == Constants.BoolType)
            {
                return System.Data.DbType.Boolean;
            }
            else if (type == Constants.StringType)
            {
                return System.Data.DbType.String;
            }
            else if (type == Constants.DateTimeOffsetType)
            {
                return System.Data.DbType.DateTimeOffset;
            }
            else
            {
                return System.Data.DbType.String;
            }

        }

        /// <summary>
        /// 获取更新SQL
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected (string, List<CusDbParameter>) GenerateUpdateSQL<T>(T t, string keyName, Func<PropertyInfo, bool> where = null)
        {
            StringBuilder updateSql = new StringBuilder($"Update {GetTableName(t.GetType())} SET ");
            List<CusDbParameter> parameters = new List<CusDbParameter>();
            var properties = typeof(T).GetProperties();
            foreach (PropertyInfo subItem in properties)
            {
                if (subItem.Name == "CREATE_DATE" || subItem.Name == keyName
                || (where != null && !where(subItem)))
                    continue;

                var subValue = subItem.GetValue(t, null);
                parameters.Add(new CusDbParameter($"{Db.ParaPrefix + subItem.Name}", subValue ?? DBNull.Value, GetDataType(subItem.PropertyType)));
                updateSql.Append($"\"{subItem.Name}\" = {Db.ParaPrefix + subItem.Name},");
            }

            return (updateSql.ToString().TrimEnd(','), parameters);
        }

        /// <summary>
        /// 获取删除SQL
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string GenerateDeleteSQL<T>(List<string> keys, bool isLogic = false)
        {
            Type type = typeof(T);
            var property = GetKeyProperty(type);
            string sql = string.Empty;
            if (property != null)
            {
                sql = $"DELETE FROM {GetTableName(type)} WHERE {property.Name} IN {keys.TryToWhere()}";
                if (isLogic)
                {
                    sql = $"UPDATE {GetTableName(type)} SET DeleteFlag= 1 WHERE {property.Name} IN {keys.TryToWhere()}";
                }
            }
            return sql;
        }

        /// <summary>
        /// 从特定sequence中获取值
        /// </summary>
        /// <param name="seqName">序列名称</param>
        /// <returns>获取到的sequence值</returns>
        protected string GetNextId(string seqName)
        {
            string strSQL = $"SELECT {seqName}.NEXTVAL MAXID  FROM DUAL";
            object obj = Db.ExecuteScalar(strSQL);
            return obj.ToString();
        }

        private void PackWork(Action work)
        {
            if (OpenTransaction)
            {
                transactionHandler += work;
            }
            else
            {
                work();
                Dispose();
            }
        }
        #endregion

        #region 事务处理
        public ITransaction BeginTransaction()
        {
            dbTransaction = Db.UseTransation();
            return this;
        }

        public ITransaction BeginTransaction(IsolationLevel level)
        {
            dbTransaction = Db.UseTransation(level);
            return this;
        }

        public void CommitTransaction()
        {
            dbTransaction?.Commit();
        }

        public void RollbackTransaction()
        {
            dbTransaction?.Rollback();
        }

        public (bool success, Exception exception) EndTransaction()
        {
            bool success = true;
            Exception exception = null;
            try
            {
                transactionHandler?.Invoke();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                success = false;
                exception = ex;
                RollbackTransaction();
            }
            finally
            {
                Dispose();
            }
            return (success, exception);
        }
        #endregion

        #region 添加数据
        public void Insert<T>(T entity)
        {
            Insert(new List<T> { entity });
        }

        public void Insert<T>(List<T> entities)
        {
            string keyName = GetKeyProperty(typeof(T))?.Name;
            PackWork(() =>
            {
                entities.ForEach(p =>
                {
                    var (InsertSQL, Parameters) = GenerateInsertSQL(p, keyName);
                    Db.ExecuteNonQuery(InsertSQL, Parameters);
                });
            });

        }

        public void BulkInsert<T>(List<T> entities)
        {
            PackWork(() =>
            {
                Db.ExecuteBulkCopy(typeof(T).Name, null);
            });
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void DeleteAll<T>()
        {
            var deleteSQL = $"Truncate Table {typeof(T).Name}";
            PackWork(() =>
            {
                Db.ExecuteNonQuery(deleteSQL);
            });

        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            Delete(new List<string> { key });
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public void Delete<T>(List<string> keys)
        {
            var deleteSQL = GenerateDeleteSQL<T>(keys);
            PackWork(() =>
            {
                Db.ExecuteNonQuery(deleteSQL);
            });
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void Delete<T>(T entity)
        {
            Delete(new List<T> { entity });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public void Delete<T>(List<T> entities)
        {
            PropertyInfo keyInfo = GetKeyProperty(typeof(T));
            List<string> keys = entities.Select(p => keyInfo.GetValue(p).ToString()).ToList();
            Delete(keys);
        }
        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity"></param>
        public void Update<T>(T entity)
        {
            Update(new List<T> { entity });
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        public void Update<T>(List<T> entities)
        {
            PackWork(() =>
            {
                PropertyInfo keyInfo = GetKeyProperty(typeof(T));
                entities.ForEach(p =>
                {
                    object value = keyInfo?.GetValue(p, null);
                    if (!value.IsNullOrEmpty() && value.ToString() != "0")
                    {
                        var (UpdateSQL, Parameters) = GenerateUpdateSQL(p, keyInfo?.Name);
                        Db.ExecuteNonQuery(UpdateSQL, Parameters);
                    }
                    else
                    {
                        (string, List<CusDbParameter>) result = GenerateInsertSQL(p, keyInfo?.Name);
                        Db.ExecuteNonQuery(result.Item1, result.Item2);
                    }
                });
            });
        }

        /// <summary>
        /// 更新单条数据指定属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        public void UpdateAny<T>(T entity, List<string> properties)
        {
            UpdateAny<T>(new List<T> { entity }, properties);
        }

        /// <summary>
        /// 更新多条数据执行属性
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        public void UpdateAny<T>(List<T> entities, List<string> properties)
        {
            PropertyInfo keyInfo = GetKeyProperty(typeof(T));
            PackWork(() =>
            {
                entities.ForEach(entity =>
                {
                    var (UpdateSQL, Parameters) = GenerateUpdateSQL(entity, keyInfo?.Name, p => properties.Contains(p.Name));
                    Db.ExecuteNonQuery(UpdateSQL, Parameters);
                });
            });
        }
        #endregion

        #region 查询数据

        /// <summary>
        /// 通过主键获取单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public T GetEntity<T>(params object[] keyValue) where T : class, new()
        {
            Type type = typeof(T);
            string keyName = GetKeyProperty(type)?.Name;
            string columnNameStr = string.Join(",", type.GetProperties().Select(p => $"{p.Name}"));
            string selectSQL = $"Select {columnNameStr} From {GetTableName(type)} Where {keyName} IN {keyValue.TryToWhere()}";
            List<T> list = Db.ExecuteDataTable(selectSQL).ToList<T>();
            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        /// <summary>
        /// 获取所有数据
        /// 注:会获取所有数据,数据量大请勿使用
        /// </summary>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            Type type = typeof(T);
            string columnNameStr = string.Join(",", type.GetProperties().Select(p => $"\"{p.Name}\""));
            string selectSQL = $"Select {columnNameStr} From {GetTableName(type)}";
            DataTable dataTable = Db.ExecuteDataTable(selectSQL);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql)
        {
            return Db.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql, List<CusDbParameter> parameters)
        {
            return Db.ExecuteDataTable(sql, parameters);
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr) where U : class, new()
        {
            return Db.ExecuteDataTable(sqlStr).ToList<U>();
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr, List<CusDbParameter> param) where U : class, new()
        {
            return Db.ExecuteDataTable(sqlStr, param).ToList<U>();
        }

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        public int ExecuteSql(string sql)
        {
            return Db.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="spList">参数</param>
        public int ExecuteSql(string sql, List<CusDbParameter> cusDbParameters)
        {
            return Db.ExecuteNonQuery(sql, cusDbParameters);
        }

        #endregion

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Db?.Dispose();
                dbTransaction?.Dispose();
            }
            OpenTransaction = false;
            transactionHandler = null;
            Disposed = true;
        }

        ~DbContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}