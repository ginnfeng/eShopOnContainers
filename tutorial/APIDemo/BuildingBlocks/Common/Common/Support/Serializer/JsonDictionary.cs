////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/30/2011 3:53:29 PM 
// Description: JsonDictionary.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.Support.Serializer
{
    [Serializable]
    public class JsonDictionary<TValue> :ISerializable
    {
        public JsonDictionary()
            :this(new Dictionary<string, TValue>())
        {           
        }
        public JsonDictionary(Dictionary<string, TValue> dic)
        {
            Dictionary = dic;
        }
        protected  JsonDictionary(SerializationInfo info, StreamingContext context) 
            :this()
        {            
            foreach (var entry in info)
            {
                Dictionary.Add(entry.Name, (TValue)entry.Value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (string key in Dictionary.Keys)
            {
                var value = Dictionary[key];
                info.AddValue(key, value, value == null ? typeof(object) : value.GetType());
            }
        }
        
        public Dictionary<string, TValue> Dictionary { get; set; }
    }
}
