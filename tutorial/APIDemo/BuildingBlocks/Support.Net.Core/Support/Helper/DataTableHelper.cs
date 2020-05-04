////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/19/2011 10:10:41 AM 
// Description: DataTableHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;

namespace Support.Helper
{
    public class DataTableHelper
    {
        static public DataTableHelper Instance
        {
            get
            {
                return Singleton<DataTableHelper>.Instance;
            }
        }

        public DataTable CreateDataTable(IEnumerable sampleList, Func<object, string> columnNameFunc, Func<object, string> captionNameFunc)
        {
            DataTable table = new DataTable();
            foreach (var item in sampleList)
            {
                var column = table.Columns.Add(columnNameFunc(item), item.GetType());
                if (captionNameFunc != null)
                    column.Caption = captionNameFunc(item);
            }
            return table;
        }

        public DataTable ConvertToDataTable<TFields>(IEnumerable<TFields> entitys, Func<object, string> columnNameFunc, Func<object, string> captionNameFunc)
            where TFields : IEnumerable
        {
            DataTable table = null;
            int count = 0;
            foreach (var fieldValues in entitys)
            {
                if (++count == 1)
                    table = CreateDataTable(fieldValues, columnNameFunc, captionNameFunc);
                List<object> values = new List<object>();
                foreach (var value in fieldValues)
                {
                    values.Add(value);
                }

                table.Rows.Add(values.ToArray());
            }
            return table;
        }
        public DataTable CreateDataTable<TEntity>()
        {
            return CreateDataTable<TEntity, DataTable>();
        }

        public TDataTable CreateDataTable<TEntity, TDataTable>()
            where TDataTable : DataTable
        {
            Type entityType = typeof(TEntity);
            DataTable table;
            lock (this)
            {
                if (!tableMap.TryGetValue(entityType, out table) || typeof(TDataTable) != table.GetType())
                {
                    table = new DataTable();
                    foreach (var property in entityType.GetProperties())
                    {
                        var column = table.Columns.Add(property.Name, property.PropertyType);
                        column.DefaultValue = null;
                    }
                    tableMap[entityType] = table;
                }
                return table.Clone() as TDataTable;
            }
        }

        public DataTable ConvertToDataTable<TEntity>(IEnumerable<TEntity> entitys)
        {
            Type entityType = typeof(TEntity);
            DataTable table = CreateDataTable<TEntity>();
            foreach (var entity in entitys)
            {
                DataRow dataRow = table.Rows.Add();
                var properties = entityType.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    dataRow[i] = properties[i].GetValue(entity, null);
                }
            }
            return table;
        }
        public TList ConvertToList<TEntity, TList>(DataTable dataTable)
            where TEntity : new()
            where TList : IList, new()
        {
            Type entityType = typeof(TEntity);
            var properties = entityType.GetProperties();
            TList list = new TList();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                TEntity entity = new TEntity();
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i].SetValue(entity, dataRow[i], null);
                }
                list.Add(entity);
            }
            return list;
        }

        public List<TEntity> ConvertToList<TEntity>(DataTable dataTable)
            where TEntity : new()
        {
            return ConvertToList<TEntity, List<TEntity>>(dataTable);
        }

        Dictionary<Type, DataTable> tableMap = new Dictionary<Type, DataTable>();
    }
}
