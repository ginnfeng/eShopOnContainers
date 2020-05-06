////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 3/19/2020 3:17:11 PM 
// Description: SampleClasses.cs  
// Revisions  :            		
// **************************************************************************** 


using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UTool.Test
{
    public interface IMessage
    {
        string Write(string message);
    }
    public class ConsoleMessage : IMessage
    {
        int HashCode;
        public ConsoleMessage()
        {

            HashCode = this.GetHashCode();
            Debug.WriteLine($"ConsoleMessage ({HashCode}) 已經被建立了");
        }
        public string Write(string message)
        {
            string result = $"[Console 輸出  ({HashCode})] {message}";
            Debug.WriteLine(result);

            return result;
        }
        ~ConsoleMessage()
        {
            Debug.WriteLine($"ConsoleMessage ({HashCode}) 已經被釋放了");

        }
    }


    //***************************MediatR****************************************
    public class SampleCommand : IRequest<bool>, INotification
    {
        public enum CmdType
        {
            Add,
            Insert,
            Delete
        }
        public CmdType Id { get; set; }
        public string Content { get; set; }
    }

    public class SampleCommandHandler : IRequestHandler<SampleCommand, bool>
    {
        private readonly ILogger<SampleCommandHandler> logger;
        public SampleCommandHandler(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<SampleCommandHandler>();

        }
        public Task<bool> Handle(SampleCommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd.Id)
            {
                case SampleCommand.CmdType.Add:
                    logger.LogDebug("Exec: Add");
                    break;
                case SampleCommand.CmdType.Insert:
                    logger.LogDebug("Exec: Insert");
                    break;
                case SampleCommand.CmdType.Delete:
                    logger.LogDebug("Exec: Insert");
                    break;
                default:
                    break;
            }
            logger.LogInformation($"Content={cmd.Content}");
            return Task.FromResult(true);
        }
    }
    public class SampleNotificationHandler1 : INotificationHandler<SampleCommand>
    {// INotificationHandler notification to n handlers. don't return value.
        private readonly ILogger<SampleCommandHandler> logger;
        public SampleNotificationHandler1(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<SampleCommandHandler>();
        }
        Task INotificationHandler<SampleCommand>.Handle(SampleCommand cmd, CancellationToken cancellationToken)
        {
            logger.LogInformation($"SampleNotificationHandler1 Content={cmd.Content}");
            return Task.CompletedTask;
        }
    }
    public class SampleNotificationHandler2 : INotificationHandler<SampleCommand>
    {// INotificationHandler notification to n handlers. don't return value.
        private readonly ILogger<SampleCommandHandler> logger;
        public SampleNotificationHandler2(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<SampleCommandHandler>();
        }
        Task INotificationHandler<SampleCommand>.Handle(SampleCommand cmd, CancellationToken cancellationToken)
        {
            logger.LogInformation($"SampleNotificationHandler2 Content={cmd.Content}");
            return Task.CompletedTask;
        }
    }



    public class ClientInfo
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }


    public class AppSettingsHelper
    {
        //private IConfigurationRoot _configuration;
        private IConfiguration _configuration;

        private readonly IConfigurationBuilder _builder;

        private static readonly Lazy<AppSettingsHelper> Lazy =
            new Lazy<AppSettingsHelper>(() => new AppSettingsHelper());

        public static AppSettingsHelper Instance
        {
            get { return Lazy.Value; }
        }

        AppSettingsHelper()
        {
            var basePath = Directory.GetCurrentDirectory();
            _builder = new ConfigurationBuilder()
                .SetFileProvider(new PhysicalFileProvider(basePath))
                //.AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true);

            _configuration = _builder.Build();
        }

        public string GetValueFromKey(string key)
        {
            return _configuration.GetSection(key).Value;
        }
    }
}
