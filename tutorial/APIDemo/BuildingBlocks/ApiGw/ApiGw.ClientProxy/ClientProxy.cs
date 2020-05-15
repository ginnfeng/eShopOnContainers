////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 9:23:38 AM 
// Description: ClientProxy.cs  
// Revisions  :            		
// **************************************************************************** 
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

        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {
            RestClient client = new RestClient(ApiEndpoint);
            var req = client.TakeRequest<JObject>("/swagger/v1/swagger.json");
            return null;
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
        
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        private IHttpSpecFactory httpSpecFactory = HttpSpecFactory<TService>.Instance;
    }
}
