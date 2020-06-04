////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/1/2020 10:12:27 AM 
// Description: ClientProxyExt.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ApiGw.ClientProxy;
using Common.Policy;

namespace ApiGw.ClientProxy.Ext
{
    static public class ClientProxyExt
    {
        static public void RegisterChtSwaggerDoc<TService>(this ClientProxy<TService> it, bool useApiGateway)
            where TService:class
        {
            //Ordering/api/v1/HelloWorld
            var host = $"{it.ApiEndpoint.Scheme}://{it.ApiEndpoint.Host}:{it.ApiEndpoint.Port}";
            var swaggerPath = useApiGateway ?
                string.Format(SwaggerExt.TakeSwaggerPathTemplate<TService>(), it.ApiVersion)
                : string.Format(SwaggerExt.SwaggerPathTemplate,it.ApiVersion);
            var swaggerEndpoint=new Uri($"{host}{swaggerPath}");
            it.RegisterSwaggerDoc(swaggerEndpoint);
            //if (useApiGateway)
            //{
            //    it.ApiEndpoint = new Uri($"{host}/{SwaggerExt.ResolveServiceName<TService>()}/api/v{it.ApiVersion}");
            //}
        }        
    }
}
