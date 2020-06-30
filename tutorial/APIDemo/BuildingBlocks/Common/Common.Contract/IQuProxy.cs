﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/22/2020 1:43:26 PM 
// Description: IQuServiceProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Contract
{
    public interface IQuProxy<TService> :IDisposable, IApiProxy<TService>
         where TService : class
    {
    }
}
