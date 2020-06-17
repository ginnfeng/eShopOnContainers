﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/16/2020 2:38:46 PM 
// Description: IPaymentService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Service.Banking.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Contract.Service
{
    [ApiSpec(typeof(IPaymentService), RouteTemplate.API_VER_SVC)]
    public interface IPaymentService
    {
        QuResult<TransferRecord> BankTransfers(string fromId, string toId,PaymentDetail detail);//轉帳

        QuResult<TransferRecord> CardTransfer(string fromId, string toId, PaymentDetail detail);//信用卡支付
        void WireTransfer(string toId, PaymentDetail detail);//電匯
        bool WireDeposit(string accountId, PaymentDetail detail);//電存
    }
}
