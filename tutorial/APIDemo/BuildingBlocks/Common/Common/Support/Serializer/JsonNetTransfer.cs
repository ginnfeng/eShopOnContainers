
/*
using Newtonsoft.Json;

#if SILVERLIGHT
using System.Json;
#endif
namespace Common.Support.Serializer
{
    class JsonNetTransfer : BaseTransfer
    {
        readonly JsonSerializer serializer = new JsonSerializer();
        public JsonNetTransfer()
        {
            TheEncoding = Encoding.UTF8; ;
        }
        override public void Serialize(object it, Type objType, Stream stream)
        {
            
            using (var writer = new StreamWriter(stream))
            {
                var jsonTextWriter = new JsonTextWriter(writer);
                serializer.Serialize(jsonTextWriter, it);
                jsonTextWriter.Flush();
                stream.Position = 0;
            }
        }
        override public object Deserialize(Stream stream, Type type)
        {
            using (var reader = new StreamReader(stream))
            {
                var jsonTextReader = new JsonTextReader(reader);
                return serializer.Deserialize(jsonTextReader, type);
            }
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
*/