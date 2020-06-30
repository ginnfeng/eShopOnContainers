////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/10/2020 5:22:23 PM 
// Description: Test_EventBus.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using IoC.DI;
using Microsoft.Extensions.DependencyInjection;
using Service.Banking.Application.EventHandler;
using Service.Banking.Contract.Event;

using Service.Ordering.Contract.Service;
using Sid.Bss.Ordering;
using System;

using UTDll;
namespace UTool.Test
{
    class Test_IoC : UTest
    {
        private IServiceProvider serviceProvider;
        public Test_IoC()
        {
            //
            // TODO: Add constructor logic here
            //     
            Init();
        }
        private void Init()
        {
            var serviceCollection = new ServiceCollection();
            MyDIContainer.Instance.ResgisterServices(serviceCollection,null);
            SP=serviceCollection.BuildServiceProvider();
        }
        public IServiceProvider SP { get; private set; }
        static long orderIdNum = 1000;
        [UMethod]
        public void T_IssueOrder(string customerId, string adr)
        {// TODO: Add Testing logic here
            var order = new Order() {Id= $"{orderIdNum++}",CustomerId= customerId, ShipAddress=adr };
            var svc=SP.GetService<IOrderingService>();
            svc.IssueOrder(order);
        }
        
        [UMethod]
        public void T_QueryOrder(string orderId)
        {// TODO: Add Testing logic here
            var svc = SP.GetService<IOrderingService>();
            var order = svc.QueryOrder(orderId);
            print($"CustId={order.CustomerId} ShipAddres={order.ShipAddress}");
        }

        [UMethod]
        public void T_IssueOrderEventHandler()
        {// TODO: Add Testing logic here
            var bus = SP.GetService<IEventBus>();
            bus.SubscribeEvent<IssueOrderEvent,IssueOrderEventHandler>();
        }

    }
}
