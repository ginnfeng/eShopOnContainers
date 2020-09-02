////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/1/2011 10:14:51 AM 
// Description: IDictionaryAccess.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;

namespace Common.DataCore
{
    public interface IDictionaryAccess
    {
        object this[string propertyKey] { get; set; }
        List<string> Keys { get; }
    }
}
