////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/8/2011 10:01:13 AM 
// Description: IConverterField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Common.DataContract;

namespace Common.DataCore
{
    public interface IConverterField:IEntityField
    {        
        IEntityTableSource SourceProvider { get; set; }
        event Action<IConverterField,string> ContentUpdatedEvent;
        void Initializing();
    }
}
