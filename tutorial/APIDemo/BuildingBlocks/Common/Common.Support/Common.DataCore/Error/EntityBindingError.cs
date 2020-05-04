////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/8/2009 4:48:01 PM 
// Description: DataTableBindingError.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Support.ErrorHandling;

namespace Common.DataCore.Error
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class EntityBindingError : ErrorInfoBase
    {
        public EntityBindingError()
        {
        }
    }
}
