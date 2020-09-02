////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/11/2008 7:08:53 PM 
// Description: IDataOwner.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.Support.DataTransaction
{
    public interface IDataOwner
    {
        bool IsRequired { get; set; }
        bool Validate(object value);
        string Description { get; set; }
    }
}
