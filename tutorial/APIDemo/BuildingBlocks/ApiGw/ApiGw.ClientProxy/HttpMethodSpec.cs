////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 10:17:59 AM 
// Description: HttpMethodSpec.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGw.ClientProxy
{
    public class HttpMethodSpec
    {
        public string Tag { get; set; }
        public string Path { get; set; }
        public List<HttpMethodParameterSpec> ParameterSpecs {
            get { 
                parameterSpecs ??= new List<HttpMethodParameterSpec>();
                return parameterSpecs; 
            }
        }
        private List<HttpMethodParameterSpec> parameterSpecs;
    }
    public class HttpMethodParameterSpec
    {
        public string name { get; set; }
        public string _in { get; set; }
        public bool required { get; set; }
        public SchemaSpec schema { get; set; }
    }

    public class SchemaSpec
    {
        public string type { get; set; }
        public bool nullable { get; set; }
        public string _ref { get; set; }
    }
}
