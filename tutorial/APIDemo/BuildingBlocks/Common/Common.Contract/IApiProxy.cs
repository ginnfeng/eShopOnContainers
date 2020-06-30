////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 4:37:58 PM 
// Description: IServiceProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Contract
{
    public interface IApiProxy<TService>
        where TService:class
    {
        TService Svc { get; }
    }
}
