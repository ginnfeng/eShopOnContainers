////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 5:42:59 PM 
// Description: SwaggerDocStore.cs  
// Revisions  :            		
// **************************************************************************** 
using Newtonsoft.Json.Linq;
using RestSharp;
using Common.Support;

using Common.Open.RestSharp;
using System;
using System.Collections.Generic;


namespace ApiGw.ClientProxy
{
    public class SwaggerDocStore: ISwaggerDocStore
    {
        static public SwaggerDocStore Instance
        {
            get { return Singleton0<SwaggerDocStore>.Instance; }
        }
        public SwaggerDocStore()
        {
        }
        public bool TryGetValue(Uri endpoint,string tag,out List<HttpMethodSpec> httpMethodSpecList)
        {
            lock (this)
            {
                var key = GenKey(endpoint, tag);
                return swaggerSpecDic.TryGetValue(key, out httpMethodSpecList);
            }
        }
        public void RegisterSwaggerDoc(Uri endpoint)
        {
            swaggerSpecDic ??= new Dictionary<string, List<HttpMethodSpec>>();
            swaggerSpecDic.Clear();
            RestClient client = new RestClient($"{endpoint.Scheme}://{endpoint.IdnHost}:{endpoint.Port}");
            var req = client.TakeRequest<JObject>(endpoint.LocalPath);
            var content = client.Execute(req);
            var paths = content["paths"];
            foreach (JProperty prop in paths)
            {
                var methodSpec = new HttpMethodSpec() { Path = prop.Name };
                var methodProp = prop.Value.First as JProperty;
                methodSpec.AssignHttpMethod(methodProp.Name);
                var tags = methodProp.Value["tags"] as JArray;
                if (tags != null && tags.Count > 0)
                    methodSpec.Tag = tags.First.Value<string>();
                var parameters = methodProp.Value["parameters"];
                var parameterSpecArray = (parameters != null) ? parameters.Value<JArray>() : null;
                if (parameterSpecArray != null)
                {
                    methodSpec.ParameterSpecs.AddRange(parameterSpecArray.ToObject<List<HttpMethodParameterSpec>>());
                }
                var requestBody=methodProp.Value["requestBody"];
                if (requestBody != null)
                {
                    if (requestBody["content"]["application/json"] !=null)
                        methodSpec.ParameterSpecs.Add(new HttpMethodParameterSpec() { _in = "body" });
                    else
                    {
                        var formData = requestBody["content"]["multipart/form-data"];
                        if(formData!=null)
                        {
                            var formParameters = formData["schema"]["properties"];
                            foreach (JProperty formParameter in formParameters)
                            {
                                methodSpec.ParameterSpecs.Add(new HttpMethodParameterSpec() { name = formParameter.Name, _in = "form" });
                            }
                        }

                    }
                }

                var key=GenKey(endpoint, methodSpec.Tag);
                lock (this)
                {
                    List<HttpMethodSpec> httpMethodSpecList;
                    if (!swaggerSpecDic.TryGetValue(key, out httpMethodSpecList))
                    {
                        httpMethodSpecList = new List<HttpMethodSpec>();
                        swaggerSpecDic[key] = httpMethodSpecList;
                    }
                    httpMethodSpecList.Add(methodSpec);
                }
            }
        }
        private string  GenKey(Uri endpoint,string tag)
        {
            return $"{tag}#{endpoint.ToString()}";
        }
        private Dictionary<string, List<HttpMethodSpec>> swaggerSpecDic;
    
    }
}
