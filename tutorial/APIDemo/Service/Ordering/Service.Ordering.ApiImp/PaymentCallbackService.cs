////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/19/2020 2:28:09 PM 
// Description: PaymentCallbackService.cs  
// Revisions  :            		
// **************************************************************************** 
using Service.Ordering.Application.Data.Context;
using Service.Ordering.Contract.Servic;
using Sid.Bss.Banking;
using Sid.Bss.Ordering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.ApiImp
{
    public class PaymentCallbackService : IPaymentCallbackService
    {
        public void WireTransferCommit(TransferRecord detail)
        {
            OrderContext.Instance.FindAndDo(
                order=> {
                    if(order.PaymentDetailRecord.Id==detail.Detail.Id)
                    {
                        order.Status = Order.OrderStatus.Paid;
                        order.Comment = "已收到匯款，進行備貨中!";
                        return true;
                    }
                    return false;
                    }
                );
        }

    }
}
