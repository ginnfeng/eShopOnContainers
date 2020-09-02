////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/4/2011 10:54:45 AM 
// Description: EntityTable.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class EntityTable : IEntityTableInfo, INotifyCollectionChanged, IComparable<EntityTable>
    {
        
        public EntityTable()
        {            
            Columns = new EntityColumnCollection();
            Rows = new List<EntityRow>();
            this.UpDateVersion();
        }
        public void ShrinkColumnAt(int idx)
        {
            RebindRows();
            Columns.RemoveAt(idx);
            Rows.ForEach(it => it.OnRemovColumn(idx));
        }
        public void ExpandColumn(EntityColumn newColumn)
        {
            RebindRows();
            Columns.Add(newColumn);
            Rows.ForEach(it => it.OnAppendColumn());
        }
        [DataMember(Name = "Version")]
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [DataMember(Name = "Columns")]
        public EntityColumnCollection Columns { get; set; }

        /// <summary>
        /// 直接操做Rows不會產生INotifyCollectionChanged
        /// </summary>
        [DataMember(Name = "Rows")]
        public List<EntityRow> Rows
        {
            get 
            {                
                if (!isBindedRowToTable) this.RebindRows();
                return rows; 
            }
            set
            {
                if (rows == value) return;
                rows = value;
                this.RebindRows();
            }
        }

        [DataMember(Name = "NS")]
        [XmlAttribute("NS")]
        public string Namespace { get; set; }

        [DataMember(Name = "Name")]
        [XmlAttribute("Name")]
        public string TableName { get; set; }

        [DataMember(Name = "Attributes", EmitDefaultValue = false)]
        [XmlAttribute("Attributes")]
        public string Attributes { get; set; }

        [DataMember(Name = "OldSchema", EmitDefaultValue=false)]       
        //EntityTable內容為schema table,此OldSchema為異動前的舊schema
        public EntityTable OldSchema { get; set; }

        //public Type BindingEntityType { get; set; }
        [XmlIgnore]
        public string FullName 
        {
            get { return Namespace+"."+TableName;}
        }
        [XmlIgnore]
        public EntityColumn PrimaryKey
        {
            get
            {
                if (primaryKey == null)
                {
                    primaryKey = Columns.Find(col => col.Unique);
                }
                return primaryKey;
            }
        }
        
        public void ForeachRowByKey(Action<object, EntityRow> action)
        {
            this.RebindRows();
            if (PrimaryKey == null) return;
            int idx = Columns.IndexOf(PrimaryKey);
            foreach (var row in Rows)
            {
                action(row.ItemArray[idx], row);
            }
        }
        public EntityRow Find(string keyValue)
        {
            if (PrimaryKey == null) return null;
            int idx = Columns.IndexOf(PrimaryKey);
            var entityRow = Rows.Find(row => keyValue.Equals(row[idx].ToString()));
            entityRow.Table = this;
            return entityRow;
        }
        public bool Remove(string keyValue)
        {
            if (PrimaryKey == null) return false;
            int idx = Columns.IndexOf(PrimaryKey);
            var it = Rows.Find(row => keyValue.Equals(row[idx]));
            if (it == null) return false;
            Rows.Remove(it);
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, it, idx);
            OnCollectionChanged(e);
            return true;
        }
        public EntityRow Append(object[] values = null)
        {
            EntityRow row = new EntityRow(this, values);
            Append(row);
            return row;
        }

        // Add By Sean
        public void Append(EntityRow row)
        {
            row.Table = this;
            Rows.Add(row);
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, row, Rows.Count - 1);
            OnCollectionChanged(e);
        }

        // Add By Sean
        public bool Remove(EntityRow row)
        {
            bool t= Rows.Remove(row);
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, row, Rows.Count - 1);
            OnCollectionChanged(e);
            return t;
        }

        // Add By Sean
        public void RemoveAll()
        {
            Rows.Clear();
        }

        public void RebindRows()
        {            
            isBindedRowToTable = true;
            rows.ForEach(row => row.Table = this);            
        }
        public EntityTable CloneDigest()
        {
            return new EntityTable() { Namespace = this.Namespace, TableName = this.TableName, Version = this.Version, Attributes = Attributes };
        }
        public EntityTable Clone(bool includingRows = true)
        {
            var table = CloneDigest();
            CloneTo(ref table, includingRows);            
            return table;
        }
        public void CloneTo(ref EntityTable table, bool includingRows = true)
        {
            table.Columns.Clear();
            table.Rows.Clear();
            table.Columns.AddRange(Columns);
            if (includingRows)
                table.Rows.AddRange(Rows);        
        }
        public bool IsEmpty() { return Rows.Count == 0; }
        // Add by Sean
        public void UpDateVersion(string version = null)
        {
            /// 若沒指定則用現在的時間當 version 版本
            if (string.IsNullOrEmpty(version))
            {
                version = DateTime.Now.ToFileTime().ToString();
            }
            this.Version = version;
        }
        public bool HasCollectionChangedEventSubscriber()
        {
            return this.CollectionChanged != null;
        }

        #region INotifyCollectionChanged Members
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
        #endregion

        private EntityColumn primaryKey;
        private List<EntityRow> rows;
        private bool isBindedRowToTable;

        #region IComparable<EntityTable> Members



        public int CompareTo(EntityTable other)
        {
            return (other == null)? +1:TableName.CompareTo(other.TableName);            
        }

        #endregion

        public override bool Equals(object obj)
        {            
            EntityTable it = obj as EntityTable;
             if (it == null) return false;
             //check reference equality  
             if (object.ReferenceEquals(this, it))
                 return true;
            return it.TableName==TableName&&it.Namespace==Namespace;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(TableName)?"".GetHashCode():TableName.GetHashCode();
        }

        internal void OnDataValidating(int columnIdx,object fieldValue)
        {            
            var method=Columns[columnIdx].ValidationMethod;
            if (method != null) method(fieldValue);            
        }
    }
}
