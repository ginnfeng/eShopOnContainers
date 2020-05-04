using Support.Net.Proxy;
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 9/18/2013 6:07:52 PM 
// Description: AppDomainFactory.cs  
// object跨AppDomain可兩種方式
// 1.繼承MarshalByRefObject，會變成一個Proxy Object
// 2.標示[Serializable],會clone object
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

namespace Support.Net.Util
{
    /*
    public class AppDomainFactory
    {
        static AppDomainFactory()
        {
            //非常重要,否則會造成(T)appDomain.CreateInstanceFrom(..).Unwrap(),Unable to cast transparent proxy to xxx type之error
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyContainer.Instance.OnAssemblyResolve;
        }
        static public AppDomainFactory Instance { get { return Support.Singleton<AppDomainFactory>.Instance; } }

        public AppDomainFactory()
        {
        }
        public bool TryGetAppDomain(string appDomainName, out DisposableAdapter<AppDomain> appDomainProxy)
        {
            return appDomainMap.TryGetValue(appDomainName, out appDomainProxy);
        }
        public DisposableAdapter<AppDomain> TakeNewAppDomain(string appDomainName)
        {
            return TakeNewAppDomain(appDomainName,AppDomain.CurrentDomain.BaseDirectory );
        }
        public DisposableAdapter<AppDomain> TakeNewAppDomain(string appDomainName, string applicationBase)
        {
            if (AppDomain.CurrentDomain.FriendlyName.Equals(appDomainName))
            {
                var currentDomain = AppDomain.CurrentDomain;
                return new DisposableAdapter<AppDomain>(ref currentDomain);
            }
            
            UnloadAppDomain(appDomainName);
            
            lock (this)
            {
                Support.Log.DailyLogger.Instance.Write<AppDomainFactory>("TakeNewAppDomain:" + appDomainName);
                AppDomainSetup setup = new AppDomainSetup();
                setup.PrivateBinPath = applicationBase;  
                setup.ApplicationBase = applicationBase;            
                setup.ApplicationName = appDomainName;
                
                //setup.LoaderOptimization = LoaderOptimization.SingleDomain;
                AppDomain appDomain = AppDomain.CreateDomain(appDomainName, null, setup);
                var appDomainProxy = new DisposableAdapter<AppDomain>(ref appDomain);
                appDomainProxy.DisposeEvent += OnAppDomainProxyDisposeEvent;
                appDomainMap[appDomainName] = appDomainProxy;
                
                return appDomainProxy;
            }
        }
        public void UnloadAppDomain(string appDomainName)
        {
            DisposableAdapter<AppDomain> appDomainProxy;
            lock (this)
            {   
                if (appDomainMap.TryGetValue(appDomainName, out appDomainProxy))
                {
                    appDomainProxy.Dispose();
                }  
            }
        }
        public void UnloadAllAppDomain()
        {
            DisposableAdapter<AppDomain>[] appDomains;
            lock (this)
            {
                appDomains=new DisposableAdapter<AppDomain>[appDomainMap.Values.Count];
                appDomainMap.Values.CopyTo(appDomains,0);
            }
            foreach (var appDomain in appDomains)
            {
                appDomain.Dispose();
            }            
        }
        void OnAppDomainProxyDisposeEvent(IDisposableAdapter<AppDomain> it)
        {
            AppDomain appDomain = it.Entity;
            lock (this)
            {
                if (appDomain != null && appDomainMap.ContainsKey(appDomain.FriendlyName))
                {
                    Support.Log.DailyLogger.Instance.Write<AppDomainFactory>("OnAppDomainProxyDisposeEvent:" + appDomain.FriendlyName);
                    appDomainMap.Remove(appDomain.FriendlyName);
                    AppDomain.Unload(appDomain);
                }
            }

        }

        private Dictionary<string, DisposableAdapter<AppDomain>> appDomainMap = new Dictionary<string, DisposableAdapter<AppDomain>>();
    }
    */
}
