////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 10:17:59 AM 
// Description: HttpMethodSpec.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGw.ClientProxy
{
    
    public enum ParameterFrom
    {
        QUERY,
        FORM,
        BODY,
        HEADER,
        PATH
    }
    public class HttpMethodSpec
    {
        public string Tag { get; set; }
        public HTTP HttpMethod { get; set; }
        public string Path { get; set; }
        public List<HttpMethodParameterSpec> ParameterSpecs {
            get { 
                parameterSpecs ??= new List<HttpMethodParameterSpec>();
                return parameterSpecs; 
            }
        }
        public void AssignHttpMethod(string methodString)
        {
            HttpMethod=(HTTP)Enum.Parse(typeof(HTTP), methodString.ToUpper());
        }
        private List<HttpMethodParameterSpec> parameterSpecs;
    }
    public class HttpMethodParameterSpec
    {
        public string name { get; set; }
        [JsonProperty("in")]
        public string _in { 
            //get;
            set { AssignParameterFrom(value); }
        }
        public bool required { get; set; }
        public SchemaSpec schema { get; set; }
        public ParameterFrom From { get; set; }
        public void AssignParameterFrom(string methodString)
        {
            From = (ParameterFrom)Enum.Parse(typeof(ParameterFrom), methodString.ToUpper());
        }
    }

    public class SchemaSpec
    {
        public string type { get; set; }
        public bool nullable { get; set; }
        
        [JsonProperty("$ref")]
        public string _ref { get; set; }
    }
}
