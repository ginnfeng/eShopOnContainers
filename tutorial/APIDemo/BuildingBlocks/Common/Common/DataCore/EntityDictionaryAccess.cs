////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/18/2009 5:23:25 PM 
// Description: EntityDictionaryAccess.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
namespace Common.DataCore
{
    public class EntityDictionaryAccess<TEntity> : EntityAccessBase<TEntity>
    {
        public EntityDictionaryAccess()
            :this(new Dictionary<string, object>())
        {
        }
        public EntityDictionaryAccess(Dictionary<string, object> valueMap)
        {
            ValueMap = valueMap;
        }     
        
        //private Dictionary<string, object> valueMap = new Dictionary<string, object>();

        internal override void DoSetProperty(System.Reflection.MethodInfo methodInfo, string propertyName, object value)
        {
            ValueMap[propertyName] = value;
        }

        internal override object DoGetProperty(System.Reflection.MethodInfo methodInfo, string propertyName)
        {
            object value;
            ValueMap.TryGetValue(propertyName, out value);
            return value;
        }
        public Dictionary<string, object> ValueMap { get; set; }
    }
}
