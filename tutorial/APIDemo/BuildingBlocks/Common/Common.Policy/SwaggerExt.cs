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
            var swaggerRoutePrefix = "swagger";
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{ver}/swagger.json", $"{svcName} API {ver}");
                c.RoutePrefix = $"{swaggerRoutePrefix}";// is default
                //swagger UI=> http://localhost:<port>/swagger
                //swagger Doc=> http://localhost:<port>/swagger/v1/swagger.json
            });
        }
        static string TakeServiceName()
        {
            var callingAssemblyName= Assembly.GetEntryAssembly().GetName().Name;
            var match = regex.Match(callingAssemblyName);
            if (match.Success)
                return match.Groups[1].ToString();
            return callingAssemblyName.Replace(".","");
        }
        static readonly Regex regex = new Regex("\\.([^\\.]{1,})\\.");
    }
}
