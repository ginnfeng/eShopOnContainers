////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/8/2020 11:59:15 AM 
// Description: SwaggerHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Policy
{
    public static class SwaggerExt
    {
        //static public string TakeServiceName<TServiceInterface>()
        //{
        //    typeof(TServiceInterface)
        //}
        static public void AddChtSwagger(this IServiceCollection services,string version, string description)
        {
            //version = version.ToLower();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Ordering.Api", Version = "v1" });
            //});
            //return;
            var apiVer = ApiVersion.Parse(version);
            var ver = $"v{apiVer}";
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = apiVer;
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
            });
            var svcName = TakeServiceName();
            var apiInfo = new OpenApiInfo
            {
                Title = $"{svcName} API",
                Version = ver,
                Description = description,
                Contact = new OpenApiContact() { Name = "ApiTeam", Email = "service@cht.com.tw" }
            };
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc(ver, apiInfo);//version此必須與/swagger/{version}/swagger.json配合，且必須為小寫
            });
            //new ApiVersion(1, 0);
            
        }
        static public void UseChtSwagger(this IApplicationBuilder app, string version)
        {
            //version = version.ToLower();
            var ver = ApiVersion.Parse(version);
            var svcName = TakeServiceName();
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(string.Format(SwaggerPathTemplate,ver), $"{svcName} API {ver}");
                c.RoutePrefix = swaggerRoutePrefix;// "swagger" is default
                //swagger UI=> http://localhost:<port>/swagger
                //swagger Doc=> http://localhost:<port>/swagger/v1/swagger.json
            });
        }
        static public string SwaggerGwRoutePrefix { get { return swaggerGwRoutePrefix; } } //"/swagger/docs"
       
        static public string SwaggerPathTemplate { get { return swaggerPathTemplate; } }
        static public string TakeSwaggerPathTemplate<TServiceInterface>()
        {
            var svcName = ResolveServiceName<TServiceInterface>();
            return string.Format("/{0}/v{1}/{2}", swaggerGwRoutePrefix, "{0}", svcName); ///swagger/docs/v1/ordering
        }
        static string TakeServiceName()
        {
            return ResolveServiceName(Assembly.GetEntryAssembly().GetName().Name);            
        }
        static public string ResolveServiceName<TServiceInterface>()
        {
            return ResolveServiceName(typeof(TServiceInterface).Assembly.GetName().Name);
        }
        static public string ResolveServiceName(string moduleName)
        {
            var match = regex.Match(moduleName);
            var svcName= (match.Success)?match.Groups[1].ToString(): moduleName.Replace(".", "");
            return svcName;//.ToLower();
        }
        static readonly Regex regex = new Regex("\\.([^\\.]{1,})\\.");
        static readonly string swaggerRoutePrefix = "swagger";
        static readonly string swaggerGwRoutePrefix = $"{swaggerRoutePrefix}/docs";
        static readonly string swaggerPathTemplate = string.Format("/{0}/v{1}/swagger.json", swaggerRoutePrefix, "{0}");        
    }
}
