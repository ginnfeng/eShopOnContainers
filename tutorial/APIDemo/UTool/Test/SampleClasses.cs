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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UTool.Test
{
    #region ****DI sample classes****
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
    #endregion

    #region ****MediatR****

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

    #endregion  

    #region ****AppSettings****
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
    #endregion

    #region ****IpLogParser****
    public class IpLogParser
    {
        public IpLogParser()
        {
        }
        public List<IPAddress> Parse(string log)   //, Action<Match> act = null)
        {
            var iplist=new List<IPAddress>();            
            var matchs=regex.Matches(log);
            foreach (Match match in matchs)
            {
                iplist.Add(IPAddress.Parse(match.Groups[0].ToString()));
            }
            return iplist;
        }
        //(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})
        static readonly Regex regex = new Regex("(\\d{1,3})\\.(\\d{1,3})\\.(\\d{1,3})\\.(\\d{1,3})");
    }
    #endregion  

    #region *****Factory Design Pattern*****
    public interface IInstanceFactortry
    {
        TInterface CreateInstance<TInterface>();
    }
    public class InstanceFactortry : IInstanceFactortry
    {
        public void Register<TInterface, TImplementation>(Func<IInstanceFactortry, TImplementation> ctorFun = null)
            where TImplementation : TInterface, new()
        {
            dic[typeof(TInterface)] = new InstanceCreator<TImplementation>(this, ctorFun); ;
        }
        public TInterface CreateInstance<TInterface>()
        {
            IInstanceCreator ctorFun;
            if (dic.TryGetValue(typeof(TInterface), out ctorFun))
            {
                return (TInterface)ctorFun.Create();
            }
            throw new Exception();
        }

        private Dictionary<Type, IInstanceCreator> dic = new Dictionary<Type, IInstanceCreator>();
    }
    public interface IInstanceCreator
    {
        object Create();
    }
    public class InstanceCreator<TImplementation> : IInstanceCreator
        where TImplementation : new()
    {
        public InstanceCreator(IInstanceFactortry factory, Func<IInstanceFactortry, TImplementation> createFunc)
        {
            this.createFunc = createFunc;
            this.factory = factory;
        }
        public object Create()
        {
            return (createFunc == null) ? new TImplementation() : createFunc(factory);
        }
        private Func<IInstanceFactortry, TImplementation> createFunc;
        private IInstanceFactortry factory;
    }

    public interface IDemoSvc
    {
        string Echo(string s);
    }
    public class DemoSvc : IDemoSvc
    {
        public DemoSvc()
        {
        }
        public DemoSvc(IDemoImp imp)
        {
            this.imp = imp;
        }
        public string Echo(string s)
        {
            return imp.Echo(s);
        }
        IDemoImp imp;
    }

    public interface IDemoImp : IDemoSvc
    {
    }
    public class DemoImp1 : IDemoImp
    {
        public DemoImp1()
        {
        }
        public string PreString { get; set; }
        public string Echo(string s)
        {
            return $"{PreString}{s}";
        }
    }
    public class DemoImp2 : IDemoImp
    {
        public DemoImp2() { }
        public string PostString { get; set; }
        public string Echo(string s)
        {
            return $"{s}{PostString}";
        }
    }
    #endregion  
}
