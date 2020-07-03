using System.Data;
using System.Collections.Generic;
using ADF.DataAccess.ORM;

namespace ADF.Business
{
    public interface IBaseBusiness<T> : ITransaction where T : class, new()
    {
        #region 增加数据

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Insert(T entity);

        /// <summary>
        /// 添加多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        void Insert(List<T> entities);

        /// <summary>
        /// 批量添加数据,速度快
        /// </summary>
        /// <param name="entities"></param>
        void BulkInsert(List<T> entities);

        #endregion

        #region 删除数据
        /// <summary>
        /// 删除所有数据
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        void Delete(List<string> keys);

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Delete(T entity);

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        void Delete(List<T> entities);
        #endregion

        #region 更新数据

        /// <summary>
        /// 更新单条数据
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities"></param>
        void Update(List<T> entities);

        /// <summary>
        /// 更新单条数据指定属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="properties">属性</param>
        void UpdateAny(T entity, List<string> properties);

        /// <summary>
        /// 更新多条数据执行属性
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="properties">属性</param>
        void UpdateAny(List<T> entities, List<string> properties);
        #endregion

        #region 查询数据

        /// <summary>
        /// 通过主键获取单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        T GetEntity(params object[] keyValue);

        /// <summary>
        /// 获取所有数据
        /// 注:会获取所有数据,数据量大请勿使用
        /// </summary>
        /// <returns></returns>
        List<T> GetList();

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        DataTable GetDataTableWithSql(string sql);

        /// <summary>
        /// 通过SQL获取DataTable
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataTable GetDataTableWithSql(string sql, List<CusDbParameter> parameters);

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <returns></returns>
        List<U> GetListBySql<U>(string sqlStr) where U : class, new();

        /// <summary>
        /// 通过SQL获取List
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="sqlStr">SQL</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        List<U> GetListBySql<U>(string sqlStr, List<CusDbParameter> param) where U : class, new();

        #endregion

        #region 执行Sql语句

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        int ExecuteSql(string sql);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="cusDbParameters">参数</param>
        int ExecuteSql(string sql, List<CusDbParameter> cusDbParameters);

        #endregion
    }
}