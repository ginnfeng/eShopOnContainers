////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/19/2020 5:02:22 PM 
// Description: Test_OrderCase.cs  
// Revisions  :            		
// **************************************************************************** 
using ApiGw.ClientProxy;
using ApiGw.ClientProxy.Ext;
using Service.Banking.Contract.Service;
using Service.Ordering.Contract.Service;
using Sid.Bss.Banking;
using Sid.Bss.Ordering;
using Support.Open.Google;
using System;
using System.Collections.Generic;
using System.Text;
using UTDll;
namespace UTool.Test
{
    class Test_OrderCase : UTest
    {
        public Test_OrderCase()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        private ClientProxy<TService> CreateProxy<TService>()
            where TService : class
        {
            var proxy = new ClientProxy<TService>(host);
            proxy.ApiVersion = "1";
            proxy.RegisterChtSwaggerDoc(useApiGateway: true);
            return proxy;
        }
        [UMethod]
        public void T_A1_CreateAccount()
        {// TODO: Add Testing logic here
            var proxy = CreateProxy<IDepositService>();
            currentCustomerAccount = proxy.Svc.CreateBankAccount("R122088167");
            print($"BankAccount.Id={currentCustomerAccount.Id}");
        }
        [UMethod]
        public void T_A1_NewOrder(string prodId,int quantity,int payMethod)
        {// TODO: Add Testing logic here
            
            currentOrderId = Guid.NewGuid().ToString();
            var order = new Order()
            {
                Id= currentOrderId,
                Detail=new OrderDetail(){ProductId= prodId ,Quantity= quantity }
            };
            order.Detail.PayMethod = (payMethod ==0) ? OrderDetail.PayMethodMode.Bank : OrderDetail.PayMethodMode.Wire;
            if (order.Detail.PayMethod == OrderDetail.PayMethodMode.Bank)
                order.Detail.PaymentAccout = currentCustomerAccount.Id;
            var proxy = CreateProxy<IOrderingService>();
            proxy.Svc.IssueOrder(order);
        }
        [UMethod]
        public void T_A1_QueryOrder()
        {// TODO: Add Testing logic here
            var proxy = CreateProxy<IOrderingService>();
            var order=proxy.Svc.QueryOrder(currentOrderId);
            print($"Status={order.Status} {order.Comment}");
        }
        [UMethod]
        public void T_CaseA3()
        {// TODO: Add Testing logic here

        }
        [UMethod]
        public void T_CaseA4()
        {// TODO: Add Testing logic here

        }
        private static BankAccount currentCustomerAccount;
        private static string currentOrderId;
        private readonly Uri host = new Uri("http://localhost:88");
    }
}
