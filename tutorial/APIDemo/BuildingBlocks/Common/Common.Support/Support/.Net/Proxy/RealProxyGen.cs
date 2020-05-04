////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/7/2011 11:33:36 AM 
// Description: RealProxyGen.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.Proxy
{
    static public class RealProxyGen
    {
        static public TEntity GenEntityProxy<TEntity>(GetPropertyDelegate getPropertyValue, SetPropertyDelegate setPropertyValue)
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += getPropertyValue;
            realProxy.SetPropertyEvent += setPropertyValue;
            return realProxy.Entity;
        }
        static public TEntity GenEntityProxy2<TEntity>(Func< string, object> getPropertyValue, Action<string, object> setPropertyValue)
        {
            
            GetPropertyDelegate getDelegate = new GetPropertyDelegate((methodInfo, propertyName) => CommonExtension.ToObject(getPropertyValue(propertyName), methodInfo.ReturnType));
            SetPropertyDelegate setDelegate = new SetPropertyDelegate((methodInfo, propertyName, value) => setPropertyValue(propertyName, value));
            return GenEntityProxy<TEntity>( getDelegate, setDelegate);
        }
    }
}
