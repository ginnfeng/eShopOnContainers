////*************************Copyright © 2013 Feng 豐**************************
// Created    : 9/19/2014 5:46:51 PM
// Description: JsonNetTransfer.cs
// 使用Json.Net取代.net 內建JsonSerializer機制
//http://alexandrebrisebois.wordpress.com/2012/06/24/using-json-net-to-serialize-objects/
// Revisions  :
// ****************************************************************************
using System;
using System.Text;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if SILVERLIGHT
using System.Json;
#endif

namespace Support.Serializer
{
    public class JsonNetTransfer : BaseTransfer
    {
        private readonly JsonSerializer serializer = new JsonSerializer();

        public JsonNetTransfer()
        {
            //Json.Net很奇怪，在Serialize需用Encoding.Default,Deserialize用Encoding.UTF8
            //在英文版OS，中文字序列化後會出現亂碼。解法: Region=>Administrative=>Change system locale...=>改成Chines(Traditional,Taiwan)
            TheEncoding = Encoding.Default;
        }

        override protected Encoding RetriveTextEncoding(string text)
        {
            return Encoding.UTF8;
        }

        static public bool IsJObject(object it)
        {
            return it == null ? false : it.GetType().Equals(typeof(JObject));
        }

        override public void Serialize(object it, Type objType, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.Default, 120, true))
            {
                var jsonTextWriter = new JsonTextWriter(writer);
                serializer.Serialize(jsonTextWriter, it);
                jsonTextWriter.Flush();
            }
        }

        override public object Deserialize(Stream stream, Type type)
        {
            //using (var reader = new StreamReader(stream)) // 2016/06/29
            using (var reader = new StreamReader(stream, TheEncoding))
            {
                var jsonTextReader = new JsonTextReader(reader);
                return serializer.Deserialize(jsonTextReader, type);
            }
        }

        override public T AutoToObject<T>(object text, bool asFieldMember = false)
        {
            if (text == null) return default(T);
            //JSON.Net在序列化時，若某Field Type為byte[]時，其轉為string,若欲將此string value單獨轉回byte[],則需在外加上""
            if (typeof(T).Equals(typeof(byte[])))
                return base.ToObject<T>("\"" + text.ToString() + "\"");

            JObject jobject = text as JObject;
            return (jobject == null) ? base.AutoToObject<T>(text) : jobject.ToObject<T>();
        }

        /// <summary>
        /// 107/5/8 override ToObject<T>，因base.ToObject<T>容易在有些文字格式時發生exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        override public T ToObject<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
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