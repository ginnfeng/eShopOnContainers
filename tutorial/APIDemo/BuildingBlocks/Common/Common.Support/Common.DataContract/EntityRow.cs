////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/4/2011 10:54:59 AM 
// Description: EntityRow.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class EntityRow : INotifyPropertyChanged
    {
        private EntityRow()
        {
        }
        internal EntityRow(EntityTable table)
            : this(table, null)
        {
        }
        internal EntityRow(EntityTable table, object[] items)
        {
            Table = table;
            ItemArray = new object[Table.Columns.Count];
            bool useDefaultValue = items == null;
            for (int i = 0; i < ItemArray.Length; i++)
            {
                if (useDefaultValue)
                {
                    var entityField = Table.Columns[i].DefaultValue as IDefaultEntityField;
                    ItemArray[i] = (entityField == null) ? Table.Columns[i].DefaultValue : entityField.Content;//.Clone(this);
                }
                else
                {
                    ItemArray[i] =  items[i];
                }                
            }
        }

        [XmlIgnore]
        public Guid InternalId
        {
            get
            {
                if (Guid.Empty == guid || guid == null)
                    guid = Guid.NewGuid();
                return guid;
            }
        }

        [XmlIgnore]
        public EntityTable Table { get; internal set; }

        [DataMember(Name = "ItemArray")]
        public object[] ItemArray { get; set; }


        [DataMember(Name = "Attributes", IsRequired = false)]
        [XmlAttribute("Attributes")]
        public string Attributes 
        {
            get { return attributes; } 
            set{attributes=value;OnPropertyChanged("Attributes");}
        }
        private string attributes;

        public object this[EntityColumn column]
        {
            get { return this[Table.Columns.IndexOf(column)]; }
            set { this[Table.Columns.IndexOf(column)] = value; }
        }
        public object this[string columnName]
        {
            get { return this[Table.Columns.FindIndex(columnName)]; }
            set { this[Table.Columns.FindIndex(columnName)] = value; }
        }
        public object this[int columnIndex]
        {
            get { return (columnIndex<0)?null:ItemArray[columnIndex]; }
            set
            {
                SetValue(columnIndex, value, false);
            }
        }
        public string PrimaryKey
        {
            get 
            {
                var value = this[Table.PrimaryKey];
                return (value == null) ? null : value.ToString(); 
            }          
        }
        public void SetPrimaryKey(object value, bool disablePropertyChangedEvent)
        {
            SetValue(Table.PrimaryKey,value, disablePropertyChangedEvent);
        }
        public void SetValue(EntityColumn column, object value, bool disablePropertyChangedEvent)
        {
            SetValue(Table.Columns.IndexOf(column), value, disablePropertyChangedEvent);
        }
        public void SetValue(int columnIndex, object value, bool disablePropertyChangedEvent)
        {
            var oldValue=ItemArray[columnIndex];
            if (value==null||( !value.Equals(oldValue)))
            {
                if(Table!=null)
                    Table.OnDataValidating(columnIndex, value);
                ItemArray[columnIndex] = value;
                if (!disablePropertyChangedEvent)
                    OnPropertyChanged(Table.Columns[columnIndex].ColumnName);
            }
        }
        internal void OnAppendColumn()
        {
            var old = ItemArray;
            ItemArray = new object[old.Length +1];
            old.CopyTo(ItemArray, 0);
            ItemArray[old.Length] = Table.Columns[old.Length].DefaultValue;           
        }
        internal void OnRemovColumn(int columnIndex)
        {
            var old = ItemArray;
            ItemArray = new object[old.Length - 1];
            int pos = 0;
            for (int idx = 0; idx < old.Length; idx++)
            {
                if (idx == columnIndex) continue;
                ItemArray.SetValue(old[idx],pos);
                pos++;
            }
        }

        public EntityRow Clone()
        {
            return new EntityRow(Table, ItemArray) { Attributes = Attributes };
        }
        public void CopyItermArrayTo(EntityRow targetRow)
        {
            if (ItemArray.Length != targetRow.ItemArray.Length)
                throw new Exception("EntityRowTransaction.Restore()");
            for (int i = 0; i < targetRow.ItemArray.Length; i++)
            {
                targetRow[i] = this[i];
            }
        }
        #region INotifyPropertyChanged Members
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void OnPropertyChanged(string propertyName)
        {            
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
        private  Dictionary<string,object> Cache 
        { 
            get
            {
                if (cache == null)
                    cache = new Dictionary<string, object>();
                return cache;
            }
        }
        public void AddCache(string key,object value)
        {
            if (cache == null) cache = new Dictionary<string, object>();
            cache[key] = value;
        }
        public bool TryGetCache<TValue>(string key,out TValue value)
        {
            value = default(TValue);
            object item;
            if (cache == null || !cache.TryGetValue(key,out item)) return false;           
                       
            value = (TValue)item;
            return true;
        }
        public void CleanCache()
        {
            if (cache == null) return;
            foreach (var item in cache.Values)
            {
                using (item as IDisposable) { } 
            }
        }
        private Dictionary<string, object> cache;
        private Guid guid;
    }
}
