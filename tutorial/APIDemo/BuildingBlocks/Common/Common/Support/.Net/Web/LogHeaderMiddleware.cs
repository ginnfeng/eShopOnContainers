////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/15/2020 10:35:05 AM 
// Description: LogHeaderMiddleware.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Support.Net.Web
{
    public class LogHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        public static string CorrelationHeader => "x-correlation-id";
        public LogHeaderMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers[CorrelationHeader];
            if (header.Count > 0)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<LogHeaderMiddleware>>();
                using (logger.BeginScope("{@id}", header[0]))
                {
                    await this._next(context);
                }
            }
            else
            {
                await this._next(context);
            }
        }
    }
}
