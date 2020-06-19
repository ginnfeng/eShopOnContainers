////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/17/2020 10:12:41 AM 
// Description: PaymentService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using EventBus.RabbitMQ;
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
       
        public QuResult<TransferRecord> BankTransfers(string from, string to, PaymentDetail detail)
        {
            var rd = new TransferRecord()
            {
                At = DateTime.Now,
                Detail = detail,
                Succes = false
            };
            BankAccount fromAccount;
            if (!AccountContext.Instance.TryGetValue(to, out fromAccount))
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
            fromAccount.AccountBalance-= detail.Amount;
            toAccount.AccountBalance += detail.Amount;
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
            waitingWirePayments ??= new Dictionary<string, PaymentDetail>();            
            
            BankAccount toAccount;
            if (!AccountContext.Instance.TryGetValue(account, out toAccount))
                return false;
            toAccount.AccountBalance += detail.Amount;
            
            lock (waitingWirePayments)
            {
                PaymentDetail toPaymentDetail;
                if (waitingWirePayments.TryGetValue(detail.Id, out toPaymentDetail))
                {
                    if (detail.Amount != toPaymentDetail.Amount)
                        return false;
                    var rd = new TransferRecord()
                    {
                        At = DateTime.Now,
                        Detail = toPaymentDetail,
                        Succes = true
                    };
                    using (var mqProxy = new QuCleintProxy<IPaymentCallbackService>("localhost"))//host暫時
                    {
                        mqProxy.Svc.WireTransferCommit(rd);
                    }                        
                }
            }
            return true;
        }

        public BankAccount CreateBankAccount(string cid)
        {
            accountIdx++;
            return new BankAccount()
            {
                Id = $"A(accountIdx)"
            };
        }
        private static int accountIdx=1000; 
        private static Dictionary<string, PaymentDetail> waitingWirePayments;
    }
}
