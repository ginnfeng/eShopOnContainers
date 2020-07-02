////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 4:41:28 PM 
// Description: Test_MQProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Service.HelloWorld.ApiImp;
using Service.HelloWorld.Contract.Entity;
using Service.HelloWorld.Contract.Servic;
using System;
using System.Diagnostics;
using UTDll;
namespace UTool.Test
{
    class Test_MQProxy : UTest
    {
        public Test_MQProxy()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        [UMethod]
        public void T_StartSubscriber()
        {// TODO: Add Testing logic here
            var svcHandler = new QuListener("localhost");
            var holderSvc = new HelloWorldService();
            svcHandler.Subscribe<IHelloWorldService>(holderSvc);
            svcHandler.Subscribe<IHelloQuService>(holderSvc);
        }
        [UMethod]
        public void T_Publish(string id1)
        {
            using (var mqProxy = new QuProxy<IHelloWorldService>("localhost"))
            {
                var id4 = new HelloInput() { UserName = "Lee", Date = DateTime.Today };
                
                DateTime id3 = DateTime.Today;
                mqProxy.Svc.OneWayCall(id1, id4);

                
            }
        }
        [UMethod]
        public void T_Publish2(string id1)
        {
            using (var mqProxy = new QuProxy<IHelloWorldService>("localhost"))
            {
                var id4 = new HelloInput() { UserName = "Lee", Date = DateTime.Today };
                long id2 = 99;
                DateTime id3 = DateTime.Today;
                
                var rltStr = mqProxy.Svc.HelloGet(id1, "HI");
                print(rltStr);

                var rlt = mqProxy.Svc.Hello(id1, id2, id3, id4);
                print($"API={nameof(IHelloWorldService.Hello)} User={rlt.UserName}\nDate={rlt.Date}\n{rlt.Summary}");
            }
        }

        [UMethod]
        public void T_PublishTwoWay(string id1)
        {// TODO: Add Testing logic here
            
            using (var mqProxy = new QuProxy<IHelloQuService>("localhost"))
            {                
                var quRlt=mqProxy.Svc.TwoWayCall(id1);
                var obj=mqProxy.WaitResult(quRlt);                
                print(obj.Summary);
            }
        }
        [UMethod]
        async public void T_PublishTwoWayAsync(string id1)
        {// TODO: Add Testing logic here

            using (var mqProxy = new QuProxy<IHelloQuService>("localhost"))
            {
                var quRlt = mqProxy.Svc.TwoWayCall(id1);
                var obj = await mqProxy.AsyncWaitResult(quRlt);
                Debug.WriteLine(obj.Summary);
            }
        }

        static private IServiceProvider serviceProvider;
        public IServiceProvider SP => serviceProvider ??= Test_IoC.InitSP();
        [UMethod]
        public void T_PublishTwoWay2(string id1)
        {// TODO: Add Testing logic here
            using (var mqProxy = SP.GetService<IQuProxy<IHelloQuService>>())
            {
                var quRlt = mqProxy.Svc.TwoWayCall(id1);                
                var obj = mqProxy.WaitResult(quRlt);
                print(obj.Summary);
            }
        }

    }
}
