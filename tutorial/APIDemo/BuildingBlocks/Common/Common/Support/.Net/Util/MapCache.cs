////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/10/2011 2:21:51 PM 
// Description: MapCache.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

namespace Common.Support.Net.Util
{
    public class MapCache:IDisposable
    {
    	public MapCache()
    	{
    	}
        public bool ContainsKey(string key)
        {
            return Cache.ContainsKey(key);
        }
        public void AddCache(string key, object value)
        {
            if (cache == null) cache = new Dictionary<string, object>();
            cache[key] = value;
        }
        public bool TryGetCache<TValue>(string key, out TValue value)
        {
            value = default(TValue);
            object item;
            if (cache == null || !cache.TryGetValue(key, out item)) return false;

            value = (TValue)item;
            return true;
        }
        public void Clean()
        {
            if (cache == null) return;
            foreach (var item in cache.Values)
            {
                using (item as IDisposable) { }
            }
        }
        private Dictionary<string, object> Cache
        {
            get
            {
                if (cache == null)
                    cache = new Dictionary<string, object>();
                return cache;
            }
        }

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
                    Clean();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        private Dictionary<string, object> cache;
    }
}
