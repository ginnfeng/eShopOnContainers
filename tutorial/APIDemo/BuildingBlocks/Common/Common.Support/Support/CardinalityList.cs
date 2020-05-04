////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/13/2007 
// Description: CardinalityList.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Collections;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters.Soap;

namespace Support
{
    public interface ICardinalityList : IEnumerator
    {
        string Id { get; set; }
        string Name { get; set; }

        int MinCardinality { get; }
        int MaxCardinality { get; }
        int Cardinality { get; set; }        
    }

    public interface ICardinalityList<T,TList> :ICardinalityList, IEnumerator<T>
        where TList:IList<T>,new()
    {
        TList Items { get; }
        T SampleData {get;set;}
    }
    [Serializable]
    public class CardinalityList<T, TFormatter> : CardinalityList<T, TFormatter, List<T>>           
        where TFormatter:IFormatter,new()    
    {
        public CardinalityList()
        { }
        public CardinalityList(int minCardinality, int maxCardinality)
            : base(minCardinality, minCardinality, maxCardinality)
        {
        }
        public CardinalityList(int minCardinality, int maxCardinality,int initialCardinality)
            : base(minCardinality, maxCardinality, initialCardinality)
        {           
        }

        public CardinalityList(int minCardinality, int maxCardinality, int initialCardinality, T sampleData)
            : base(minCardinality, maxCardinality, initialCardinality, sampleData)
          {
          }
    }
      /// <typeparam name="T"></typeparam>
    /// <typeparam name="Formatter">BinaryFormatter , SoapFormatter ..</typeparam>    
    [Serializable]
    public class CardinalityList<T, TFormatter, TList> : ICardinalityList<T, TList>
        where TList : IList<T>,new()
        where TFormatter:IFormatter,new()    
    {
        //public delegate void EventHandler(object sender, T[] changedItems);
        public CardinalityList()
            : this(0, int.MaxValue)
        {
        }
        public CardinalityList(int minCardinality, int maxCardinality)
            : this(minCardinality, minCardinality, maxCardinality)
        {
        }
        public CardinalityList(int minCardinality, int maxCardinality, int initialCardinality)
            : this(minCardinality, maxCardinality, initialCardinality, defaultSample)
        {
        }

        public CardinalityList(int minCardinality, int maxCardinality, int initialCardinality, T sampleData)
        {
            this.sampleData = sampleData;
            this.minCardinality = minCardinality;
            this.maxCardinality = maxCardinality;
            //避免一開始無法建立
            this.Cardinality = initialCardinality;
            Reset();
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int MinCardinality
        {
            get { return minCardinality; }
            set
            {
                minCardinality = value;
                if (cardinality < value)
                {
                    Cardinality = value;
                }
            }
        }

        public int MaxCardinality
        {
            get { return maxCardinality; }
            set
            {
                maxCardinality = value;
                if (cardinality > value)
                {
                    Cardinality = value;
                }
            }
        }
        protected void ForceSyncCardinality()
        {
            cardinality = Items.Count;
        }
        virtual public int Cardinality
        {
            get { return cardinality; }
            set
            {
                if (value < MinCardinality || value > MaxCardinality)
                    throw new IndexOutOfRangeException("CardinalityList.Cardinality");                

                cardinality = value;
                if (cardinality < items.Count)
                {
                    var removeItems = ListHelper<T>.GetRange<List<T>>(items, cardinality, (items.Count - cardinality));
                    ListHelper<T>.RemoveRange(items, cardinality, (items.Count - cardinality));                    
                    FireChangeEvent(RemovedItemsEvent, ListHelper<T>.ToArray(removeItems));
                }
                if ((SampleData != null) && (cardinality > items.Count))
                {
                    int size = cardinality - items.Count;
                    T[] appendedItems = ObjectHelper.CloneMany(SampleData, size);
                    
                    FireChangeEvent(BeforeAppedItemsEvent, appendedItems);
                    foreach (T obj in appendedItems)
                    {
                        items.Add(obj);
                    }
                    FireChangeEvent(AppendedItemsEvent, appendedItems);
                   
                }
            }
        }

        private void FireChangeEvent(TAction<object, T[]> changeEvent, T[] changeItems)
        {
            if (changeEvent != null)
                changeEvent(this, changeItems);
        }
        

        public TList Items
        {
            get
            {
                return items;//.AsReadOnly();
            }
        }

        public T SampleData
        {
            get { return sampleData; }
            set
            {
                sampleData = value;

                if (items.Count == 0 && minCardinality>0)
                {
                    if (sampleData != null)
                    {
                        RebuildItems();
                    }
                }
            }
        }

        public void RebuildItems()
        {
            T[] updatedItems = ObjectHelper.CloneMany(sampleData, Cardinality);
            items = CreateList(updatedItems);        
            FireChangeEvent(AppendedItemsEvent, updatedItems);
        }

        [field: NonSerialized]
        public event TAction<object, T[]> BeforeAppedItemsEvent;
        [field: NonSerialized]
        public event TAction<object, T[]> AppendedItemsEvent;
        [field: NonSerialized]
        public event TAction<object, T[]> RemovedItemsEvent;
        
        public T AddNew()
        {
            Cardinality++;
            return Last;
        }

        /// <param name="idx">zero base</param>        
        public virtual void RemoveAt(int index)
        {            
            if ((Cardinality - 1) < MinCardinality)
            {
                throw new IndexOutOfRangeException("RemoveAt(int index): (Cardinality - 1) < MinCardinality");
            }            
            if (index < 0 || index >= Cardinality)
            {
                throw new IndexOutOfRangeException("RemoveAt(int index): index < 0 || index >= Cardinality");
            }
            var removeItem=items[index];
            items.RemoveAt(index);
            cardinality--;
            FireChangeEvent(RemovedItemsEvent, ListHelper<T>.ToArray(new T[] { removeItem }));
        }

        public T First
        {
            get
            {
                if (Cardinality < 1)
                    throw new IndexOutOfRangeException("First");
                return Items[0];
            }
        }
        public T Last
        {
            get
            {
                if (Cardinality < 1)
                    throw new IndexOutOfRangeException("Last");
                return Items[Items.Count - 1];
            }
        }

        #region IEnumerator<T> Members

        public T Current
        {
            get
            {
                return Items[position];
            }
        }

        public bool MoveNext()
        {
            if ((position + 1) >= Cardinality)
                return false;
            position++;
            return true;
        }

        public void Reset()
        {
            position = -1;
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;

        #endregion
        static TList CreateList(T[] array)
        {
            TList list = new TList();
            foreach (var item in array)
            {
                list.Add(item);
            }
            return list;
        }
        
        static readonly T defaultSample = MakeDefaultSample();
        static T MakeDefaultSample()
        {
            Type type = typeof(T);
            if (type.IsPrimitive) return default(T);
            if (type == typeof(string))
            {
                object defaulrStr = "";
                return (T)defaulrStr;
            }

            return (type.GetConstructor(Type.EmptyTypes) != null) ? (T)Activator.CreateInstance(typeof(T)) : default(T);
        }
        private int position;
        #region DataMembers
        private int maxCardinality;
        private int minCardinality;
        private int cardinality;
        private TList items = new TList();
        private T sampleData;
        private string id;
        private string name;
        #endregion
    }

    static public class ListHelper<T>        
    {
        static public T[] ToArray(IList<T> srcList)
        {
            T[] array = new T[srcList.Count];
            for (int i = 0; i < srcList.Count; i++)
            {
                array.SetValue(srcList[i], i);
            }
            return array;
        }
        static public void RemoveRange(IList<T> srcList, int index, int count)
        {
            for (int idx = 0; idx < count; idx++)
            {
                srcList.RemoveAt(index); ;
            }
        }
        static public TList GetRange<TList>(IList<T> srcList, int index, int count)
            where TList:IList<T>,new()
        {
            TList list = new TList();
            for (int idx = index; idx < (index + count); idx++)
            {
                list.Add(srcList[index]);
            }
            return list;
        }
    }
}
