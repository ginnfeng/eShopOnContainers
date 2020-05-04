////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/18/2020 12:00:15 PM 
// Description: Test_DI.cs  
// Revisions  :            		
// **************************************************************************** 
using MediatR;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using UTDll;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace UTool.Test
{
    
    class Test_DI : UTest
    {
        public Test_DI()
        {
            //
            // TODO: Add constructor logic here
            serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
        private IServiceProvider serviceProvider;
        private IServiceCollection serviceCollection;


        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            //LOGGING 
            //https://stackoverflow.com/questions/50849251/net-core-di-logger-on-console-app-not-logging-to-console
            //https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
            
            serviceCollection.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));
            serviceCollection.AddTransient<IRequestHandler<SampleCommand, bool>, SampleCommandHandler>();
            serviceCollection.AddTransient<INotificationHandler<SampleCommand>, SampleNotificationHandler1>();
            serviceCollection.AddTransient<INotificationHandler<SampleCommand>, SampleNotificationHandler2>();
            serviceCollection.AddMediatR(typeof(IMediator));
            //serviceCollection.AddMediatR(typeof(Test_DI)); //以此可自動註冊所有Handler class

            serviceCollection.AddSingleton<IConfiguration>(
                sp =>
                {
                    var basePath = Directory.GetCurrentDirectory();
                    var builder = new ConfigurationBuilder()
                        .SetFileProvider(new PhysicalFileProvider(basePath))
                        //.AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                            optional: true);
                    return builder.Build();
                }
                );
        }
        [UMethod]
        public void T_MediatorR()
        {// TODO: Add Testing logic here            
            var cmd = new SampleCommand() { Id = SampleCommand.CmdType.Add, Content = "******" };
            var mediator = serviceProvider.GetService<IMediator>();
            mediator.Send(cmd);
            mediator.Publish(cmd);
        }
        [UMethod]
        public void T_Config()
        {// TODO: Add Testing logic here
            //https://www.jerriepelser.com/tutorials/airport-explorer/basic/working-with-configuration/
            //https://dotnettutorials.net/lesson/asp-net-core-appsettings-json-file/
            //https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/environments?view=aspnetcore-3.1
            var cfg = serviceProvider.GetService<IConfiguration>();
            print(cfg["EventBusConnection"]);
            print(cfg.GetValue<bool>("AzureServiceBusEnabled"));
            var x=cfg.GetSection("ClientInfo");
            var v=x.Get<ClientInfo>();
            //serviceCollection.Configure<ClientInfo>(i=>i.Name="aaa");
            serviceCollection.Configure<ClientInfo>(cfg.GetSection("ClientInfo"));

            var sp = serviceCollection.BuildServiceProvider();
            //serviceCollection.Configure<ClientInfo>(cfg.GetSection("ClientInfo"));
            var ci=sp.GetService<IOptions<ClientInfo>>().Value;
            //var clientInfo = serviceCollection.Configure<Clientinfo>(cfg);

            print($"{ci.Name}  {ci.ClientId}  {ci.ClientSecret}");
        }
    }
}
