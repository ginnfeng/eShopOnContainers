////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 5:47:24 PM 
// Description: ISwaggerDocStore.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApiGw.ClientProxy
{
    public interface ISwaggerDocStore
    {
        bool TrySpec<TServiceInterface>(out HttpMethodSpec sepc);
        void RegisterSwaggerDoc(Uri endpoint);
    }
}
