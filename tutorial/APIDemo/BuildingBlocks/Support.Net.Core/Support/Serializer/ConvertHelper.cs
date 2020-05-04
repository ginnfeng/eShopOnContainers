////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/19/2008 3:44:10 PM 
// Description: TransferHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Support.Serializer
{
    public static class ConvertHelper
    {        
        static public byte[] ConvertToBytes(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return null;
            using (MemoryStream stream = ConvertToStream<MemoryStream>(xmlDoc))
            {
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        static public T ConvertToStream<T>(XmlDocument xmlDoc)
            where T : Stream, new()
        {
            T stream = new T();
            xmlDoc.Save(stream);
            stream.Flush();
            return stream;          
        }

        static public XmlDocument ConvertToXmlDocument(byte[] buffer)
        {
            if (buffer == null)
                return null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return ConvertToXmlDocument(ms);
            }
        }

        static public XmlDocument ConvertToXmlDocument(Stream stream)
        {
            XmlTextReader xReader = new XmlTextReader(stream);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xReader);
            return xmlDoc;
        }



        static internal XmlSerializer CreateXmlSerializer(Type objType)
        {
            string ns = null;
            if (Attribute.IsDefined(objType, typeof(XmlTypeAttribute)))
            {
                XmlTypeAttribute xmlTypeAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(objType, typeof(XmlTypeAttribute));
                ns = xmlTypeAttribute.Namespace;
            }
            XmlSerializer serializer = (ns == null) ? new XmlSerializer(objType) : new XmlSerializer(objType, ns);
            return serializer;
        }

        static public Encoding TakeEncodingFromXml(string xml)
        {
            Match match = regex.Match(xml);
            if (!match.Success)
                return Encoding.UTF8;
            return Encoding.GetEncoding(match.Groups[1].Value);
        }

        /// <\?.*\sencoding="([^"]{3,})".*\?>
        static readonly private Regex regex = new Regex("<\\?.*\\sencoding=\"([^\"]{3,})\".*\\?>");

    }
}
