////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/15/2011 5:44:35 PM 
// Description: IEntityTableInfo.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataContract
{
    public interface IEntityTableInfo
    {  
        string Namespace { get; set; }        
        string TableName { get; set; }
    }
    
}
