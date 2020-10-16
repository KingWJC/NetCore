using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADF.Utility;
using ADF.IBusiness;
using ADF.DataAccess.ORM;

namespace ADF.Business
{
    public abstract class BaseBusiness<T> where T : class, new()
    {
        public DbContext Service { get; set; }

        public BaseBusiness()
        {
            SetService(null, null);
        }

        public BaseBusiness(DatabaseType? type, string conStr)
        {
            SetService(type, conStr);
        }

        public void SetService(DatabaseType? type, string conStr)
        {
            Service = new DbContext(type, conStr);
        }

        #region 添加数据
        public void Insert(T entity)
        {
            Service.Insert<T>(entity);
        }

        public void Insert(List<T> entities)
        {
            Service.Insert<T>(entities);
        }

        public void BulkInsert(List<T> entities)
        {
            Service.BulkInsert<T>(entities);
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void DeleteAll()
        {
            Service.DeleteAll<T>();
        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            Service.Delete<T>(new List<string> { key });
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public void Delete(List<string> keys)
        {
            Service.Delete<T>(keys);
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void Delete(T entity)
        {
            Service.Delete<T>(entity);
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public void Delete(List<T> entities)
        {
            Service.Delete<T>(entities);
        }
        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            Service.Update<T>(entity);
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        public void Update(List<T> entities)
        {
            Service.Update<T>(entities);
        }

        /// <summary>
        /// 更新单条数据指定属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        public void UpdateAny(T entity, List<string> properties)
        {
            Service.UpdateAny<T>(entity, properties);
        }

        /// <summary>
        /// 更新多条数据执行属性
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        public void UpdateAny(List<T> entities, List<string> properties)
        {
            Service.UpdateAny<T>(entities, properties);
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 通过主键获取单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public T GetEntity(params object[] keyValue)
        {
            return Service.GetEntity<T>(keyValue);
        }

        /// <summary>
        /// 获取所有数据
        /// 注:会获取所有数据,数据量大请勿使用
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            return Service.GetList<T>();
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql)
        {
            return Service.GetDataTableWithSql(sql);
        }

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetDataTableWithSql(string sql, List<CusDbParameter> parameters)
        {
            return Service.GetDataTableWithSql(sql, parameters);
        }

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <returns></returns>
        public List<U> GetListBySql<U>(string sqlStr) where U : class, new()
        {
            return Service.GetListBySql<U>(sqlStr);
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
            return Service.GetListBySql<U>(sqlStr, param);
        }

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        public int ExecuteSql(string sql)
        {
            return Service.ExecuteSql(sql);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="spList">参数</param>
        public int ExecuteSql(string sql, List<CusDbParameter> cusDbParameters)
        {
            return Service.ExecuteSql(sql, cusDbParameters);
        }
        #endregion

        #region 事务处理
        public ITransaction BeginTransaction()
        {
            return Service.BeginTransaction();
        }
        public ITransaction BeginTransaction(IsolationLevel level)
        {
            return Service.BeginTransaction(level);
        }

        public void CommitTransaction()
        {
            Service.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Service.RollbackTransaction();
        }

        public (bool, Exception) EndTransaction()
        {
            return Service.EndTransaction();
        }
        #endregion
    }
}