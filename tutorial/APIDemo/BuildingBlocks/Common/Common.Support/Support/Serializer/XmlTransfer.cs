////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/1/2008 9:40:00 AM 
// Description: XmlTransfer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Support.Serializer
{
    public class XmlTransfer:BaseXmlTransfer
    {
        public XmlTransfer()
        {

        }
        public XmlTransfer(bool noNamespace)
        {
            WithoutNamespaces(noNamespace);
        }
        public void WithoutNamespaces(bool noNamespace)
        {
            XmlNamespaces=null;
            if(noNamespace)
            {
                XmlNamespaces=new XmlSerializerNamespaces();
                XmlNamespaces.Add("","");
            }
        }
        public XmlSerializerNamespaces XmlNamespaces { get; set; }

        override public void Serialize(object it, Type type, Stream stream)
        {
            if (it == null) throw new NullReferenceException();
            
            XmlTextWriter writer = new XmlTextWriter(stream, TheEncoding);
            XmlSerializer serializer = ConvertHelper.CreateXmlSerializer(type);
            if(XmlNamespaces==null)
                serializer.Serialize(writer, it);
            else
                serializer.Serialize(writer, it,XmlNamespaces);
            writer.Flush();
            //writer.Close() 也會造成stream.Close()
        }

        override public object Deserialize(Stream stream,Type type)
        {
            
            XmlSerializer serializer = ConvertHelper.CreateXmlSerializer(type);            
            //XmlTextWriter writer = new XmlTextWriter(stream, TheEncoding);
            //writer.Close() 也會造成stream.Close()
            return serializer.Deserialize(stream);
        }
       
    }
}
