////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/27/2020 2:32:54 PM 
// Description: CfgEntity.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Policy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Support;
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ApiGw.Ocelot
{

    public class CfgGen
    {
        static public CfgGen Instance
        {
            get
            {
                return Singleton0<CfgGen>.Instance;
            }
        }
        public CfgGen()
        {
        }
        public string GenOcelotJsonFile()
        {
            //var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                //.SetFileProvider(new PhysicalFileProvider(basePath))                
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true);
            var cfg= builder.Build();
            var registeredServices = cfg.GetSection("RegisteredServices").Get<List<string>>();
            var svcDefs = new List<ServiceDef>();
            foreach (var svcTagName in registeredServices)
            {
                svcDefs.Add(cfg.GetSection(svcTagName).Get<ServiceDef>());
            }
            return GenOcelotJsonFile(svcDefs);
        }
        public string GenOcelotJsonFile(List<ServiceDef> svcDefs)
        {
            var rootCfg = new CfgRootobject() { GlobalConfiguration = new Globalconfiguration() {BaseUrl= "http://localhost" }};
            var reroutes = new List<Reroute>();
            var swaggerendpoints = new List<Swaggerendpoint>();
            foreach (var svcDef in svcDefs)
            {
                var svcAliasName = SwaggerExt.ResolveServiceName(svcDef.Host);
                var everything = "{everything}";
                var route = new Reroute()
                {
                    DownstreamHostAndPorts = new Downstreamhostandport[] { new Downstreamhostandport() { Host = svcDef.Host, Port = 80 } }
                    ,DownstreamScheme = "http"
                    ,DownstreamPathTemplate = $"/api/{everything}"
                    ,UpstreamPathTemplate= $"/{svcAliasName}/api/{everything}"
                    ,UpstreamHttpMethod=new string[] { "POST", "PUT", "GET" ,"DELETE"}
                    ,SwaggerKey= svcAliasName
                };
                //foreach (var ver in svcDef.Versions)
                {
                    var ver = svcDef.Versions.Last();
                    var swaggerPath=string.Format(SwaggerExt.SwaggerPathTemplate, ver);
                    var swaggerendpoint = new Swaggerendpoint()
                    {
                        Key = svcAliasName                        
                        ,Config = new Config[] { new Config() { Name = svcAliasName, Version = $"v{ver}", Url = $"http://{svcDef.Host}{swaggerPath}" } }
                    };
                    swaggerendpoints.Add(swaggerendpoint);
                }
                
                reroutes.Add(route);
                
            }
            rootCfg.ReRoutes = reroutes.ToArray();
            rootCfg.SwaggerEndPoints = swaggerendpoints.ToArray();
            var ts=new JsonNetTransfer();
            ts.NeedFormatting = true;
            var jsonPath = GetOcelotJsonPath();
            ts.Save(rootCfg, jsonPath );
            return jsonPath;
        }
        public string GetOcelotJsonPath()
        {
            return Path.Combine("configuration", "ocelot.json"); ;
        }
    }
   
}
