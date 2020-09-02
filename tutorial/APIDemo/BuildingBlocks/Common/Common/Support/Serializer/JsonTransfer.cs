////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 1/27/2011 10:56:10 AM 
// Description: JsonTransfer.cs  
// 參考 Working with JSON Data http://msdn.microsoft.com/en-us/library/cc197957(v=vs.95).aspx
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

#if SILVERLIGHT
using System.Json;
using System.Collections;
#endif



namespace Common.Support.Serializer
{
    /// <summary>
    /// 只支援 UTF-8(、UTF-16LE 和 UTF-16BE) 編碼。
    /// </summary>
    public class JsonTransfer : BaseTransfer
    {
        public JsonTransfer()
        {
            TheEncoding = Encoding.UTF8; ;
        }
        override public void Serialize(object it, Type objType, Stream stream)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType,base.KnownTypes);
            serializer.WriteObject(stream, it);
            stream.Flush();
        }
        override public object Deserialize(Stream stream, Type type)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(stream);
        }
#if SILVERLIGHT


        /// <typeparam name="T">可為List<>或有定DataContractAttribute之class</typeparam>        
        /// <returns>JsonValue 可cast到JsonObject或JsonArray</returns>
        public JsonValue ToJsonValue<T>(T it)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(it, it.GetType(), ms);
                ms.Position = 0;
                return JsonValue.Load(ms);
            }
        }
        public JsonValue ToJsonValue(string jsonText)
        {
            Byte[] buffer = TheEncoding.GetBytes(jsonText);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return JsonValue.Load(stream);
            }
        }
        override public T ToObject<T>(string jsonText)
        {
            var jsonValue=ToJsonValue(jsonText);
            return (T)ToObject(jsonValue, typeof(T));
        }
        //此會有問題,導致override T ToObject<T>(string jsonText)不會被呼叫
        //public T ToObject<T>(JsonValue json)
        //{ }
        public object ToObject(JsonValue json, Type type)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                json.Save(stream);
                return Deserialize(stream, type);
            }
        }
        public string ToText(JsonValue json)
        {
            return json.ToString();
        }
#endif
    }
}
