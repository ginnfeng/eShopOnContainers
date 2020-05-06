////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 9:49:43 PM 
// Description: ApiConfigAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Contract
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class ApiConfigAttribute : Attribute, IActionHttpMethodProvider, IRouteTemplateProvider
    {
        public ApiConfigAttribute(Type interfaceType)
        {
            routeConfig = new RouteConfigAttribute(interfaceType);
        }
        public ApiConfigAttribute(Type interfaceType, string methodName)
        {
            routeConfig = new RouteConfigAttribute(interfaceType, methodName);
            httpConfig = new HttpConfigAttribute(interfaceType, methodName);
        }
        public IEnumerable<string> HttpMethods => httpConfig!=null?httpConfig.HttpMethods: new[] {"GET"};

        public string Template => routeConfig!=null? routeConfig.Template:"";

        public int? Order => routeConfig.Order;

        public string Name => routeConfig.Name;
        private HttpConfigAttribute httpConfig;
        private RouteConfigAttribute routeConfig;
    }
}
