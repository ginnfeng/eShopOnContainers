////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/17/2011 3:46:27 PM 
// Description: ISpecBase.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataContract
{
    public interface ISpecBase
    {
        string FieldName { get; set; }
        string Type { get; set; }
        bool IsKey { get; set; }
        string KeyGen { get; set; }
    }
}
