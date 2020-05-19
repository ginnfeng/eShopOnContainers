////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 9:49:43 PM 
// Description: ApiConfigAttribute.cs  
// Revisions  :      
// https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Core/src/Routing/HttpMethodAttribute.cs
// **************************************************************************** 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Contract
{    
    public enum HTTP
    {
        GET,
        PUT,
        POST,
        DELETE
    }
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class ApiSpecAttribute : Attribute, IActionHttpMethodProvider, IRouteTemplateProvider
    {
        /// <summary>
        /// For Service Interface only
        /// </summary>
        public ApiSpecAttribute(string template) 
        //: this(new string[] { "GET" }, template)
        {
            HttpMethods = new string[] { };
            Template = template;
        }
        /// <summary>
        /// For Controller class
        /// </summary>
        public ApiSpecAttribute(Type fromInterfaceType)//For Controller
        {
            CopyFrom(TakeFrom(fromInterfaceType));
            HttpMethods = new string[] { };
        }
        /// <summary>
        /// For Controller Method
        /// </summary>
        public ApiSpecAttribute(HTTP httpMethod, Type fromInterfaceType, string methodName)
            : this(new HTTP[] { httpMethod }, fromInterfaceType, methodName)
        {
            
        }
        /// <summary>
        /// For Controller Method
        /// </summary>
        public ApiSpecAttribute(IEnumerable<HTTP> httpMethods,Type fromInterfaceType, string methodName)
        {
            CopyFrom(TakeFrom(fromInterfaceType, methodName));
            var methods = new string[httpMethods.Count()];
            for (int i = 0; i < httpMethods.Count(); i++)
            {
                methods[i] = httpMethods.ElementAt(i).ToString();
            }            
            HttpMethods = methods;            
        }
        
        
        public IEnumerable<string> HttpMethods { get; private set; }

        public string Template { get; private set; }

        public int? Order { get; private set; }

        public string Name { get; private set; }
        private void CopyFrom(ApiSpecAttribute baseAttri)
        {
            if (baseAttri == null) return;
            Template = baseAttri.Template;
            //HttpMethods = baseAttri.HttpMethods;            
            //Order = baseAttri.Order;
            //Name = baseAttri.Name;
        }
        public static ApiSpecAttribute TakeFrom(Type interfaceType)
        {
            var attris = interfaceType.GetCustomAttributes(typeof(ApiSpecAttribute), true);
            return  (attris.Length > 0)? (ApiSpecAttribute)attris[0]:null;
           
        }

        public static ApiSpecAttribute TakeFrom(Type interfaceType, string methodName)
        {
            var method = interfaceType.GetMethod(methodName);
            var attris = method.GetCustomAttributes(typeof(ApiSpecAttribute), true);
            return (attris.Length > 0) ? (ApiSpecAttribute)attris[0] : null;
        }
        
    }

    //[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    //public class ApiSpecAttribute2 : Attribute, IActionHttpMethodProvider, IRouteTemplateProvider
    //{
    //    public ApiSpecAttribute2(Type interfaceType)
    //    {
    //        routeConfig = new RouteConfigAttribute(interfaceType);
    //    }
    //    public ApiSpecAttribute2(Type interfaceType, string methodName)
    //    {
    //        routeConfig = new RouteConfigAttribute(interfaceType, methodName);
    //        httpConfig = new HttpConfigAttribute(interfaceType, methodName);
    //    }
    //    public IEnumerable<string> HttpMethods => httpConfig != null ? httpConfig.HttpMethods : new[] { "GET" };

    //    public string Template =>
    //        routeConfig != null ? routeConfig.Template :
    //        httpConfig != null ? httpConfig.Template : "";

    //    public int? Order => routeConfig.Order;

    //    public string Name => routeConfig.Name;
    //    private HttpConfigAttribute httpConfig;
    //    private RouteConfigAttribute routeConfig;
    //}
}
