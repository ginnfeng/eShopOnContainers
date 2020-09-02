////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 10:27:44 AM 
// Description: HttpSpecFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;

using Common.Support;

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
                return Singleton0<HttpSpecFactory<TServiceInterface>>.Instance;
            }
        }
        public HttpSpecFactory()
        {
            swaggerDocStore = SwaggerDocStore.Instance;
            ServiceSpec = ApiSpecAttribute.TakeFrom(typeof(TServiceInterface));
        }
        public bool TryGetValue(MethodInfo methodInfo, out HttpMethodSpec sepc)
        {
           return serviceInterfaceMethodSpecDic.TryGetValue(methodInfo.Name,out sepc);
        }
        public ApiSpecAttribute ServiceSpec { get; private set; }
        public void RegisterSwaggerDoc(Uri endpoint,bool forceReregister=false)
        {
            if (forceReregister || currentSwaggerEndpoint != endpoint) serviceInterfaceMethodSpecDic = null;
            if (serviceInterfaceMethodSpecDic == null)
            {
                currentSwaggerEndpoint = endpoint;
                swaggerDocStore.RegisterSwaggerDoc(endpoint);
                Type type = typeof(TServiceInterface);
                serviceInterfaceMethodSpecDic= new Dictionary<string, HttpMethodSpec>();
                serviceInterfaceMethodSpecDic.Clear();               
                //var matchs=regex.Matches(serviceInterfaceSpec.Template);
                var controllerName = ServiceSpec.ServiceName; //(matchs.Count >= 2) ? matchs[1].Groups[0].Value.ToString() : matchs[0].Groups[0].Value.ToString();
                List <HttpMethodSpec> httpMethodSpecList;
                if (!swaggerDocStore.TryGetValue(endpoint, controllerName, out httpMethodSpecList))
                    throw new KeyNotFoundException("RegisterSwaggerDoc");
                var methodInfos = type.GetMethods();
                foreach (var methodInfo in methodInfos)
                {
                    var apiSpec = ApiSpecAttribute.TakeFrom(type, methodInfo.Name);
                    var template = (apiSpec == null) ? methodInfo.Name : apiSpec.Template;
                    var path =string.IsNullOrEmpty(template) ? $"/{ServiceSpec.SwaggerRoutePath}":$"/{ServiceSpec.SwaggerRoutePath}/{template}";
                    var matchedMethodSpec=httpMethodSpecList.Find(spec=> spec.Path.Contains(path)) ?? throw new KeyNotFoundException("RegisterSwaggerDoc");
                    
                    serviceInterfaceMethodSpecDic[methodInfo.Name] = matchedMethodSpec;
                }
            }
        }
        private Dictionary<string, HttpMethodSpec> serviceInterfaceMethodSpecDic;
        //private Dictionary<string, ApiSpecAttribute> serviceInterfaceMethodSpecDic;
        
        private ISwaggerDocStore swaggerDocStore;
        private  readonly Regex regex = new Regex("([^/]{1,})");
        private Uri currentSwaggerEndpoint;
    }
}
