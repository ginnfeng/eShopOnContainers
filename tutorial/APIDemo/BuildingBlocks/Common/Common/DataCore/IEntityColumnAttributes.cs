////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/7/2011 11:03:01 AM 
// Description: IEntityColumnAttributes.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;

namespace Common.DataCore
{
    public interface IEntityColumnAttributes
    {
        EntityFieldType FieldType { get; set; }
        string KeyGen { get; set; }
        OPStatus Status { get; set; }
        string AliasType { get; set; }
    }   
}
