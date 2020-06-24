////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 5:15:43 PM 
// Description: OrderingService.cs  
// Revisions  :            		
// **************************************************************************** 

using EventBus.Domain;
using EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Service.Banking.Contract.Service;
using Service.Ordering.Application.Data.Context;
using Service.Ordering.Contract.Command;
using Service.Ordering.Contract.Service;
using Sid.Bss.Banking;
using Sid.Bss.Ordering;
using System;

namespace Service.Ordering.ApiImp
{
    public class OrderingService : IOrderingService
    {
        private string storePaymentAccount = "B0001";
        private IConnectionFactory connFactory;
        private ILoggerFactory loggerFactory;
        private ILogger TheLogger { get; }
        public OrderingService(IConnectionFactory connFactory, ILoggerFactory loggerFactory)
        {
            this.connFactory = connFactory;
            this.loggerFactory = loggerFactory;
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuListener>();
        }
        
        async public void IssueOrder(Order order)
        {
            var paymentDetail = new PaymentDetail() 
            { 
                Id=Guid.NewGuid().ToString()
                ,TaxRate=5
                ,Amount=order.Detail.Quantity*1000
            };
            order.PaymentDetailRecord = paymentDetail;
            order.Status = Order.OrderStatus.Create;
            order.Comment = "訂單成立!";
            OrderContext.Instance.Insert(order);
            //using (var mqProxy = new QuProxy<IPaymentService>("service.rabbitmq"))//host暫時
            using (var mqProxy = new QuProxy<IPaymentService>(connFactory, loggerFactory))
            {
                switch (order.Detail.PayMethod)
                {
                    case OrderDetail.PayMethodMode.Bank:
                        var quRlt = mqProxy.Svc.BankTransfers(order.Detail.PaymentAccout, storePaymentAccount, paymentDetail);                        
                        var transferRecord=await mqProxy.AsyncWaitResult(quRlt);
                        if (transferRecord.Succes)
                        {
                            order.Status = Order.OrderStatus.Paid;
                            order.Comment = "已付款，進行備貨中!";
                        }
                        else
                        {
                            order.Status = Order.OrderStatus.Trouble;
                            order.Comment = transferRecord.Info;
                        }
                        break;
                    case OrderDetail.PayMethodMode.Wire:
                        mqProxy.Svc.WireTransfer(storePaymentAccount, paymentDetail);
                        order.Status = Order.OrderStatus.Paying;
                        order.Detail.PaymentAccout = storePaymentAccount;
                        order.Comment = "等待付款!";                        
                        break;                    
                }                
            }
            OrderContext.Instance.Update(order);
        }

        public Order QueryOrder(string orderId)
        {
            Order order;
            if (OrderContext.Instance.TryGetValue(orderId, out order))
                return order;
            return null;                
        }
        //public OrderingService(IEventBus bus)
        //{
        //    this.bus = bus;
        //}
        //public void IssueOrder(Order order)
        //{
        //    bus.SendCmd(new IssueOrderCmd(order));
        //}
        //private readonly IEventBus bus;
        //public Order QueryOrder(string orderId)
        //{
        //    var task= bus.SendCmd(new QueryOrderCmd(orderId));            
        //    return task.Result;
        //}

    }
}
