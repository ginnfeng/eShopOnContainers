////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/8/2011 3:59:00 PM 
// Description: EntityTableParser.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.DataContract;
using System.Reflection;
using Common.Support;
using Common.Support.Helper;
using Common.Support.Serializer;

namespace Common.DataCore
{
    public class EntityTableHelper
    {
        static public bool IsColumnChanged(EntityTable tbl)
        {
            if (tbl.OldSchema == null) return false;
            return DefaultTransfer.ToText(tbl.Rows) != DefaultTransfer.ToText(tbl.OldSchema.Rows);
        }
        static public bool IsNamingChanged(EntityTable tbl)
        {
            if (tbl.OldSchema == null) return false;
            return tbl.Namespace != tbl.OldSchema.Namespace || tbl.TableName != tbl.OldSchema.TableName;
        }
        static public EntityTable CreateEntitySchema(bool genDefaultKey=false)
        {
            var schema = EntityTableHelper<IEntitySchema>.CreateDataTable();
            schema.Columns["DataType"].DefaultValue=typeof(string).FullName;
            schema.Columns["FieldType"].DefaultValue = EntityFieldType.Var.ToString();
            if (genDefaultKey == true)
            {
                var tblProxy=new EntityTableProxy<IEntitySchema>(schema);
                var colDef=tblProxy.Append();
                colDef.Unique=true;
                colDef.KeyGen = "id_{0}";
                colDef.ColumnName = "Id";
                schema.TableName = "";
            }
            return schema;
        }
        static public EntityTable GenEntitySchema(EntityTable srcTbl)
        {
            var schemaTable = CreateEntitySchema();
            schemaTable.Namespace = srcTbl.Namespace;
            schemaTable.TableName = srcTbl.TableName;
            var schemaTableProxy = new EntityTableProxy<IEntitySchema>(schemaTable);
            foreach (var column in srcTbl.Columns)
            {
                var colDef = schemaTableProxy.Append();
                column.CopyTo(colDef);                
                //attris.Status = OPStatus.Steady;//在此代表此column為舊有非新增的
            }
            return schemaTable;
        }
        static public BaseTransfer DefaultTransfer { get { return transfer; } }
        
        static public string AutoGenKey(ref EntityRow row)
        {
            var table = row.Table;
            if (table == null) return null;
            
            var pk = row.Table.PrimaryKey;
            var keyGen = pk.GetAttributesEntity().KeyGen;
            if (string.IsNullOrEmpty(keyGen)) 
                return null;
            var tableAttri = table.GetAttributesEntity();
            long keyIndex = tableAttri.KeyCount + 1;
            var key = CommonExtension.StringFormat(keyGen, keyIndex);
            tableAttri.KeyCount = keyIndex;
            row.SetValue(table.PrimaryKey, key, true);
            return key;
            //Table.GetAttributesEntity().KeyCount
        }
        static private JsonTransfer transfer = new JsonTransfer();
    }
    public class EntityTableHelper<TEntity> : EntityTableHelper
    {
        static public List<Type> GetInterfaces()
        {
            List<Type> interfaces = new List<Type>();
            Type entityType = typeof(TEntity);
            foreach (var type in entityType.GetInterfaces())
            {
                if(type!=typeof(IDictionaryAccess))
                    interfaces.Add(type);
            }
            interfaces.Add(entityType);
            return interfaces;
        }
        static public EntityTable CreateDataTable(EntityTable schema)
        {
            var schemaProxy = new EntityTableProxy<IEntitySchema>(schema);
            EntityTable table = new EntityTable();
            foreach (var columnDef in schemaProxy)
            {
                var fieldType=Type.GetType(columnDef.DataType);
                AddColumn(ref table, fieldType, columnType => GenEntityColumn(columnDef, columnType));
            }
            return table;
        }
        static public EntityTable CreateDataTable()
        {                   
            EntityTable table = new EntityTable();
            foreach (var property in EntityPropertyInfos)
            {
                AddColumn(ref table, property.PropertyType, columnType => GenEntityColumn(property, columnType));
            }
            return table;
        }
       

        static public void AddColumn( ref EntityTable table,Type orignailType,Func<Type,EntityColumn> CreateColumn)
        {
            Type dataType = (CommonExtension.IsPrimitiveXmlType(orignailType))
                    ? orignailType : typeof(object);
            var column = CreateColumn(dataType);
            table.Columns.Add(column);
            //column.DefaultValue = (dataType.IsValueType) ? Activator.CreateInstance(dataType) : null;
        }
        static private List<PropertyInfo> entityPropertyInfos;
        static Type lockerSymbol = typeof(EntityTableHelper<TEntity>);
        static public List<PropertyInfo> EntityPropertyInfos
        {
            get
            {
                lock (lockerSymbol)
                {
                    if (entityPropertyInfos == null)
                    {
                        entityPropertyInfos = new List<PropertyInfo>();
                        foreach (var entityType in GetInterfaces())
                        {
                            foreach (var property in entityType.GetProperties())
                            {
                                if(entityPropertyInfos.Find(it=>it.Name.Equals(property.Name))==null)
                                    entityPropertyInfos.Add(property);    
                            }                            
                        }
                    }
                    return entityPropertyInfos;
                }
            }
        }
        static public void BindEntity(ref EntityTable table, IEntityTableSource dataTableSource)
        {
            try
            {
                UnifyTableColumns(ref  table);                
                //foreach (EntityRow row in table.Rows)
                //{
                //    BindRow(table, row, dataTableSource, propertyInfos);
                //}
            }
            catch (Exception error)
            {
                var errorMsg = CommonExtension.StringFormat("DataTableParser.BindEntity<T> T={0} Table={1}", typeof(TEntity).Name, table.TableName);
                throw new System.MissingMemberException(errorMsg, error);
            }

        }
        static public bool IsKeyProperty(Type entityType, string propertyName)
        {
            var propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo == null)
                return false;
            EntityPropertyAttribute entityPropertyAttribute = GetEntityPropertyAttribute(propertyInfo);
            return entityPropertyAttribute.IsKey;
        }
        static public object RetriveKey(TEntity entity)
        {
            Type type = typeof(TEntity);
            //PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in EntityPropertyInfos)
            {
                EntityPropertyAttribute entityPropertyAttribute = GetEntityPropertyAttribute(propertyInfo);
                if (entityPropertyAttribute.IsKey)
                {
                    return propertyInfo.GetValue(entity, null);
                }
            }
            throw new Exception("No define key!");
        }

        static public EntityPropertyAttribute GetEntityPropertyAttribute<TMemberInfo>(TMemberInfo propertyInfo)
            where TMemberInfo : MemberInfo
        {
            EntityPropertyAttribute attri;
            if (!entityPropertyAttributeMap.TryGetValue(propertyInfo, out attri))
            {
                if (!AttributeHelper.TryGetCustomAttribute(propertyInfo, out attri))
                {
                    attri = new EntityPropertyAttribute() { ColumnName = propertyInfo.Name };
                }
                else
                {
                    if (attri.ColumnIndex < 0 && string.IsNullOrEmpty(attri.ColumnName))
                    {
                        attri.ColumnName = propertyInfo.Name;
                    }
                }
                entityPropertyAttributeMap[propertyInfo] = attri;
            }
            return attri;
        }
        private static void UnifyTableColumns(ref EntityTable table)
        {
            Type type = typeof(TEntity);

            EntityAttribute entityAttribute = EntityAttribute.GetEntityAttribute(type);
            //PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in EntityPropertyInfos)
            {
                EntityPropertyAttribute entityPropertyAttribute = GetEntityPropertyAttribute(propertyInfo);
                EntityColumn column = (entityAttribute.BindingByIndex) ? table.Columns[entityPropertyAttribute.ColumnIndex] : table.Columns[entityPropertyAttribute.ColumnName];
                if (column == null)
                {
                    var errorMsg = CommonExtension.StringFormat("mapping error: property={0} column={1}", propertyInfo.Name, entityPropertyAttribute.ColumnName);
                    throw new Exception(errorMsg);
                }
                column.ColumnName = propertyInfo.Name;//此保證透過entity proxy機制能取得正確值
            }
            //table.BindingEntityType = type;
        }
        static private EntityColumn GenEntityColumn(IEntitySchema schema, Type columnType)
        {
            var column = new EntityColumn();
            schema.CopyTo(column);
            return column;
        }
        static private EntityColumn GenEntityColumn(PropertyInfo propertyInfo, Type columnType)
        {            
            var entityPropertyAttribute = GetEntityPropertyAttribute(propertyInfo);
            var defaultValue = (columnType.IsValueType) ? Activator.CreateInstance(columnType) : null;
            return GenEntityColumn(propertyInfo.Name, columnType, defaultValue, entityPropertyAttribute);
        }
        
        static public EntityColumn GenEntityColumn(string name, Type columnType,object defaultValue, EntityPropertyAttribute entityPropertyAttribute)
        {
            var column = new EntityColumn(name, columnType) { DefaultValue = defaultValue };

            if (entityPropertyAttribute != null)
            {
                var columnAttributes = column.GetAttributesEntity();
                columnAttributes.KeyGen = entityPropertyAttribute.KeyGen;
                column.Unique = entityPropertyAttribute.IsKey;
            }
            return column;
        }
        
        static private Dictionary<MemberInfo, EntityPropertyAttribute> entityPropertyAttributeMap = new Dictionary<MemberInfo, EntityPropertyAttribute>();
    }
}
