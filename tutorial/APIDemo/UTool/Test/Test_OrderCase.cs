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

using System;

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
        private ApiProxy<TService> CreateProxy<TService>()
            where TService : class
        {
            var proxy = new ApiProxy<TService>(host);
            proxy.ApiVersion = "1";
            proxy.RegisterChtSwaggerDoc(useApiGateway: true);
            return proxy;
        }
        [UMethod]
        public void T_A1_CreateAccount()
        {// TODO: 開戶
            var proxy = CreateProxy<IDepositService>();
            currentCustomerAccount = proxy.Svc.CreateBankAccount("R122088167");
            print($"Create Bank Account Id={currentCustomerAccount.Id} $={currentCustomerAccount.AccountBalance}");
        }
        [UMethod]
        public void T_A2_NewOrder()
        {// TODO: 購買商品(轉帳)
            if (currentCustomerAccount == null) T_A1_CreateAccount();
            var prodId = "IBM NB099";
            int quantity = 3;
            var payMethod = OrderDetail.PayMethodMode.Bank;//銀行轉帳
            currentOrder = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = currentCustomerAccount.Id,
                Detail = new OrderDetail() { ProductId = prodId, Quantity = quantity }
            };
            currentOrder.Detail.PayMethod = (payMethod == 0) ? OrderDetail.PayMethodMode.Bank : OrderDetail.PayMethodMode.Wire;
            if (currentOrder.Detail.PayMethod == OrderDetail.PayMethodMode.Bank)
                currentOrder.Detail.PaymentAccout = currentCustomerAccount.Id;
            var proxy = CreateProxy<IOrderingService>();
            proxy.Svc.IssueOrder(currentOrder);
            print("已送出訂單!");
        }
        [UMethod]
        public Order T_A3_QueryOrder()
        {// TODO: 定單查詢
            if (currentOrder == null)
            {
                print($"未曾下單");
                return null;
            }
            var proxy = CreateProxy<IOrderingService>();
            var order=proxy.Svc.QueryOrder(currentOrder.Id);
            if(order==null) print($"查無訂單");
            else
                print($"Status={order.Status} {order.Comment}");
            return order;
        }
        [UMethod]
        public void T_A4_Deposit()
        {// TODO:存款
            decimal amount = 5000;
            var proxy = CreateProxy<IDepositService>();
            currentCustomerAccount=proxy.Svc.Deposit(currentCustomerAccount.Id,amount);
            print($"Id={currentCustomerAccount.Id} $={currentCustomerAccount.AccountBalance}");
        }
        [UMethod]
        public void T_A5_NewOrder()
        {// TODO: 購買商品(轉帳)
            T_A2_NewOrder();
        }
        [UMethod]
        public void T_A6_QueryOrder()
        {// TODO: 定單查詢
            T_A3_QueryOrder();
        }
        [UMethod]
        public void T_B1_NewOrder()
        {// TODO: 購買商品(電匯)
            if (currentCustomerAccount == null) T_A1_CreateAccount();
            var prodId = "Acer NB06";
            int quantity = 10;
            var payMethod = OrderDetail.PayMethodMode.Wire;//銀行轉帳
            
            currentOrder = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = currentCustomerAccount.Id,
                Detail = new OrderDetail() { ProductId = prodId, Quantity = quantity }
            };
            currentOrder.Detail.PayMethod = (payMethod == 0) ? OrderDetail.PayMethodMode.Bank : OrderDetail.PayMethodMode.Wire;
            if (currentOrder.Detail.PayMethod == OrderDetail.PayMethodMode.Bank)
                currentOrder.Detail.PaymentAccout = currentCustomerAccount.Id;
            var proxy = CreateProxy<IOrderingService>();
            proxy.Svc.IssueOrder(currentOrder);
            print("已送出訂單!");
        }
        [UMethod]
        public void T_B2_QueryOrder()
        {// TODO: 定單查詢
            T_A3_QueryOrder();
        }
        [UMethod]
        public void T_B3_WireDeposit()
        {// TODO: 電匯
            print("查詢訂單:");
            Order order= T_A3_QueryOrder();
            var proxy = CreateProxy<IDepositService>();
            bool rlt=proxy.Svc.WireDepositForPayment(order.Detail.PaymentAccout,order.PaymentDetailRecord);
            print($"電匯結果={rlt}");
        }
        [UMethod]
        public void T_B4_QueryOrder2()
        {// TODO: 定單查詢
            T_A3_QueryOrder();
        }
        private static BankAccount currentCustomerAccount;
        private static Order currentOrder;
        private readonly Uri host = new Uri("http://localhost:88");
    }
}
