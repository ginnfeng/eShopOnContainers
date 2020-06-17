////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/13/2020 3:04:28 PM 
// Description: Test_ClientProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using ApiGw.ClientProxy;
using ApiGw.ClientProxy.Ext;
using Newtonsoft.Json.Linq;
using RestSharp;
using Service.HelloWorld.ApiImp;
using Service.HelloWorld.Contract.Entity;
using Service.HelloWorld.Contract.Servic;

using Support.Net.Util;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
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
            HttpSpecFactory<HelloWorldService>.Instance.RegisterSwaggerDoc(new Uri(swaggerDocUrl));
        }
        private void CallApi(ClientProxy<IHelloWorldService> proxy)
        {
            IHelloWorldService helloSvc = proxy.Svc;
            string id1 = "*abc*";
            int id2 = 99;
            DateTime id3 = DateTime.Today;
            var id4 = new HelloInput() { UserName = "Lee", Date = DateTime.Today };
            var rlt = helloSvc.Hello(id1, id2, id3, id4);
            print($"API={nameof(IHelloWorldService.Hello)} User={rlt.UserName}\nDate={rlt.Date}\n{rlt.Summary}");

            var postrlt = helloSvc.HelloPost("CCC", "DDD");
            print($"API={nameof(IHelloWorldService.HelloPost)} result={postrlt}");

            var getrlt = proxy.Svc.HelloGet("EEE", "FFF");
            print($"API={nameof(IHelloWorldService.HelloGet)} result={getrlt}");
        }
        [UMethod]
        public void T_ClientProxy(string apiUrl,string swaggerDocUrl)
        {
            var proxy = new ClientProxy<IHelloWorldService>(new Uri(apiUrl));
            proxy.ApiVersion = "1";
            proxy.RegisterSwaggerDoc(new Uri(swaggerDocUrl));
            CallApi(proxy);
        }
        
        [UMethod]
        public void T_ClientProxyByGW(string apiUrl)
        {
            var proxy = new ClientProxy<IHelloWorldService>(new Uri(apiUrl));
            proxy.ApiVersion = "1";
            proxy.RegisterChtSwaggerDoc(useApiGateway: true);
            CallApi(proxy);            
        }
        [UMethod]
        async public void T_ClientProxyAsync(string apiUrl)
        {
            var proxy = new ClientProxy<IHelloWorldService>(new Uri(apiUrl));
            proxy.ApiVersion = "1";// 
            //proxy.RegisterSwaggerDoc(new Uri(swaggerDocUrl));
            proxy.RegisterChtSwaggerDoc(useApiGateway: false);
            var getrlt = await Task.Run<string>(() => proxy.Svc.HelloGet("EEE", "FFF"));
            printMsg($"API={nameof(IHelloWorldService.HelloGet)} result={getrlt}");
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
