////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/22/2020 1:43:26 PM 
// Description: IQuServiceProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contract
{
    public interface IQuProxy<TService> :IDisposable//, IApiProxy<TService>
         where TService : class
    {
        TService Svc { get; }
        T WaitResult<T>(QuResult<T> rltStamp);
        T WaitResult<T>(QuResult<T> rltStamp, TimeSpan timeOut);
        Task<T> AsyncWaitResult<T>(QuResult<T> rltStamp);
        Task<T> AsyncWaitResult<T>(QuResult<T> rltStamp, TimeSpan timeOut);        
    }
}
