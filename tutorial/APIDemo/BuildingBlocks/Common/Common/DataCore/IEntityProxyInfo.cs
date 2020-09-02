////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/9/2011 2:55:19 PM 
// Description: IEntityProxyInfo.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataCore
{
    public interface IEntityProxyInfo //: IEntityTableInfo
    {
        IEntityRowProxy Proxy { get; }
    }
}
