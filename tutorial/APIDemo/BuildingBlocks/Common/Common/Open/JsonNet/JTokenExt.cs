
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/2/2015 6:15:40 PM 
// Description: JObjectExt.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Newtonsoft.Json.Linq;
using Common.Support.Net.Util;

namespace Common.Open.JsonNet
{
    static public class JTokenExt
    {
        public static string GenParentName(this JToken jToken)
        {
            if (jToken == null)
                return "";
            switch (jToken.Type)
            {
                case JTokenType.Array:
                    return GenParentName(jToken.Parent) + "[]";
                case JTokenType.Property:
                    JProperty prop = jToken as JProperty;
                    string parentName = GenParentName(jToken.Parent);
                    return string.IsNullOrEmpty(parentName) ? prop.Name : parentName + "." + prop.Name;
                default:
                    return GenParentName(jToken.Parent);
            }
        }
        public static void ProcessingJson(this JToken jToken, Action<JValue> valueAct)
        {
            switch (jToken.Type)
            {
                case JTokenType.Array:
                    JArray array = jToken as JArray;
                    array.ForEach(item => ProcessingJson(item, valueAct));
                    break;
                case JTokenType.Object:
                    JObject obj = jToken as JObject;
                    obj.Children().ForEach(item =>ProcessingJson(item, valueAct));
                    break;
                case JTokenType.Property:
                    var property = jToken as JProperty;
                    ProcessingJson(property.Value, valueAct);
                    break;
                default:
                    valueAct(jToken as JValue);
                    break;
            }
        }
    }
}
