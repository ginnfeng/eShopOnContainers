////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/17/2020 10:12:41 AM 
// Description: PaymentService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using EventBus.RabbitMQ;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Service.Banking.Application.Data.Context;

using Service.Banking.Contract.Service;
using Service.Ordering.Contract.Servic;
using Sid.Bss.Banking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.ApiImp
{
    public class PaymentService : IPaymentService, IDepositService
    {
        private IConnectionFactory connFactory;
        private ILoggerFactory loggerFactory;
        private ILogger TheLogger { get; }
        public PaymentService(IConnectionFactory connFactory, ILoggerFactory loggerFactory)
        {
            this.connFactory = connFactory;
            this.loggerFactory = loggerFactory;
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuListener>();
        }
        public QuResult<TransferRecord> BankTransfers(string from, string to, PaymentDetail detail)
        {
            var rd = new TransferRecord()
            {
                At = DateTime.Now,
                Detail = detail,
                Succes = false
            };
            BankAccount fromAccount;
            if (!AccountContext.Instance.TryGetValue(from, out fromAccount))
            {
                rd.Info = "存款帳戶不存在";
                return new QuResult<TransferRecord>(rd);
            }
            if(fromAccount.AccountBalance<detail.Amount)
            {
                rd.Info = "存款不足";
                return new QuResult<TransferRecord>(rd);
            }
            BankAccount toAccount;
            if (!AccountContext.Instance.TryGetValue(to, out toAccount))
            {
                toAccount = new BankAccount() { Id = to };
                AccountContext.Instance.Insert(toAccount);
            }
            fromAccount.AccountBalance= fromAccount.AccountBalance-detail.Amount;
            toAccount.AccountBalance = toAccount.AccountBalance+detail.Amount;
            rd.Succes = true;
            return new QuResult<TransferRecord>(rd);
        }

        public QuResult<TransferRecord> CardTransfer(string from, string to, PaymentDetail detail)
        {
            throw new NotImplementedException();
        }


        public void WireTransfer(string to, PaymentDetail detail)
        {
            BankAccount toAccount;
            if (!AccountContext.Instance.TryGetValue(to, out toAccount))
            {
                toAccount = new BankAccount() { Id = to };
                AccountContext.Instance.Insert(toAccount);
            }
            
            lock (waitingWirePayments)
            {
                waitingWirePayments[detail.Id] = detail;
            }
        }
        public bool WireDepositForPayment(string account, PaymentDetail detail)
        {
           
            BankAccount toAccount;
            if (!AccountContext.Instance.TryGetValue(account, out toAccount))
                return false;
            toAccount.AccountBalance += detail.Amount;
            
            lock (waitingWirePayments)
            {
                PaymentDetail toPaymentDetail;
                if (!waitingWirePayments.TryGetValue(detail.Id, out toPaymentDetail))
                    return false;
                if (detail.Amount != toPaymentDetail.Amount)
                    return false;
                var rd = new TransferRecord()
                {
                    At = DateTime.Now,
                    Detail = toPaymentDetail,
                    Succes = true
                };
                //using (var mqProxy = new QuProxy<IPaymentCallbackService>("service.rabbitmq"))//host暫時
                using (var mqProxy = new QuProxy<IPaymentCallbackService>(connFactory,loggerFactory))//host暫時
                {
                    mqProxy.Svc.WireTransferCommit(rd);
                }
                waitingWirePayments.Remove(detail.Id);
            }
            return true;
        }

        public BankAccount CreateBankAccount(string cid)
        {
            accountIdx++;
            var account= new BankAccount(){Id = $"A{accountIdx}"};
            AccountContext.Instance.Insert(account);
            return account;
        }

        public BankAccount Deposit(string accountId, decimal amount)
        {
            BankAccount account;
            if (!AccountContext.Instance.TryGetValue(accountId, out account))
            {
                throw new Exception("存款帳戶不存在");                
            }
            account.AccountBalance += amount;
            return account;
        }

        private static int accountIdx=1000; 
        private static Dictionary<string, PaymentDetail> waitingWirePayments = new Dictionary<string, PaymentDetail>();
    }
}
