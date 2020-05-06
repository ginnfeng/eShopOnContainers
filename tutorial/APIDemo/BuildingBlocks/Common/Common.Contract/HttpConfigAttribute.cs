////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 4:49:58 PM 
// Description: HttpBasisAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Contract
{
    public class HttpConfigAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "GET" };
        public HttpConfigAttribute(Type interfaceType, string methodName)
            : base(GetHttpMethodAttribute(interfaceType, methodName).HttpMethods)
        {
        }
        public HttpConfigAttribute(Type interfaceType, string methodName, string template)
            : base(GetHttpMethodAttribute(interfaceType, methodName).HttpMethods, template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }
        static private HttpMethodAttribute GetHttpMethodAttribute(Type interfaceType, string methodName)
        {
            var method = interfaceType.GetMethod(methodName);
            var attris = method.GetCustomAttributes(typeof(HttpMethodAttribute), true);
            return (attris.Length > 0) ? (HttpMethodAttribute)attris.First() : new HttpGetAttribute() { };
        }

    }
}
