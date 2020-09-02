////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/1/2010 4:25:07 PM 
// Description: TransList.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Common.Support.ErrorHandling;

namespace Common.Support.DataTransaction
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class TransList<T> : TransList<T, List<T>> 
    {
        public TransList() : base() { }
        public TransList(bool propagateTransaction) : base(propagateTransaction) { }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    public class TransList<T, TList> : IDataTransaction, IList<T>, INotifyPropertyChanged
        where TList : IList<T>,new()
    {
        public TransList()
            :this(true)
        {                        
        }
        public TransList(bool propagateTransaction)
        {
            Items = new TList();
            PropagateTransaction = propagateTransaction;
        }
        public TList Items { get; private set; }
        public TList AddedItems { get { return addItems; } }
        public TList RemovedItems { get { return delItems; } }
        public bool PropagateTransaction { get; set; }
        public int OrignalCount { get { return originalItems.Count; } }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {            
            Items.Insert(index, item);
            //-----------------------------------------------------------------------
            AddOrDelItem(ref item,true);
            //-----------------------------------------------------------------------
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            //-----------------------------------------------------------------------
            AddOrDelItem(ref item, false);
            //-----------------------------------------------------------------------
            Items.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return Items[index]; }
            set { Add(value); }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            foreach (var item in Items)
            {
                Remove(item);
            }
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                DataTransactionHelper.SetReadOnly(Items, value);
            }
        }

        public bool Remove(T item)
        {
            int index=IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDataTransaction Members

        public void RollbackOriginal()
        {
            Items.Clear();
            CopyItems(originalItems,Items);
            if (PropagateTransaction)
                DataTransactionHelper.RollbackOriginal(Items);            
            IsUpdating = false;
        }

        public void SetAsOriginal()
        {
            originalItems.Clear();
            CopyItems(Items,originalItems);
            if (PropagateTransaction)
                DataTransactionHelper.SetAsOriginal(Items);            
        }

        public bool IsOriginal
        {
            get
            {
                if (originalItems.Count != Items.Count)
                    return false;
                foreach (var item in originalItems)
                {
                    if (!Items.Contains(item) || !DataTransactionHelper.IsOriginalContent(Items)) 
                        return false;
                }
                return true;                
            }
        }

        public List<DataDifference> ChangeDetails
        {
            get
            {
                List<DataDifference> ChangeSet = new List<DataDifference>();
                var deletedItems = new TList();
                var addedItems = new TList();
                var updatedItems = new TList();
                CopyItems(originalItems, deletedItems);
                foreach (var item in Items)
                {
                    if (originalItems.Contains(item))
                    {
                        deletedItems.Remove(item);
                        updatedItems.Add(item);
                    }
                    else
                    {
                        addedItems.Add(item);
                    }
                }
                ChangeSet.AddRange(BuildChangeDetails(addedItems, DataDifference.ChangeMode.Add));
                ChangeSet.AddRange(BuildChangeDetails(deletedItems, DataDifference.ChangeMode.Delete));
                ChangeSet.AddRange(DataTransactionHelper.GetChangeDetails(updatedItems));
                return ChangeSet;
            }
        }
        private List<DataDifference> BuildChangeDetails(TList list,DataDifference.ChangeMode mode)
        {
            List<DataDifference> changeDetails = new List<DataDifference>();
            foreach (var item in list)
            {
                 IDataTransaction ta = item as IDataTransaction;
                 
                 bool isInsert = (DataDifference.ChangeMode.Add == mode);
                var detail= new DataDifference
                {
                    Transaction = (ta == null) ? this : ta ,
                    CurrentContent = (isInsert)?item:default(T),
                    OriginalContent = (isInsert) ? default(T) : item,
                    Mode = mode
                };
                changeDetails.Add(detail);
            }
            return changeDetails;
        }
        public bool IsUpdating
        {
            get { return isUpdating; } //return DataTransactionHelper.IsUpdating(Items);
            private set{isUpdating = value;}
        }
        public void BeginUpdate(params object[] ids)
        {
            BeginUpdate(Parent, ids);
        }
        public void BeginUpdate(IDataTransaction parent, params object[] ids)
        {
            Parent = parent;
            if (IsReadOnly)
                return;
            ResetAddOrDelItems();
            IsUpdating = true;
            if (PropagateTransaction)
                DataTransactionHelper.BeginUpdate(Items, this, ids);
            State = DataTransactionState.Begin;
        }

        public void CommitUpdate()
        {
            isUpdating = false;
            ResetAddOrDelItems();
            if (PropagateTransaction)
                DataTransactionHelper.CommitUpdate(Items);
            State = DataTransactionState.Commit;
        }

        public void CancelUpdate()
        {
            if (isUpdating)
            {
                MoveItems(delItems, Items);
                RemoveItems(addItems, Items);
                if (PropagateTransaction)
                    DataTransactionHelper.CancelUpdate(Items);                
                isUpdating = false;
                State = DataTransactionState.Cancel;                
            }
        }

        //public bool IsReadOnly;

        private bool validationStatus;
        [NotDataTransaction]
        public bool ValidationStatus
        {
            get { return validationStatus; }
            set
            {
                validationStatus = value;
                OnPropertyChanged("ValidationStatus");
            }
        }
        public bool Validate(out ValidatingExceptionMessage exceptionMessage)
        {
            exceptionMessage = null;
            bool result = DataTransactionHelper.Validate(Items, out exceptionMessage);
            ValidationStatus = result;
            return result;

        }
        [NotDataTransaction]
        [XmlIgnore]
        public DataTransactionState State
        {
            get { return state; }
            private set
            {
                if (state != value)
                {
                    state = value;
                    if (StateChangedEvent != null) StateChangedEvent(this);
                }
            }
        }
        [NotDataTransaction]
        [XmlIgnore]
        public IDataTransaction Parent { get; set; }


        public void PropagateParent(IDataTransaction parent)
        {
            Parent = parent;
            DataTransactionHelper.PropagateParent(Items, this);            
        }

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event Action<IDataTransactionBasic> StateChangedEvent;
        #endregion

        #region INotifyPropertyChanged Members
#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        static private void CopyItems(TList srcList, TList tragetList)
        {
            foreach (var item in srcList) { tragetList.Add(item); };            
        }
        static private void MoveItems(TList srcList, TList tragetList)
        {
            CopyItems(srcList, tragetList);            
            srcList.Clear();
        }

        static private void RemoveItems(TList srcList, TList tragetList)
        {
            foreach (var item in srcList) { tragetList.Remove(item); };
            srcList.Clear();
        }
        private bool HaveAddOrDelItems()
        {
            return delItems.Count != 0 || addItems.Count != 0;
        }
        private void ResetAddOrDelItems()
        {
            delItems.Clear();
            addItems.Clear();
        }
        private void AddOrDelItem(ref T item,bool isAdd)
        {  
            TFunc<T, TList, TList,bool> action = (it, list1, list2) => { list1.Add(it); return list2.Remove(it); };
            action(item, isAdd ? addItems : delItems, isAdd ? delItems : addItems);
            if (isAdd)
            {
                var ta = item as IDataTransaction;
                if (ta != null && PropagateTransaction) ta.BeginUpdate(Parent );
            }
        }
        private DataTransactionState state;
        private bool isReadOnly;
        private bool isUpdating;
        private TList delItems = new TList();
        private TList addItems = new TList();
        private TList originalItems = new TList();
    }
}
