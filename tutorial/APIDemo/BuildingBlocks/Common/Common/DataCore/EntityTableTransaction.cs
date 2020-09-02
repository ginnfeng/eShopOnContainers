////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/26/2011 5:02:40 PM 
// Description: EntityTableTransaction.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.Support.DataTransaction;
using Common.DataContract;
using System.Xml.Serialization;
using Common.Support.Net.Util;

namespace Common.DataCore
{
    public class EntityTableTransaction : IDataTransactionBasic
    {
        public EntityTableTransaction()
        {
        }

        public IEnumerable<EntityTable> GetAllTable()
        {
            List<EntityRow> changeRows = new List<EntityRow>();
            updatingTables.Values.ForEach(tbl => changeRows.AddRange(tbl.Rows));
            return changeRows.GroupToTables();
        }

        public void ClearAllTable()
        {
            //updatingRows.Clear();
            updatingTables.Clear();
        }
        public List<EntityTable> GenEntityTableChangeChangeDetails()
        {
            List<EntityRow> changeRows = new List<EntityRow>();
            this.ChangeDetails.ForEach(detail => changeRows.Add((EntityRow)detail.CurrentContent));
            var tables = changeRows.GroupToTables();
            tables.ForEach(tbl => tbl.GetAttributesEntity().IsPartial = true);

            foreach (var tbl in this.updatingTables.Values)
            {
                if (tbl.GetAttributesEntity().Status != OPStatus.Steady)
                {
                    if (tables == null) tables = new List<EntityTable>();
                    tables.Add(tbl);
                }
            }
            return tables;
        }
        public bool FindRegistTableDo(string ns, string tableName,Action<EntityTable> action)
        {
            EntityTable table;
            if (!TryGetRegistTable(ns, tableName, out table))
                return false;
            action(table);
            return true;
        }

        public bool TryGetRegistTable(string ns, string tableName, out EntityTable table)
        {
            return updatingTables.TryGetValue(GenKey(ns, tableName), out table);
        }
        public bool TryGetRegistTable(EntityRelation relation, out EntityTable table)
        {
            return updatingTables.TryGetValue(GenKey(relation), out table);
        }
        private bool TryGetRegistTable(EntityRow row, out EntityTable table)
        {
            return updatingTables.TryGetValue(GenKey(row.Table), out table);
        }
        public void Unregist(EntityTable entityTable, bool isSchema)
        {
            string key = GenKey(entityTable, isSchema);
            entityTable.CollectionChanged -= OnTableCollectionChanged;
            updatingTables.Remove(key);          
        }
        public void Reregist(EntityTable entityTable, OPStatus opStatus)
        {
            Unregist(entityTable,false);
            Regist(entityTable, opStatus);
        }
        /// <summary>
        /// 登錄到記憶體中
        /// </summary>
        /// <param name="entityTable"></param>
        public void Regist(EntityTable entityTable, OPStatus opStatus)
        {
            //if (entityTable == null) return;
            if (entityTable == null || (opStatus==OPStatus.Steady && entityTable.HasCollectionChangedEventSubscriber())) return;
            if (opStatus.IsPassSchema())
            {
                entityTable.GetAttributesEntity().Status = opStatus;
                updatingTables[GenKey(entityTable,true)] = entityTable;
                return;
            }
            
            //if(HasCollectionChangedEventSubscriber())
            var attris = entityTable.GetAttributesEntity();
            attris.Status = opStatus;
            entityTable.CollectionChanged -= OnTableCollectionChanged;
            entityTable.CollectionChanged += OnTableCollectionChanged;
            //Comment out: 每次因EntityTable的rows可能數量不同,有的是部分
            //if (!attris.IsPartial&&updatingTables.ContainsKey(GenKey(entityTable)))
            //    return;
            Action<EntityRow> action = delegate(EntityRow row)
            {
                var attribute = row.GetAttributesEntity();
                //attribute.Status = opStatus;
                row.PropertyChanged -= OnRowChanged;
                row.PropertyChanged += OnRowChanged;
            };
            EntityTable registedTable = TakeRegistedTable(entityTable);
            entityTable.Rows.ForEach(row => { action(row); DoRegist(registedTable, row, opStatus); });

        }
        public void Regist(EntityTable srcTable, EntityRow row, OPStatus opStatus)
        {            
            EntityTable registedTable = TakeRegistedTable(srcTable);
            DoRegist(registedTable, row, opStatus);
        }
        private void DoRegist(EntityTable registedTable, EntityRow row, OPStatus opStatus)
        {
            IEntityRowAttributes rowAttributes = row.GetAttributesEntity();
            bool isExist = registedTable.Rows.Contains(row);
            bool inTransaction = (State == DataTransactionState.Begin);
            switch (opStatus)
            {
                case OPStatus.Delete:
                    if (isExist)
                        registedTable.Rows.Remove(row);
                    if (rowAttributes.Status == OPStatus.Add)
                    {                        
                        UnregistRowTransaction(row);
                    }
                    else
                    {
                        RegistRowTransaction(row, true);
                        rowAttributes.Status = opStatus;
                    }
                    break;
                case OPStatus.Add:
                    if (!isExist)
                        registedTable.Rows.Add(row);
                    RegistRowTransaction(row, false);
                    rowAttributes.Status = opStatus;
                    break;
                case OPStatus.Update:
                    if (rowAttributes.Status != OPStatus.Add && rowAttributes.Status != OPStatus.Delete)
                    {
                        rowAttributes.Status = opStatus;
                        //100/11/30 by Feng,避免已經BeginUpdate後才又有Table被查詢regist
                        RegistRowTransaction(row, true);
                    }
                    break;
                case OPStatus.Steady:
                    if (!isExist)
                    {
                        rowAttributes.Status = opStatus;
                        registedTable.Rows.Add(row);
                        RegistRowTransaction(row, true);
                    }
                    break;
                default:
                    throw new Exception("EntityTableTransaction.Regist()");
            }
        }
        private EntityTable TakeRegistedTable(EntityTable srcTable)
        {
            EntityTable table;
            string key = GenKey(srcTable);
            if (!updatingTables.TryGetValue(key, out table))
            {
                //by feng 100/08/26 ,此處應該不需作Clone,若改回則須同時改回Common.DataCore.EntityExtension.GroupToTables()
                //之tableAttributes.KeyCount = maxKeyCount;
                //table = srcTable.Clone(false); 
                table = srcTable;
                updatingTables[key] = table;
            }
            else
            {
                if (!srcTable.GetAttributesEntity().IsPartial)
                {//置換,但不能做    Uuregist把CollectionChanged event移除               
                    updatingTables[key] = srcTable;                    
                }
                else
                {
                    //Add by Feng 100/08/25
                    table.GetAttributesEntity().KeyCount = srcTable.GetAttributesEntity().KeyCount;
                }
            }
            return table;
        }
        private void RegistRowTransaction(EntityRow row, bool toBeginTransaction)
        {
            if (State != DataTransactionState.Begin) return;
            EntityRowTransaction transRow;
            if (!rowTransactionMap.TryGetValue(row.InternalId, out transRow))
            {
                transRow = new EntityRowTransaction(row);
                rowTransactionMap[row.InternalId] = transRow;
            }
            if (toBeginTransaction)
                transRow.BeginUpdate();
        }
        private void UnregistRowTransaction(EntityRow row)
        {
            if (State != DataTransactionState.Begin) return;
            rowTransactionMap.Remove(row.InternalId);
        }
        //public event Action<IDataTransaction> StateChangedEvent;

        private void OnTableCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            EntityRow row;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    row = e.NewItems[0] as EntityRow;
                    Regist(row.Table, row, OPStatus.Add);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    row = e.OldItems[0] as EntityRow;
                    Regist(row.Table, row, OPStatus.Delete);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void OnRowChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (State != DataTransactionState.Begin)
                return;
            var row = sender as EntityRow;
            Regist(row.Table, row, OPStatus.Update);
        }

        static public string GenKey(IEntityTableInfo tableInfo,bool isSchema=false)
        {
            var key = GenKey(tableInfo.Namespace, tableInfo.TableName);
            return (isSchema)?key+"#Schema":key;
        }
        static public string GenKey(string ns, string tableName)
        {
            
            return ns + "#" + tableName;
        }
        public bool IsUpdating { get { return State == DataTransactionState.Begin; } }

        #region IDataTransaction Members

        [NotDataTransaction]
        [XmlIgnore]
        public DataTransactionState State
        {
            get { return state; }
            private set
            {
                if (state == value) return;
                state = value;
                if (StateChangedEvent != null) StateChangedEvent(this);
            }
        }

        public void BeginUpdate(params object[] ids)
        {
            if (IsUpdating) return;
            Reset();

            BuildRowTransactions(ref this.rowTransactionMap);
            this.rowTransactionMap.ForEach(pair => pair.Value.BeginUpdate(ids));
            State = DataTransactionState.Begin;
        }


        public void CommitUpdate()
        {
            if (IsUpdating)
            {
                State = DataTransactionState.Commit;
                Reset(false);
            }
        }

        public void CancelUpdate()
        {
            if (IsUpdating)
            {
                State = DataTransactionState.Cancel;
                foreach (var taRow in rowTransactionMap.Values)
                {
                    IEntityRowAttributes rowAttributes = taRow.Row.GetAttributesEntity();
                    EntityTable registedTable;
                    switch (rowAttributes.Status)
                    {
                        case OPStatus.Add:
                            if (TryGetRegistTable(taRow.Row, out registedTable))
                            {
                                registedTable.Rows.Remove(taRow.Row);
                                taRow.Row.OnPropertyChanged("");//通知ForeignSetField刪除
                            }
                            break;
                        case OPStatus.Update:
                            taRow.CancelUpdate();
                            break;
                        case OPStatus.Delete:
                            if (TryGetRegistTable(taRow.Row, out registedTable))
                            {
                                taRow.CancelUpdate();
                                registedTable.Rows.Add(taRow.Row);
                            }
                            break;
                        default:
                            break;
                    }
                }
                Reset();
            }
        }


        public List<DataDifference> ChangeDetails
        {
            get
            {
                if (rowTransactionMap == null || rowTransactionMap.Count == 0)
                    return null;
                var details = new List<DataDifference>();
                foreach (var taRow in rowTransactionMap.Values)
                {
                    var rowDetails=taRow.ChangeDetails;
                    if (rowDetails != null) details.AddRange(rowDetails);                    
                }
                return details;
            }
        }



        public event Action<IDataTransactionBasic> StateChangedEvent;

        #endregion
        private void BuildRowTransactions(ref Dictionary<Guid, EntityRowTransaction> rows)
        {
            if (rows == null) rows = new Dictionary<Guid, EntityRowTransaction>();
            foreach (var table in updatingTables.Values)
            {
                foreach (var row in table.Rows)
                {
                    rows[row.InternalId] = new EntityRowTransaction(row);
                }
            }
        }
        private void Reset(bool cleanRowTransactionMap = true)
        {
            if (cleanRowTransactionMap && rowTransactionMap != null)
                rowTransactionMap.Clear();
        }

        private DataTransactionState state;

        private Dictionary<string, EntityTable> updatingTables = new Dictionary<string, EntityTable>();
        private Dictionary<Guid, EntityRowTransaction> rowTransactionMap;
    }
}
