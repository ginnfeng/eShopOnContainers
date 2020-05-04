////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/15/2011 10:09:43 AM 
// Description: ForeignFieldBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Common.DataContract;
using Support.Serializer;
using System.Collections.Specialized;

namespace Common.DataCore
{
    public abstract class ForeignFieldBase<TEntity> : ConverterFieldBase, INotifyPropertyChanged, IForeignSetBaseField
    {
        public ForeignFieldBase()
        {
        }
        abstract protected void FireResetNotifyCollection();

        override  public void Initializing()
        {
            IsLoadedForeignRelation = !string.IsNullOrEmpty(base.Content);
            this.EntityRelation = BuildEntityRelation(base.Row, base.Content);
            if (Column == null) return;
            foreignColumnAttributes = Column.GetAttributesEntity();
            foreignColumnAttributes.FieldType = EntityFieldType.Foreign;
            if (string.IsNullOrEmpty(this.EntityRelation.Namespace))
            {
                this.EntityRelation.Namespace = itemEntityAttribute.Namespace;             
            }
            if (string.IsNullOrEmpty(this.EntityRelation.TableName))
            {
                this.EntityRelation.TableName = itemEntityAttribute.TableName;
            }
        }
        public bool IsLoadedForeignRelation { get; private set; }
        public Type ItemType { get { return typeof(TEntity);} }

        public Action<IForeignSetBaseField> PropagateForeignSettingMethod { get; set; }
        public bool IsEnablePropagateForeignSetting { get; set; }
        public void SetForeignRelation(string ns, string tableName)
        {
            EntityRelation.Namespace = ns;
            if (!string.IsNullOrEmpty(tableName))
            {
                EntityRelation.TableName = tableName;
            }
            UpdateContent();
        }
        public bool TryGetForeignTable(out EntityTableProxy<TEntity> foreignTableProxy)
        {
            foreignTableProxy = null;
            EntityTable foreignTable;
            if (TryGetForeignTable(out foreignTable))
            {
                foreignTableProxy = new EntityTableProxy<TEntity>(foreignTable,this.SourceProvider);
                return true;
            }
            return false;
        }
        public bool TryGetForeignTable(out EntityTable foreignTable)
        {
            if (!this.EntityRelation.IsEmptyKeys())
            {
                return SourceProvider.TryGetTable(this.EntityRelation, out foreignTable);
            }
            foreignTable = null;
            return false;
        }
        internal EntityRow[] SelectedRows
        {
            get
            {
                EntityTable foreignTable;
                if (TryGetForeignTable(out  foreignTable))
                {
                    foreignTable.RebindRows();
                    return foreignTable.Rows.ToArray();
                }
                return new EntityRow[0];
            }
        }
        
        protected TEntity DoAddNew(string ns, string tableName)
        {
            var srcProvider = this.SourceProvider as IEntityTableSourceTransMgr;
            if (srcProvider == null)
                throw new Exception("ForeignSetField.AddNew() 目前SourceProvider無實作IEntityTableSourceMgr");
            if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(tableName))
                throw new Exception("ForeignSetField.AddNew() ForeignTable Name不能為空");

            var foreignTableProxy = srcProvider.CreateTableProxy<TEntity>(ns, tableName);
            var childEntity = foreignTableProxy.Append();
            DoAdd(childEntity);
            return childEntity;
        }
        protected void DoAdd(TEntity childEntity)
        {
            IEntityProxyInfo childEntityProxy = childEntity as IEntityProxyInfo;
            if (!string.IsNullOrEmpty(childEntityProxy.Proxy.Row.Table.Namespace))
            {
                SetForeignRelation(childEntityProxy.Proxy.Row.Table.Namespace, childEntityProxy.Proxy.Row.Table.TableName);
            }
            //修正所有子ForeignField的孫設定
            if (IsEnablePropagateForeignSetting)
            {
                PropagateForeignSetting(
                    childEntity
                    , (PropagateForeignSettingMethod != null) ? PropagateForeignSettingMethod : (itemForeignField) => itemForeignField.SetForeignRelation(EntityRelation.Namespace)
                    );
            }
            RegistForeignKeyMonitor(childEntityProxy);
        }

        static public void PropagateForeignSetting(TEntity itemEntity,Action<IForeignSetBaseField> settingMethod)
        {
            IEntityProxyInfo childEntityProxy = itemEntity as IEntityProxyInfo;
            Type type = typeof(TEntity); 
            foreach (var propInfo in type.GetProperties())
            {
                if (typeof(IForeignSetBaseField).IsAssignableFrom(propInfo.PropertyType))
                {
                    var itemForeignField = propInfo.GetValue(itemEntity, null) as IForeignSetBaseField;
                    if (itemForeignField == null) continue;
                    itemForeignField.PropagateForeignSettingMethod = settingMethod;
                    settingMethod(itemForeignField);                    
                }
            }
        }
       
        private void RegistForeignKeyMonitor(IEntityProxyInfo childEntityProxy)
        {
            if (foreignKeyMap == null)
                foreignKeyMap = new Dictionary<Guid, object>();
            
            var rowId = childEntityProxy.Proxy.Row.InternalId;
            if (foreignKeyMap.ContainsKey(rowId)) return;
            childEntityProxy.Proxy.Row.PropertyChanged += new PropertyChangedEventHandler(OnNotifyForeignPropertyChanged);

            string key = childEntityProxy.Proxy.Row.PrimaryKey;//EntityTableHelper<TEntity>.RetriveKey(childEntity);
            if (string.IsNullOrEmpty(key))////PID不可為空               
            {
                key = Guid.NewGuid().ToString();
                childEntityProxy.Proxy.Row.SetPrimaryKey(key, true);
            }
          
            foreignKeyMap[rowId] = key;
            this.EntityRelation.Keys.Add(key);
            UpdateContent();
            
        }
        private void OnNotifyForeignPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool doRemoveRow = string.IsNullOrEmpty(e.PropertyName);
            if (foreignKeyMap == null || (!doRemoveRow && !EntityTableHelper<TEntity>.IsKeyProperty(typeof(TEntity), e.PropertyName)))
                return;

            var foreignRow = sender as EntityRow;            
            
            object key;
            if (foreignKeyMap.TryGetValue(foreignRow.InternalId, out key))
            {                
                this.EntityRelation.Keys.Remove(key.ToString());       
            }
            if (doRemoveRow)
            {
                FireResetNotifyCollection();
                return;
            }
            object newKey = foreignRow[e.PropertyName];
            //if ((key == null && newKey == null) || key.Equals(newKey))
            //    return;
            this.EntityRelation.Keys.Add(newKey);
            foreignKeyMap[foreignRow.InternalId] = newKey;
            notifyCollectionChangedAction = NotifyCollectionChangedAction.Replace;
            this.UpdateContent();
        }
        public void DoRemove(TEntity childEntity, bool referenceOnly = true)
        {
            string key = EntityTableHelper<TEntity>.RetriveKey(childEntity).ToString();
            var proxyInfo = childEntity as IEntityProxyInfo;
            if (foreignKeyMap == null)
                foreignKeyMap = new Dictionary<Guid, object>();
            foreignKeyMap.Remove(proxyInfo.Proxy.Row.InternalId);
            if (!referenceOnly)
            {
                var foreignTableProxy = new EntityTableProxy<TEntity>(proxyInfo.Proxy.Row.Table, this.SourceProvider);
                foreignTableProxy.Remove(childEntity);
            }
            //int idx = this.EntityRelation.Keys.IndexOf(key);
            if (this.EntityRelation.Keys.Remove(key))
            {
                //  UpdateContent();
                //NotifyCollectionChangedAction.Remove會當,原因不明
                //NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, childEntity, idx);
                // NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                //  OnCollectionChanged(e);
                //FireResetNotifyCollection();
                
                UpdateContent();
            }

        }
        protected int GetCount()
        {            
            return EntityRelation.IsSelectAll() ? SelectedRows.Length :
                EntityRelation.IsEmptyKeys() ? 0 : EntityRelation.Keys.Count;         
        }
        protected TEntity GetFirst()
        {
            var selRows = SelectedRows;
            return (selRows.Length == 0) ? 
                default(TEntity)
                : selRows[0].GetEntity<TEntity>(base.SourceProvider);
        }
        protected TEntity GetLast()
        {
            var selRows = SelectedRows;
            return (selRows.Length == 0) ? 
                default(TEntity)
                : selRows[selRows.Length - 1].GetEntity<TEntity>(base.SourceProvider);
        }

        protected void UpdateContent()
        {
            Content = ToEntityRelationString(this.EntityRelation);
            
        }
        static public EntityRelation BuildEntityRelation(EntityRow parentRow,EntityColumn column)
        {
            var value=parentRow[column];
            if (value == null) return null;
            return BuildEntityRelation(parentRow, value.ToString());
        }
        static public EntityRelation BuildEntityRelation(EntityRow parentRow,string relationString)
        {
            EntityRelation entityRelation;
#if SILVERLIGHT //避免PolicyBuilder當掉無法編輯
            try
            {
                entityRelation = string.IsNullOrEmpty(relationString) ? new EntityRelation() : transfer.ToObject<EntityRelation>(relationString);
            }
            catch 
            {
                entityRelation = new EntityRelation();
            }           
#else
            entityRelation = string.IsNullOrEmpty(relationString) ? new EntityRelation() : transfer.ToObject<EntityRelation>(relationString);
#endif
            entityRelation.SetParent(parentRow.Table.Namespace, parentRow.Table.TableName);
            return entityRelation;
        }
        static public string ToEntityRelationString(EntityRelation entityRelation)
        {
            return transfer.ToText(entityRelation);
        }

        #region INotifyPropertyChanged Members
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        protected EntityRelation EntityRelation{get;private set;}
        protected IEntityColumnAttributes foreignColumnAttributes;
        static protected BaseTransfer transfer = EntityTableHelper.DefaultTransfer;
        /// <summary>
        /// 每個foreign row的key值異動都應觸動entityRelation同步異動
        /// </summary>
        private Dictionary<Guid, object> foreignKeyMap;
        static private EntityAttribute itemEntityAttribute = EntityAttribute.GetEntityAttribute<TEntity>();
        protected NotifyCollectionChangedAction notifyCollectionChangedAction = NotifyCollectionChangedAction.Reset;
    }
}
