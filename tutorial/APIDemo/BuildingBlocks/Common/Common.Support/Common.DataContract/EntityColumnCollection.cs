////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/8/2011 11:09:18 AM 
// Description: EntityColumnCollection.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    //[DataContract]
    public class EntityColumnCollection : List<EntityColumn>
    {
        public EntityColumnCollection()
        {             
        }
        
        public EntityColumn this[string name] 
        {
            get 
            {
                var idx = this.FindIndex(name);
                return (idx<0)?null:this[idx];
            } 
        }
        public int FindIndex(string columnName)
        {
            return this.FindIndex(col => col.ColumnName.Equals(columnName));
        }
       
    }
}
