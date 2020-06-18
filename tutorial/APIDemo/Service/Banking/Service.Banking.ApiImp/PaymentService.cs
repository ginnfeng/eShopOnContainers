////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/17/2020 10:12:41 AM 
// Description: PaymentService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Service.Banking.Application.Data.Context;

using Service.Banking.Contract.Service;
using Sid.Bss.Banking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.ApiImp
{
    internal class PaymentService : IPaymentService
    {
       
        public QuResult<TransferRecord> BankTransfers(string from, string to, PaymentDetail detail)
        {
            var rd = new TransferRecord()
            {
                At = DateTime.Now,
                Detail = detail,
                Succes = true
            };
            return new QuResult<TransferRecord>(rd);
        }

        public QuResult<TransferRecord> CardTransfer(string from, string to, PaymentDetail detail)
        {
            throw new NotImplementedException();
        }

        
        public void WireTransfer(string to, PaymentDetail detail)
        {
            throw new NotImplementedException();
        }

        public bool WireDeposit(string account, PaymentDetail detail)
        {
            waitingWirePayments ??= new Dictionary<string, PaymentDetail>();
            
            lock (AccountContext.Instance)
            {
                BankAccount toAccount;
                if (!AccountContext.Instance.TryGetValue(account, out toAccount))
                    return false;
                toAccount.AccountBalance += detail.Amount;
            }

            lock (waitingWirePayments)
            {
                PaymentDetail toPaymentDetail;
                if (waitingWirePayments.TryGetValue(detail.Id, out toPaymentDetail))
                {
                    var rd = new TransferRecord()
                    {
                        At = DateTime.Now,
                        Detail = toPaymentDetail,
                        Succes = true
                    };
                }
            }
            return true;
        }

        
        private static Dictionary<string, PaymentDetail> waitingWirePayments;
    }
}
