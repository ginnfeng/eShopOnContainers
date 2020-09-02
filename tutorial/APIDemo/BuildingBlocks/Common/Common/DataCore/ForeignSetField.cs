////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/13/2009 5:29:13 PM 
// Description: ForeignField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

using Common.DataContract;
using System.Collections.Specialized;

namespace Common.DataCore
{


#if !SILVERLIGHT
    [Serializable]
#endif

    public class ForeignSetField<TEntity> : ForeignFieldBase<TEntity>, IForeignSetField, IEnumerable<TEntity>, INotifyCollectionChanged
    {
        public ForeignSetField()
        {
            CollectionChanged += new NotifyCollectionChangedEventHandler
                (
                delegate(object sender, NotifyCollectionChangedEventArgs e) { OnPropertyChanged("Count"); }
                );
        }

        override public void Initializing()
        {
            base.Initializing();
            FireResetNotifyCollection();            
        }
        public void SetSelectAll(bool isAll)
        {
            if (isAll)
            {
                this.EntityRelation.SetAsSelectAll();
                base.UpdateContent();
            }
            else
            {
                var keyList = new List<string>();
                var selRows = SelectedRows;
                foreach (var row in selRows)
	            {
                    keyList.Add(row.PrimaryKey);
	            }
                this.EntityRelation.Keys.Clear();
                this.EntityRelation.Keys.AddRange(keyList);
            }
        }
        public void Add(TEntity childEntity)
        {
            base.DoAdd(childEntity);        
        }
        /// <summary>
        /// 以最後一筆的NS與Table為依據
        /// </summary>
        /// <returns></returns>
        public TEntity AddNew()
        {
            return AddNew(this.EntityRelation.Namespace, this.EntityRelation.TableName);
        }

        public TEntity AddNew(string ns, string tableName)
        {
            return base.DoAddNew(ns,tableName);
        }

        public void Remove(TEntity childEntity, bool referenceOnly = true)
        {
            base.DoRemove(childEntity, referenceOnly);
        }

        public TEntity First{get{return base.GetFirst();}}
        
      
        public TEntity Last { get { return base.GetLast(); } }
        

        public int Count
        {
            get 
            {
                return base.GetCount();
            }
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
                if (func(entity)) return entity;
            }
            return default(TEntity);
        }
        public List<TEntity> FindAll(Func<TEntity, bool> func)
        {
            List<TEntity> results = new List<TEntity>();
            foreach (var entity in this)
            {
                if (func(entity))
                {
                    results.Add(entity);
                }
            }           
            return results;
        }
        public void ForEach(Action<TEntity> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            var selRows = SelectedRows;
            foreach (EntityRow entity in selRows)
            {
                yield return entity.GetEntity<TEntity>(base.SourceProvider);
            }
        }
        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
       
        #region INotifyCollectionChanged Members
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion


        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {

            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }


        override protected void FireResetNotifyCollection()
        {
            if(notifyCollectionChangedAction == NotifyCollectionChangedAction.Reset)
             OnCollectionChanged(new NotifyCollectionChangedEventArgs(notifyCollectionChangedAction));
            notifyCollectionChangedAction = NotifyCollectionChangedAction.Reset;
        }
        
        private bool HasDefaultForeignInfo { get; set; }
        public string DefaultForeignTable { get; set; }      
    }
}
