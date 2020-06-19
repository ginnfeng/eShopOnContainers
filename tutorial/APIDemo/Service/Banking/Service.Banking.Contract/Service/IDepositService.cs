////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/19/2020 4:15:54 PM 
// Description: IDepositService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Sid.Bss.Banking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Contract.Service
{
    [ApiSpec(typeof(IDepositService), RouteTemplate.API_VER_SVC)]
    public interface IDepositService
    {
        bool WireDepositForPayment(string accountId, PaymentDetail detail);//電存
        BankAccount CreateBankAccount(string cid);
    }
}
