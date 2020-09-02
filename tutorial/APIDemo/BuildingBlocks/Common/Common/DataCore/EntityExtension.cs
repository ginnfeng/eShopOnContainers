////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/4/2011 1:32:55 PM 
// Description: EntityExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataContract;

namespace Common.DataCore
{
    static public class EntityExtension
    {
        static public List<EntityTable> GroupToTables(this IList<EntityRow> rows)
        {
            List<EntityTable> tables = new List<EntityTable>();
            var rowGroupMap = rows.GroupBy(row => row.Table);
            foreach (IGrouping<EntityTable, EntityRow> rowGroup in rowGroupMap)
            {
                var table = rowGroup.Key.Clone(false);
                var tableAttributes=table.GetAttributesEntity();
                foreach (var row in rowGroup.ToList())
                {
                    // 100/08/26 by Feng
                    //long maxKeyCount=row.Table.GetAttributesEntity().KeyCount;
                    //if (maxKeyCount > tableAttributes.KeyCount)
                    //    tableAttributes.KeyCount = maxKeyCount;
                    table.Append(row);
                }
                
                tables.Add(table);
            }
            return tables;
        }
        static public void CleanColumnDefaultValue(this EntityTable table)
        {
            table.Columns.ForEach(col => col.DefaultValue = null);
        }
        static public EntityCatalog[] GenEntityCatalogs(this IList<EntityTable> tables)
        {
            List<EntityCatalog> catalogs = new List<EntityCatalog>();
            var tableGroupMap = tables.GroupBy(table => table.Namespace);
            foreach (IGrouping<string, EntityTable> tableGroup in tableGroupMap)
            {
                var catalog = new EntityCatalog() { Name = tableGroup.Key };
                foreach (var entityTable in tableGroup.ToList())
                {
                    var digest =entityTable.GenEntityDigest();
                    catalog.Digests.Add(digest);
                }
                catalogs.Add(catalog);
            }
            return catalogs.ToArray();
        }
        static public EntityDigest GenEntityDigest(this EntityTable table)
        {
            return new EntityDigest() { Key = table.TableName, Version = table.Version };
        }
        static public EntityTableProxy<TEntity> GenProxy<TEntity>(this EntityTable table, IEntityTableSource entityTableSource = null)
        {
            return new EntityTableProxy<TEntity>(table, entityTableSource);
        }
        static public EntityRowProxy<TEntity> GenProxy<TEntity>(this EntityRow dataRow, IEntityTableSource entityTableSource = null)
        {
            string proxyKey = typeof(EntityRowProxy<TEntity>).FullName;
            EntityRowProxy<TEntity> proxy;
            if (!dataRow.TryGetCache(proxyKey, out proxy))
            {
                proxy = new EntityRowProxy<TEntity>(dataRow, entityTableSource);
                dataRow.AddCache(proxyKey, proxy);
            }
            return proxy;
        }
        static public TEntity GetEntity<TEntity>(this EntityRow dataRow, IEntityTableSource entityTableSource = null)
        {
            return dataRow.GenProxy<TEntity>(entityTableSource).Entity;
        }
        static public TEntity GetEntity<TEntity>(this EntityDigest digest)
        {
            var proxy = digest.GenProxy();
            return proxy.GenEntityProxy<TEntity>();
        }
        static public StringPairProxy GenProxy(this EntityDigest digest)
        {
            var stringPairProxy = new StringPairProxy(digest.Digest);
            stringPairProxy.StringPairUpdatedEvent += updatedStringPair => digest.Digest = updatedStringPair;
            return stringPairProxy;
        }
        static public IEntityTableAttributes GetAttributesEntity(this EntityTable table)
        {
            return GetAttributesEntity<EntityTable, IEntityTableAttributes>(table);
        }
        static public IEntityColumnAttributes GetAttributesEntity(this EntityColumn column)
        {
            return GetAttributesEntity<EntityColumn, IEntityColumnAttributes>(column);
        }
        static public IEntityRowAttributes GetAttributesEntity(this EntityRow table)
        {
            return GetAttributesEntity<EntityRow, IEntityRowAttributes>(table);
        }
        static public TIAttributes GetAttributesEntity<Entity, TIAttributes>(Entity entity)
        {
            dynamic it = entity;
            var stringPairProxy = new StringPairProxy(it.Attributes);
            stringPairProxy.StringPairUpdatedEvent += updatedStringPair => it.Attributes = updatedStringPair;
            return stringPairProxy.GenEntityProxy<TIAttributes>();
        }

        static public void CopyTo(this EntityColumn it, IEntitySchema columnDef)
        {
            var attris = it.GetAttributesEntity();
            columnDef.ColumnName = it.ColumnName;
            columnDef.DataType = string.IsNullOrEmpty(it.DataType) ? defaultDataType : it.DataType;
            columnDef.FieldType =attris.FieldType.ToString();
            columnDef.AliasType = attris.AliasType;
            columnDef.KeyGen = attris.KeyGen;
            columnDef.Unique = it.Unique;           
            columnDef.DefaultValue = (it.DefaultValue == null) ? null : it.DefaultValue.ToString();
        }
        static public void CopyTo(this IEntitySchema it,EntityColumn column)
        {
            var attris = column.GetAttributesEntity();
            column.ColumnName = it.ColumnName;            
            if (!string.IsNullOrEmpty(it.FieldType))
                attris.FieldType = (EntityFieldType)Enum.Parse(typeof(EntityFieldType), it.FieldType,true);
             attris.AliasType=it.AliasType;
            column.DataType = (attris.FieldType == EntityFieldType.Var && string.IsNullOrEmpty(it.DataType)) ? defaultDataType : it.DataType;
            attris.KeyGen = it.KeyGen;            
            column.Unique = it.Unique;
            column.DefaultValue = string.IsNullOrEmpty(it.DefaultValue) ? null : it.DefaultValue;

        }
        
       private static string defaultDataType=typeof(string).FullName; 
    }
}
