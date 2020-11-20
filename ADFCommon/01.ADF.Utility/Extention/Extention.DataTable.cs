using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ADF.Utility
{
    public static partial class Extention
    {
        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();

            //确认参数有效,若无效则返回Null
            if (dt == null)
                return list;
            else if (dt.Rows.Count == 0)
                return list;

            Dictionary<string, string> dicField = new Dictionary<string, string>();
            Dictionary<string, string> dicProperty = new Dictionary<string, string>();
            Type type = typeof(T);

            //创建字段字典，方便查找字段名
            type.GetFields().ForEach(aFiled =>
            {
                dicField.Add(aFiled.Name.ToLower(), aFiled.Name);
            });

            //创建属性字典，方便查找属性名
            type.GetProperties().ForEach(aProperty =>
            {
                dicProperty.Add(aProperty.Name.ToLower(), aProperty.Name);
            });

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                T _t = Activator.CreateInstance<T>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string memberKey = dt.Columns[j].ColumnName.ToLower();

                    //字段赋值
                    if (dicField.ContainsKey(memberKey))
                    {
                        FieldInfo theField = type.GetField(dicField[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if (dbValue != null)
                        {
                            Type memberType = theField.FieldType;
                            dbValue = dbValue.ChangeType_ByConvert(memberType);
                        }
                        theField.SetValue(_t, dbValue);
                    }
                    //属性赋值
                    if (dicProperty.ContainsKey(memberKey))
                    {
                        PropertyInfo theProperty = type.GetProperty(dicProperty[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if (dbValue != null)
                        {
                            Type memberType = theProperty.PropertyType;
                            dbValue = dbValue.ChangeType_ByConvert(memberType);
                        }
                        theProperty.SetValue(_t, dbValue);
                    }
                }
                list.Add(_t);
            }
            return list;
        }

        /// <summary>
        /// DataRow转实体
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="row">数据源</param>
        /// <param name="timeFormat">日期格式</param>
        /// <returns></returns>
        public static T CreateItem<T>(DataRow row, string timeFormat = "")
        {
            string columnName;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    //Get property with same columnName
                    PropertyInfo prop = obj.GetType().GetProperty(columnName);
                    try
                    {
                        if (prop == null) continue;
                        //Get value for the column
                        object value = (row[columnName].GetType() == typeof(DBNull))
                        ? null : row[columnName];
                        //Set property value
                        if (prop.CanWrite)    //判断其是否可写
                        {
                            if (string.IsNullOrEmpty(timeFormat))
                            {
                                // prop.SetValue(obj, value!=null?Convert.ChangeType(value, prop.PropertyType):null, null);
                                if (value != null && value.GetType().FullName == "System.DateTime")
                                {
                                    prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                }
                                else if (value != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
                                {
                                    prop.SetValue(obj, Int32.Parse(value.ToString()), null);
                                }
                                else if (value != null && prop.PropertyType.Name == "Decimal")
                                {
                                    prop.SetValue(obj, Decimal.Parse(value.ToString()), null);
                                }
                                else
                                {
                                    //prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                    prop.SetValue(obj, value, null);
                                }
                            }
                            else
                            {
                                DateTime dt;
                                if (value != null
                                    && value.GetType().FullName == "System.DateTime"
                                    && !string.IsNullOrEmpty(value.ToString())
                                    && DateTime.TryParse(value.ToString(), out dt))
                                {
                                    prop.SetValue(obj, Convert.ChangeType(dt.ToString(timeFormat), prop.PropertyType), null);
                                }
                                else
                                {
                                    //prop.SetValue(obj, value, null);
                                    prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                        //Catch whatever here
                    }
                }
            }
            return obj;
        }
    }
}