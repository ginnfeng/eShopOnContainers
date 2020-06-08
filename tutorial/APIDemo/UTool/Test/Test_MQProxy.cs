////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 4:41:28 PM 
// Description: Test_MQProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.RabbitMQ;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Servic;
using System;
using System.Collections.Generic;
using System.Text;
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
            var svcHandler = new QuServiceHandler();
            var holderSvc = new HelloWorldService();
            svcHandler.Subscribe<IHelloWorldService>(holderSvc);
        }
        [UMethod]
        public void T_Publish(string id1)
        {
            using (var mqProxy = new QuCleintProxy<IHelloWorldService>())
            {
                var id2 = new HelloInput() { UserName = "Lee", Date = DateTime.Today };
                mqProxy.Svc.OneWayCall(id1, id2);
            }
        }
    }
}
