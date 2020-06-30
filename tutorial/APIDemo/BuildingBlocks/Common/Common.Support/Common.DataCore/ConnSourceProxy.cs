////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/30/2020 3:48:52 PM 
// Description: ConnSourceProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.Support.Common.DataCore
{
    public class ConnSourceProxy: IConnSource
    {
        public ConnSourceProxy(string connString)
        {
            ConnString = connString;
        }
        public TEntity GenEntityProxy<TEntity>()
            where TEntity : class
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += DoGetProperty;            
            return realProxy.Entity;
        }

        private object DoGetProperty(MethodInfo methodInfo, string propertyName)
        {
            throw new NotImplementedException();
        }

        public string ConnString { get; }
    }
    public class ConnSourceProxy<T> : IConnSource<T>
        where T:class
    {
        public ConnSourceProxy(string connString)
        {
            proxy = new ConnSourceProxy(connString);
        }
        public T Entity => proxy.GenEntityProxy<T>();

        public string ConnString => proxy.ConnString;


        private ConnSourceProxy proxy;
    }
}
