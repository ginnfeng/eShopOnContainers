////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 5:42:59 PM 
// Description: SwaggerDocStore.cs  
// Revisions  :            		
// **************************************************************************** 
using Newtonsoft.Json.Linq;
using RestSharp;
using Support;
using Support.Net.Util;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGw.ClientProxy
{
    public class SwaggerDocStore
    {
        static public SwaggerDocStore Instance
        {
            get { return Singleton<SwaggerDocStore>.Instance; }
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
                var methodSpec = new HttpMethodSpec() { Path = prop.Name };
                httpMethodSpecList.Add(methodSpec);
                foreach (JProperty method in prop.Values())
                {
                    var tags = method.Value["tags"] as JArray;
                    if (tags != null && tags.Count > 0)
                        methodSpec.Tag = tags.First.Value<string>();
                    var parameters = method.Value["parameters"];
                    var parameterSpecArray = (parameters != null) ? parameters.Value<JArray>() : null;
                    if (parameterSpecArray != null)
                    {
                        methodSpec.ParameterSpecs.AddRange(parameterSpecArray.ToObject<List<HttpMethodParameterSpec>>());
                    }
                    if (method.Exists(it => it["requestBody"] != null))
                        methodSpec.ParameterSpecs.Add(new HttpMethodParameterSpec() { _in = "body" });

                }
            }
        }
        private Dictionary<string, HttpMethodSpec> swaggerSpecDic;
    
    }
}
