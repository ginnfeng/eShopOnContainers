////*************************Copyright © 2008 Feng 豐**************************
// Created    : 6/26/2009 11:03:08 AM
// Description: DataTableProxy.cs
// Revisions  :
// ****************************************************************************
using System;
using System.Collections.Generic;
using System.Data;

using Support.Net.LINQ;
using Support.Helper;
using Support.Net.Proxy;
using System.Reflection;
using Support;

namespace Common.DataCore
{
    public class DataTableProxy<TEntity>
    {
        public DataTableProxy()
            : this(DataTableHelper.Instance.CreateDataTable<TEntity>())
        {
        }

        public DataTableProxy(DataTable dataTable)
        {
            Table = dataTable;
        }

        public DataTable Table { get; private set; }

        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (DataRow row in Table.Rows)
            {
                yield return row.GenEntityProxy<TEntity>();
            }
        }

        public bool ContainRow(string filterExpression, params object[] parameters)
        {
            string expression = CommonExtension.StringFormat(filterExpression, parameters);
            return Table.Select(expression).Length != 0;
        }

        public bool Contain(Func<TEntity, bool> func)
        {
            TEntity entity = Find(func);
            return entity != null;
        }

        public TEntity Find(Func<TEntity, bool> func)
        {
            foreach (var entity in this)
            {
                if (func(entity))
                    return entity;
            }
            return default(TEntity);
        }

        public TEntity Find(params object[] keyValues)
        {
            var row = Table.Rows.Find(keyValues);
            if (row == null)
                return default(TEntity);
            return row.GenEntityProxy<TEntity>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filterExpression">ex1: "FieldName='xxx'"  ex2: "FieldName is NULL"</param>
        /// <returns></returns>
        public List<TEntity> Select(string filterExpression)
        {
            List<TEntity> entityList = new List<TEntity>();
            var rows = Table.Select(filterExpression);
            foreach (var row in rows)
            {
                entityList.Add(row.GenEntityProxy<TEntity>());
            }
            return entityList;
        }

        public List<TEntity> SelectAll()
        {
            List<TEntity> entityList = new List<TEntity>();
            var rows = Table.Select();
            foreach (var row in rows)
            {
                entityList.Add(row.GenEntityProxy<TEntity>());
            }
            return entityList;
        }

        public List<TEntity> Select(Func<TEntity, bool> func)
        {
            List<TEntity> entityList = new List<TEntity>();
            foreach (var entity in this)
            {
                if (func(entity))
                    entityList.Add(entity);
            }
            return entityList;
        }

        public TEntity Append()
        {
            object[] valueList = new object[Table.Columns.Count];
            Table.Rows.Add(valueList);
            return Last();
        }

        public void Append(TEntity entity)
        {
            object[] valueList = new object[Table.Columns.Count];
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                valueList.SetValue(ObjectDelegate.GetPropertyValue(entity, Table.Columns[i].ColumnName), i);
            }
            Table.Rows.Add(valueList);
        }

        public TEntity this[int index]
        {
            get
            {
                return Table.Rows[index].GenEntityProxy<TEntity>();
            }
        }

        public TEntity First()
        {
            return this[0];
        }

        public TEntity Last()
        {
            return this[Count - 1];
        }

        public int Count
        {
            get { return Table.Rows.Count; }
        }
    }

    static public class DataRowExtension
    {
        static public TEntity GenEntityProxy<TEntity>(this DataRow it)
        {
            GetPropertyDelegate getMethod = (x, y) => y;
            return RealProxyGen.GenEntityProxy<TEntity>
                (
                (methodInfo, name) => DoGetPropertyValue(methodInfo, it[name]),
                (methodInfo, name, value) => it[name] = value
                );
        }

        static private object DoGetPropertyValue(MethodInfo methodInfo, object retValue)
        {
            if (methodInfo == null) return retValue;
            var propertyType = methodInfo.ReturnType;
            if (retValue == null || retValue is DBNull)
            {
                retValue = (propertyType.IsValueType) ? Activator.CreateInstance(propertyType) : null;
            }
            if (retValue == null || propertyType.IsAssignableFrom(retValue.GetType()))
            {
                return retValue;
            }
            return CommonExtension.ToObject(retValue.ToString(), propertyType);
        }
    }
}