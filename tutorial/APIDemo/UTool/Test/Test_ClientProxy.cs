////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/13/2020 3:04:28 PM 
// Description: Test_ClientProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using ApiGw.ClientProxy;
using Newtonsoft.Json.Linq;
using RestSharp;
using Service.Ordering.Contract.Servic;
using Service.Ordering.Contract.Service;
using Support.Net.Util;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using UTDll;
namespace UTool.Test
{
    public class HttpMethodSpec
    {
        public string Tag { get; set; }
        public string Path { get; set; }
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



    class Test_ClientProxy : UTest
    {
        public Test_ClientProxy()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        [UMethod]
        public void T_HttpSpecFactory(string swaggerDocUrl)
        {
            HttpSpecFactory<IHelloWorldService>.Instance.RegisterSwaggerDoc(new Uri(swaggerDocUrl));
        }
        [UMethod]
        public void T_ParseSwaggerJson(int port)
        {// TODO: Add Testing logic here
            //https://localhost:44363/swagger/v1/swagger.json
            //Uri r = new Uri("https://localhost:44363/swagger/v1/swagger.json");
            RestClient client = new RestClient($"https://localhost:{port}");
            var req = client.TakeRequest<JObject>("/swagger/v1/swagger.json");
            var content = client.Execute(req);            
            foreach (JProperty prop in content["paths"])
            {
                print($"routePath={prop.Name}");
                foreach (JProperty method in prop.Values())
                {
                    print($"httpMethod={method.Name}");
                    var tags = method.Value["tags"] as JArray;
                    var parameters = method.Value["parameters"];
                    var parameterSpecArray = (parameters != null) ? parameters.Value<JArray>() : null;
                    if (parameterSpecArray != null)
                    {
                        var y = parameterSpecArray.ToObject<List<HttpMethodParameterSpec>>();
                        foreach (JObject item in parameterSpecArray.Children())
                        {
                            //item.ToObject
                            //var x=item.Value<JObject>();
                            var x1 = item.ToObject<HttpMethodParameterSpec>();
                        }
                    }
                    bool b = method.Exists(it => it["requestBody"] != null);
                    if (b)
                    {
                        var requestBodyContent = method.Value["requestBody"]["content"]["application/json"].ToObject<HttpMethodParameterSpec>();

                    }
                }
            }


        }
    }
}
