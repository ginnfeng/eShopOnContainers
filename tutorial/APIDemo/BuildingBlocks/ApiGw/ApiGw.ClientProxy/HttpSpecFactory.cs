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
            BuildServiceInterfaceSpec();
        }
        public bool TryGetHttpMethodSpec(MethodInfo methodInfo, out HttpMethodSpec sepc)
        {
            return swaggerSpecDic.TryGetValue(methodInfo.Name,out sepc);
        }
        public void RegisterSwaggerDoc(Uri endpoint)
        {
            swaggerSpecDic ??= new Dictionary<string, HttpMethodSpec>();
            swaggerSpecDic.Clear();
            RestClient client = new RestClient($"{endpoint.Scheme}://{endpoint.IdnHost}:{endpoint.Port}");
            var req = client.TakeRequest<JObject>(endpoint.LocalPath);
            var content = client.Execute(req);
            var httpMethodSpecList = new List<HttpMethodSpec>();
            foreach (JProperty prop in content["paths"])
            {
                var methodSpec=new HttpMethodSpec() { Path = prop.Name };
                httpMethodSpecList.Add(methodSpec);
                foreach (JProperty method in prop.Values())
                {                   
                    var tags = method.Value["tags"] as JArray;
                    if (tags != null && tags.Count > 0)
                        methodSpec.Tag=tags.First.Value<string>();
                    var parameters = method.Value["parameters"];
                    var parameterSpecArray = (parameters != null) ? parameters.Value<JArray>() : null;
                    if (parameterSpecArray != null)
                    {
                        methodSpec.ParameterSpecs.AddRange(parameterSpecArray.ToObject<List<HttpMethodParameterSpec>>());                        
                    }
                    if (method.Exists(it => it["requestBody"] != null))
                        methodSpec.ParameterSpecs.Add(new HttpMethodParameterSpec() { _in="body"});
                    
                }
            }
        }
        private void BuildServiceInterfaceSpec()
        {
            Type type = typeof(TServiceInterface);
            serviceInterfaceMethodSpecDic ??= new Dictionary<string, ApiSpecAttribute>();
            serviceInterfaceMethodSpecDic.Clear();
            serviceInterfaceSpec=ApiSpecAttribute.TakeFrom(type);
            var methodInfos=type.GetMethods();
            foreach (var methodInfo in methodInfos)
            {
                serviceInterfaceMethodSpecDic[methodInfo.Name] =ApiSpecAttribute.TakeFrom(type, methodInfo.Name);
            }
        }
        private Dictionary<string, HttpMethodSpec> swaggerSpecDic;
        private Dictionary<string, ApiSpecAttribute> serviceInterfaceMethodSpecDic;
        ApiSpecAttribute serviceInterfaceSpec;
    }
}
