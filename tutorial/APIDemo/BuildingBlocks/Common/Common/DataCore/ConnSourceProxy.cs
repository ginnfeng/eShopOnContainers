////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/30/2020 3:48:52 PM 
// Description: ConnSourceProxy.cs  
// Revisions  :          
// Sample
//public void T6()
//{
//    string s = "Provider=MSDataShape;Data Provider=SQLOLEDB;Data Source=(local);Initial Catalog=pubs;Integrated Security=SSPI;MaxConnection=10";
//    //var rlt=s.ReadPairs('=', ';');
//    var proxy = new ConnSourceProxy<IDbCfg>(s);
//    var cfg = proxy.Entity;
//    print($"Provider={cfg.Provider} DataSource={cfg.DataSource} MaxConnection={cfg.MaxConnection}");
//    cfg = new DbCfg();
//    proxy.ForEachSet(cfg);
//    print($"Provider={cfg.Provider} DataSource={cfg.DataSource} MaxConnection={cfg.MaxConnection}");
//}	
// **************************************************************************** 
using Common.DataContract;
using Common.Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Support.Net.Util;

using Common.Support.Helper;

namespace Common.Support.Common.DataCore
{
    public class ConnSourceProxy: IConnSource
    {
        public ConnSourceProxy(string connString)
        {
            ConnString = connString;
        }
        public TEntity TakeCache<TEntity>()
            where TEntity : class, new()
        {
            kvCacheMap ??= new Dictionary<Type, object>();
            var type = typeof(TEntity);
            object entity;
            if(!kvCacheMap.TryGetValue(type,out entity))
            {
                entity = new TEntity();
                ForEachSet<TEntity>((TEntity)entity);
                kvCacheMap[type] = entity;
            }
            return (TEntity)entity;
        }
        public TEntity GenEntityProxy<TEntity>()
            where TEntity : class
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += DoGetProperty;            
            return realProxy.Entity;
        }
        public void ForEachSet<TEntity>(TEntity it)
            where TEntity : class
        {
            var type=typeof(TEntity);            
            foreach (var propInfo in type.GetProperties(BindingFlags.SetProperty| BindingFlags.Public|BindingFlags.Instance))
            {
                object settingValue;
                if(TryGetSettingValue(propInfo, propInfo.PropertyType, out settingValue))
                    propInfo.SetMethod.Invoke(it, new object[] { settingValue });
            }
        }
        private object DoGetProperty(MethodInfo methodInfo, string propertyName)
        {
            var propInfo=methodInfo.DeclaringType.GetProperty(propertyName);
            object settingValue;
            TryGetSettingValue(propInfo, propInfo.PropertyType, out settingValue);
            return settingValue;
        }
        private bool TryGetSettingValue(PropertyInfo propInfo, Type returnType,out object settingValue,bool isInBaseType=false)
        {            
            kvMap ??= Init(ConnString);
            var idxAttribute = propInfo.GetCustomAttribute<IndexingAttribute>();
            var key = (idxAttribute == null) ? propInfo.Name : idxAttribute.Id;
            string value;
            if (kvMap.TryGetValue(key, out value))
            {
                settingValue=(returnType.Equals(typeof(string)))? value : CommonExtension.ToObject(value, returnType);
                return true;
            }else if (!isInBaseType)
            {
                Func<Type, PropertyInfo> cond = type => (!propInfo.DeclaringType.IsInterface)? null: type.GetProperty(propInfo.Name); 
                propInfo = propInfo.DeclaringType.FindIncludBaseType<PropertyInfo>(cond);
                if(propInfo!=null)
                    return TryGetSettingValue(propInfo, returnType, out settingValue,true);
            }
            
            settingValue = null;
            return false;
        }
        static Dictionary<string, string>  Init(string connString)
        {
            var rlt = connString.ReadPairs('=', ';');
            return new Dictionary<string, string>(rlt);
        }

        

        public string ConnString { get; }
        private Dictionary<string, string> kvMap;
        private Dictionary<Type, object> kvCacheMap;
    }
    public class ConnSourceProxy<T> : ConnSourceProxy,IConnSource<T>
        where T:class
    {
        public ConnSourceProxy(string connString)
            :base(connString)
        {            
        }

        public T Entity => entity ??= GenEntityProxy<T>();
        

        private T entity;
    }
}
