////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/13/2011 3:38:17 PM 
// Description: DictionaryProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
using Common.Support.Net.Proxy;
using System.ComponentModel;
using Common.Support;

namespace Common.DataCore
{
    public class DictionaryProxy<TValue> : INotifyPropertyChanged
    {
        public DictionaryProxy()
            : this(new Dictionary<string, TValue>())
        {
        }
        public DictionaryProxy(Dictionary<string, TValue> valueMap)
        {
            ValueMap = valueMap;
        }
        //public event Action<string> UpdatedEvent;

        public TEntity GenEntityProxy<TEntity>()
        {           
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += DoGetProperty;
            realProxy.SetPropertyEvent += DoSetProperty;
            return realProxy.Entity;
        }
        public object this[string propertyKey]
        {
            get {return ValueMap[propertyKey];}
            set { ValueMap[propertyKey] = (TValue)value; }
        }
        internal void DoSetProperty(System.Reflection.MethodInfo methodInfo, string propertyName, object value)
        {
            ValueMap[propertyName] = CommonExtension.ConvertTo<TValue>(value); 
            OnPropertyChanged(propertyName);
        }

        internal object DoGetProperty(System.Reflection.MethodInfo methodInfo, string propertyName)
        {
            TValue value;
            if (ValueMap.TryGetValue(propertyName, out value))
            {
                return (methodInfo.ReturnType.Equals(typeof(string)))
                    ?value:CommonExtension.ToObject(value, methodInfo.ReturnType);
            }
            return value;
        }

        public Dictionary<string, TValue> ValueMap { get; set; }

        #region INotifyPropertyChanged Members
        //[field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
