////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/10/2020 5:22:23 PM 
// Description: Test_EventBus.cs  
// Revisions  :            		
// **************************************************************************** 
using ApiGw.ClientProxy;
using ApiGw.ClientProxy.Ext;
using Common.Contract;
using EventBus.Domain;
using EventBus.RabbitMQ;
using IoC.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Service.Banking.Application.EventHandler;
using Service.Banking.Contract.Event;
using Service.HelloWorld.ApiImp;
using Service.HelloWorld.Contract.Servic;
using Service.Ordering.Contract.Service;
using Sid.Bss.Ordering;
using System;
using System.IO;
using UTDll;
namespace UTool.Test
{
    class Test_IoC : UTest
    {
        
        public Test_IoC()
        {
            //
            // TODO: Add constructor logic here
            //     
            
        }
        static internal IServiceProvider InitSP()
        {
            var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetFileProvider(new PhysicalFileProvider(basePath))
                //.AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true);
            var cfg= builder.Build();
            var serviceCollection = new ServiceCollection();
            UtoolDIContainer.Instance.ResgisterServices(serviceCollection, cfg);
            return serviceCollection.BuildServiceProvider();
        }
        static private IServiceProvider serviceProvider;
        public IServiceProvider SP => serviceProvider??= InitSP();



        //static IApiProxy<IHelloWorldService> proxy;
        [UMethod]
        public void T_ClientProxyByGW()
        {
            var proxy = SP.GetRequiredService<IApiProxy<IHelloWorldService>>();
            proxy.ApiVersion = "1";
            proxy.RegisterChtSwaggerDoc(useApiGateway: true);
            IHelloWorldService helloSvc = proxy.Svc;
            string id1 = "*abc*";
            int id2 = 99;
            DateTime id3 = DateTime.Today;

            var postrlt = helloSvc.HelloPost("CCC", "DDD");
            print($"API={nameof(IHelloWorldService.HelloPost)} result={postrlt}");

            var getrlt = proxy.Svc.HelloGet("EEE", "FFF");
            print($"API={nameof(IHelloWorldService.HelloGet)} result={getrlt}");
        }

        [UMethod]
        public void T_StartSubscriber()
        {// TODO: Add Testing logic here
            var svcHandler = SP.GetService<QuListener>();
            var holderSvc = new HelloWorldService();
            svcHandler.Subscribe<IHelloWorldService>(holderSvc);
            svcHandler.Subscribe<IHelloQuService>(holderSvc);
        }
        
        [UMethod]
        public void T_PublishTwoWay(string id1)
        {// TODO: Add Testing logic here
            using (var mqProxy = SP.GetService<IQuProxy<IHelloQuService>>())
            {
                var quRlt = mqProxy.Svc.TwoWayCall(id1);
                var obj = mqProxy.WaitResult(quRlt);
                print(obj.Summary);
            }
        }
        //[UMethod]
        //public void T_IssueOrderEventHandler()
        //{// TODO: Add Testing logic here
        //    var bus = SP.GetService<IEventBus>();
        //    bus.SubscribeEvent<IssueOrderEvent,IssueOrderEventHandler>();
        //}
        //static long orderIdNum = 1000;
        //[UMethod]
        //public void T_IssueOrder(string customerId, string adr)
        //{// TODO: Add Testing logic here
        //    var order = new Order() { Id = $"{orderIdNum++}", CustomerId = customerId, ShipAddress = adr };
        //    var svc = SP.GetService<IOrderingService>();
        //    svc.IssueOrder(order);
        //}

        //[UMethod]
        //public void T_QueryOrder(string orderId)
        //{// TODO: Add Testing logic here
        //    var svc = SP.GetService<IOrderingService>();
        //    var order = svc.QueryOrder(orderId);
        //    print($"CustId={order.CustomerId} ShipAddres={order.ShipAddress}");
        //}
    }
}
