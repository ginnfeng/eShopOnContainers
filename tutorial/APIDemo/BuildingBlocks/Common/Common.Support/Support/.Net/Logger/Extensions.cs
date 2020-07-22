////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/17/2020 2:44:46 PM 
// Description: FileLoggerFactoryExtensions.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Support.Net.Logger
{
    public static class Extensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        {
            //builder.AddConfiguration();
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();            
            return builder;
        }

        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            builder.AddFile();
            builder.Services.Configure(configure);
            return builder;
        }
        
        public static ILoggingBuilder AddDailyFile(this ILoggingBuilder builder)
        {
            //builder.AddConfiguration();            
            builder.Services.AddSingleton<ILoggerProvider, DailyLoggerProvider>();
            return builder;
        }

        public static ILoggingBuilder AddDailyFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            builder.AddDailyFile();            
            builder.Services.Configure(configure);
            return builder;
        }
       
    }
}
