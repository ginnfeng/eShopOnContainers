////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/21/2009 10:09:27 AM 
// Description: EntityAccessBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Reflection;
using Common.Support.Net.Proxy;

namespace Common.DataCore
{
    abstract public class EntityAccessBase : IEntityAccess
    {
        public EntityAccessBase()
        {
            EntityType = GetType();
        }
        public EntityAccessBase(Type entityType)
        {
            EntityType = entityType;
        }
        public Type EntityType { get; protected set; }
        abstract internal void DoSetProperty(MethodInfo methodInfo, string propertyName, object value);
        abstract internal object DoGetProperty(MethodInfo methodInfo, string propertyName);

        #region IEntityAccess Members

        public object GetPropertyValue(string name)
        {            
            return  DoGetProperty(null,name);            
        }

        public void SetPropertyValue(string name, object value)
        {
            DoSetProperty(null,name,value);            
        }
        
        public object this[string propertyKey]
        {
            get
            {
                return GetPropertyValue(propertyKey);
            }
            set
            {
                SetPropertyValue(propertyKey, value);
            }
        }

        #endregion

    }

    abstract public class EntityAccessBase<TEntity> : EntityAccessBase
    {
        public EntityAccessBase()
            : base(typeof(TEntity))
        {
        }
        public TEntity GenEntityProxy()
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += DoGetProperty;
            realProxy.SetPropertyEvent += DoSetProperty;
            return realProxy.Entity;
        }        
    }
}
