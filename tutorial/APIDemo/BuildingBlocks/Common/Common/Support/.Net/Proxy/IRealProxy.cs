////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/3/2011 11:08:43 AM 
// Description: IRealProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Reflection;

namespace Common.Support.Net.Proxy
{
    public interface IRealProxy
    {
        object InvokeMethod(MethodInfo methodInfo, ref object[] args);
    }
}
