////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/16/2011 10:07:51 AM 
// Description: IEntitySchema.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataCore
{
    public interface IEntitySchema
    {
        [EntityProperty(IsKey = true, KeyGen = "Col_{0}")]
        string ColumnName { get; set; }
        bool Unique { get; set; }
        string DataType { get; set; }
        string KeyGen { get; set; }       
 
        string FieldType { get; set; }
        string AliasType { get; set; }

        string DefaultValue { get; set; }
        
    }
}
