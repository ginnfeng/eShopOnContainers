////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/18/2020 9:45:49 AM 
// Description: IPaymentCallbackService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Sid.Bss.Banking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Servic
{
    [ApiSpec(typeof(IPaymentCallbackService), RouteTemplate.API_VER_SVC)]
    public interface IPaymentCallbackService
    {
        void WireTransferCommit(TransferRecord detail);
    }
}
