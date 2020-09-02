////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/26/2009 4:39:16 PM 
// Description: IEntityAccess.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataCore
{
    public interface IEntityAccess
    {
        object this[string propertyKey] { get; set; }
        void SetPropertyValue(string name, object value);
        object GetPropertyValue(string name);
        //object InvokeMethod(string methodName, params object[] args);        
    }
}
