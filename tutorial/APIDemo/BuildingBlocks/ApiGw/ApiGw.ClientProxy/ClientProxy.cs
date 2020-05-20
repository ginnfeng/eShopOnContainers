﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 9:23:38 AM 
// Description: ClientProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using Support.Net.Proxy;
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
            if (!httpSpecFactory.TryGet(methodInfo, out httpMethodSpec))
                throw new Exception("RealProxyInvokeMethodEvent");
            RestClient client = new RestClient(ApiEndpoint);
            MethodInfo method = typeof(RestClientExt).GetMethod($"{nameof(RestClientExt.TakeRequest)}", 1, new Type[] { typeof(string), typeof(string), typeof(string) });
            MethodInfo takeRequestMethodInfo = method.MakeGenericMethod(methodInfo.ReturnType);
            var req = takeRequestMethodInfo.Invoke(typeof(RestClientExt), new object[] { httpMethodSpec.Path, null, null }) as DynamicRestRequest;//DynamicRestRequest<T>
            req.Node.Method = To(httpMethodSpec.HttpMethod);
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < httpMethodSpec.ParameterSpecs.Count; i++)
                {
                    var parameterSpec = httpMethodSpec.ParameterSpecs[i];
                    switch (parameterSpec.From)
                    {
                        case ParameterFrom.QUERY:
                            req.Node.AddQueryParameter(parameterSpec.name, args[i].ToString());
                            break;
                        case ParameterFrom.FORM:
                            req.Node.AddParameter(parameterSpec.name, args[i].ToString());
                            break;
                        case ParameterFrom.BODY:
                            req.Node.AddJsonBody(args[i]);
                            break;
                        case ParameterFrom.HEADER:
                            req.Node.AddHeader(parameterSpec.name, args[i].ToString());
                            break;
                        case ParameterFrom.PATH:
                            req.Node.AddUrlSegment("id", args[i]);
                            break;                        
                    }                    
                }
            }
            var response = client.Execute(req);
            //ToEntity<T>(DynamicRestRequest < T > request, IRestResponse response)
            MethodInfo execMethod = typeof(RestClientExt).GetMethod($"{nameof(RestClientExt.ToEntity)}");
            MethodInfo exevcMethodInfo = execMethod.MakeGenericMethod(methodInfo.ReturnType);
            var rlt=exevcMethodInfo.Invoke(typeof(RestClientExt), new object[] { req, response });
            return rlt;
        }        
        public TService Api
        {
            get { return realProxy.Entity; }
        }
        public Uri ApiEndpoint { get; set; }
        public void RegisterSwaggerDoc(Uri endpoint)
        {
            httpSpecFactory.RegisterSwaggerDoc(endpoint);
        }        
        //private Method ToHttpMethod(HttpMethodSpec spec)
        //{
        //    spec.
        //}
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        private IHttpSpecFactory httpSpecFactory = HttpSpecFactory<TService>.Instance;
    }
}