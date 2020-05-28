////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 9:23:38 AM 
// Description: ClientProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using Support.Net.Proxy;
using Support.Net.Util;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;

namespace ApiGw.ClientProxy
{
    class ClientProxy
    {
    }
    public class ClientProxy<TService>
    {
        public ClientProxy(IConfiguration cfg)
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;                        
        }
        public ClientProxy(Uri apiEndpoint)
        {
            ApiEndpoint = apiEndpoint;
            //SwaggerDocEndpoint = swaggerDocEndpoint;
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;
        }
        private Method To(HTTP  method)
        {
            var name=Enum.GetName(typeof(HTTP), method);
            return (Method)Enum.Parse(typeof(Method), name);
        }
        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {

            HttpMethodSpec httpMethodSpec;
            if (!TryGetSpec(methodInfo, out httpMethodSpec))
                throw new Exception("RealProxyInvokeMethodEvent");
            RestClient client = new RestClient(ApiEndpoint);
            var req=client.TakeRequest(httpMethodSpec.Path);
            var parameterDic = new Dictionary<string, object>();
            var parameters=methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterDic[parameters[i].Name] = args[i];
            }            
            //MethodInfo method = typeof(RestClientExt).GetMethod($"{nameof(RestClientExt.TakeRequest)}", 1, new Type[] { typeof(string), typeof(string), typeof(string) });
            //MethodInfo takeRequestMethodInfo = method.MakeGenericMethod(methodInfo.ReturnType);
            //var req = takeRequestMethodInfo.Invoke(typeof(RestClientExt), new object[] { httpMethodSpec.Path, null, null }) as DynamicRestRequest;//DynamicRestRequest<T>
            req.Node.Method = To(httpMethodSpec.HttpMethod);
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < httpMethodSpec.ParameterSpecs.Count; i++)
                {
                    var parameterSpec = httpMethodSpec.ParameterSpecs[i];
                    switch (parameterSpec.From)
                    {
                        case ParameterFrom.QUERY:
                            req.Node.AddQueryParameter(parameterSpec.name, parameterDic[parameterSpec.name].ToString());
                            break;
                        case ParameterFrom.FORM:
                            req.Node.AddParameter(parameterSpec.name, parameterDic[parameterSpec.name].ToString());
                            break;
                        case ParameterFrom.BODY:
                            req.Node.AddJsonBody(args[httpSpecFactory.ServiceSpec.IsVersionInRoutePath()?i-1:i]);
                            break;
                        case ParameterFrom.HEADER:
                            req.Node.AddHeader(parameterSpec.name, parameterDic[parameterSpec.name].ToString());
                            break;
                        case ParameterFrom.PATH:
                            object v;
                            if(parameterDic.TryGetValue(parameterSpec.name,out v))
                                req.Node.AddUrlSegment($"{parameterSpec.name}", v);
                            else
                                req.Node.AddUrlSegment($"{parameterSpec.name}", this.ApiVersion);
                            break;                        
                    }                    
                }
            }
            var response = client.Execute(req);            
            JsonToObjectGerericMethod??= typeof(IRestResponseExt).GetMethod($"{nameof(IRestResponseExt.Json2Object)}");
            MethodInfo exevcMethodInfo = JsonToObjectGerericMethod.MakeGenericMethod(methodInfo.ReturnType);            
            var rlt = exevcMethodInfo.Invoke(typeof(IRestResponseExt), new object[] { response,null });
            return rlt;
        }        
        public TService Api
        {
            get { return realProxy.Entity; }
        }
        public Uri ApiEndpoint { get; set; }
        public string ApiVersion
        {
            get {return apiVersion.ToString(); } 
            set { apiVersion = Microsoft.AspNetCore.Mvc.ApiVersion.Parse(value); }
        }
        public void RegisterSwaggerDoc(Uri endpoint,bool lazyDo=true)
        {
            swaggerDocEndpoint = endpoint;
            if (lazyDo)
                httpSpecFactory = null;
            else
            {
                httpSpecFactory = HttpSpecFactory<TService>.Instance;
                httpSpecFactory.RegisterSwaggerDoc(swaggerDocEndpoint);
            }

        }
        private bool TryGetSpec(MethodInfo methodInfo, out HttpMethodSpec httpMethodSpec)
        {
            if (httpSpecFactory == null)
                RegisterSwaggerDoc(swaggerDocEndpoint, false);
            return httpSpecFactory.TryGetValue(methodInfo, out httpMethodSpec);
        }
        private MethodInfo JsonToObjectGerericMethod;
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        private IHttpSpecFactory httpSpecFactory ;
        private ApiVersion apiVersion =new ApiVersion(1,0);
        private Uri swaggerDocEndpoint;
    }
}
