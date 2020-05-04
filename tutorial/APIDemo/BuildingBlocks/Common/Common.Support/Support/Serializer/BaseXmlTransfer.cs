////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/1/2008 9:49:30 AM 
// Description: BaseXmlTransfer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.IO;
using System.Xml;

namespace Support.Serializer
{
    abstract public class BaseXmlTransfer:BaseTransfer
    {
        public BaseXmlTransfer()
        {
            
        }

        public T ToObject<T>(XmlDocument xmlDoc)
        {
            return (T)ToObject(xmlDoc,typeof(T));
        }
        public object ToObject(XmlDocument xmlDoc, Type type)
        {
            using (MemoryStream stream = ConvertHelper.ConvertToStream<MemoryStream>(xmlDoc))
            {
                stream.Position = 0;
                return Deserialize(stream,type);
            }
        }        

        public object ToObject(string xml,Type type)
        {
            return base.ToObject(xml, type, RetriveTextEncoding(xml));
        }
        public XmlDocument ToXmlDocument<T>(T it)
        {
            return ToXmlDocument(it, typeof(T));
        }

        public XmlDocument ToXmlDocument(object it, Type type)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(it, type, ms);
                ms.Position = 0;
                return ConvertHelper.ConvertToXmlDocument(ms);
            }
        }
        public string ToXml<T>(T it)
        {
            return base.ToText<T>(it);
        }

        override protected Encoding RetriveTextEncoding(string text)
        {
            return ConvertHelper.TakeEncodingFromXml(text); 
        }
        
   
    }
}
