////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 10:27:44 AM 
// Description: HttpSpecFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Common.Policy;
using Newtonsoft.Json.Linq;
using RestSharp;
using Support;
using Support.Net.Util;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ApiGw.ClientProxy
{
    

    public class HttpSpecFactory<TServiceInterface>: IHttpSpecFactory
    {
        static public HttpSpecFactory<TServiceInterface> Instance
        {
            get
            {
                return Singleton<HttpSpecFactory<TServiceInterface>>.Instance;
            }
        }
        public HttpSpecFactory()
        {
            swaggerDocStore = SwaggerDocStore.Instance;            
        }
        public bool TryGet(MethodInfo methodInfo, out HttpMethodSpec sepc)
        {
           return serviceInterfaceMethodSpecDic.TryGetValue(methodInfo.Name,out sepc);
        }
        public void RegisterSwaggerDoc(Uri endpoint,bool forceReregister=false)
        {
            if (forceReregister) serviceInterfaceMethodSpecDic = null;
            if (serviceInterfaceMethodSpecDic == null)
            {
                swaggerDocStore.RegisterSwaggerDoc(endpoint);
                Type type = typeof(TServiceInterface);
                serviceInterfaceMethodSpecDic= new Dictionary<string, HttpMethodSpec>();
                serviceInterfaceMethodSpecDic.Clear();
                serviceInterfaceSpec = ApiSpecAttribute.TakeFrom(type);
                var matchs=regex.Matches(serviceInterfaceSpec.Template);
                var controllerName = (matchs.Count >= 2) ? matchs[1].Groups[0].Value.ToString() : matchs[0].Groups[0].Value.ToString();
                List <HttpMethodSpec> httpMethodSpecList;
                if (!swaggerDocStore.TryGetValue(endpoint, controllerName, out httpMethodSpecList))
                    throw new KeyNotFoundException("RegisterSwaggerDoc");
                var methodInfos = type.GetMethods();
                foreach (var methodInfo in methodInfos)
                {
                    
                    var apiSpec = ApiSpecAttribute.TakeFrom(type, methodInfo.Name);
                    var path =string.IsNullOrEmpty( apiSpec.Template)? $"/{serviceInterfaceSpec.Template}":$"/{serviceInterfaceSpec.Template}/{apiSpec.Template}";
                    var matchedMethodSpec=httpMethodSpecList.Find(spec=> spec.Path.Equals(path));
                    if(matchedMethodSpec==null)
                        throw new KeyNotFoundException("RegisterSwaggerDoc");
                    serviceInterfaceMethodSpecDic[methodInfo.Name] = matchedMethodSpec;
                }
            }
        }
        private Dictionary<string, HttpMethodSpec> serviceInterfaceMethodSpecDic;
        //private Dictionary<string, ApiSpecAttribute> serviceInterfaceMethodSpecDic;
        ApiSpecAttribute serviceInterfaceSpec;
        ISwaggerDocStore swaggerDocStore;
        readonly Regex regex = new Regex("([^/]{1,})");
    }
}
