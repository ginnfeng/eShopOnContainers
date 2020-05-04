////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/7/2011 11:02:36 AM 
// Description: IEntityTableAttributes.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;

namespace Common.DataCore
{
    public interface IEntityTableAttributes
    {
        bool IsPartial { get; set; }
        long KeyCount { get; set; }
        OPStatus Status { get; set; }
        OPStatus PreStatus { get; set; }

        //異動前的SchemaTable,用JSon描述,此為過渡作法
        string OrignalTableSchema { get; set; }
    }
}
