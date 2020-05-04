////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/17/2011 3:10:22 PM 
// Description: IField.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataContract
{
    public interface IEntityField
    {
        EntityColumn Column { get; set; }
        EntityRow Row { get; set; }
        string Content { get; set; }
       
    }
    public interface IDefaultEntityField : IEntityField
    {
        IEntityField Clone();
        IEntityField Clone(EntityRow owner);
    }
}
