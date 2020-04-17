////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/10/2020 5:22:23 PM 
// Description: Test_EventBus.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.DependencyInjection;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Service;
using System;
using System.Collections.Generic;
using System.Text;
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
            DIContainer.ResgisterServices(serviceCollection);
            SP=serviceCollection.BuildServiceProvider();
        }
        public IServiceProvider SP { get; private set; }
        [UMethod]
        public void T_IssueOrder(string orderId,string adr)
        {// TODO: Add Testing logic here
            var order = new Order() {OrderId= orderId, ShipAddress=adr };
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
    }
}
