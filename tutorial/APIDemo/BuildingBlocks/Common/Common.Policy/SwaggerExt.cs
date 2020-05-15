////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/8/2020 11:59:15 AM 
// Description: SwaggerHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Builder;
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
        static public void AddChtSwagger(this IServiceCollection services,string version, string description)
        {
            version = version.ToLower();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Ordering.Api", Version = "v1" });
            //});
            //return;
            var svcName = TakeServiceName();
            var apiInfo = new OpenApiInfo
            {
                Title = $"{svcName} API",
                Version = version,
                Description = description,
                Contact = new OpenApiContact() { Name = "ApiTeam", Email = "service@cht.com.tw" }
            };
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc(version, apiInfo);//version此必須與/swagger/{version}/swagger.json配合，且必須為小寫
            });
        }
        static public void UseChtSwagger(this IApplicationBuilder app, string version)
        {
            version = version.ToLower();
            var svcName = TakeServiceName();
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            var swaggerRoutePrefix = "swagger";
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{svcName} API {version}");
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
