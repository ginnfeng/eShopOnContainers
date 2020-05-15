////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/15/2020 10:47:16 AM 
// Description: IHttpSpecFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApiGw.ClientProxy
{
    public interface IHttpSpecFactory
    {
        bool TryGetHttpMethodSpec(MethodInfo methodInfo, out HttpMethodSpec sepc);

        void RegisterSwaggerDoc(Uri endpoint);
    }
}
