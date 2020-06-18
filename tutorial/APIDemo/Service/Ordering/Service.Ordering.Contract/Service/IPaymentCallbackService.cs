////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/18/2020 9:45:49 AM 
// Description: IPaymentCallbackService.cs  
// Revisions  :            		
// **************************************************************************** 
using Sid.Bss.Banking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Servic
{
    public interface IPaymentCallbackService
    {
        void WireTransfer(PaymentDetail detail);
    }
}
