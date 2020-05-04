////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/1/2008 9:40:40 AM 
// Description: DataContractTransfer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace Support.Serializer
{
    public class DataContractTransfer:BaseXmlTransfer
    {
        override public void Serialize(object it, Type objType, Stream stream)
        {
            
            XmlTextWriter writer = new XmlTextWriter(stream, TheEncoding);
            DataContractSerializer serializer = new DataContractSerializer(objType,this.KnownTypes);
            serializer.WriteObject(writer, it);
            writer.Flush();
            //writer.Close(); //也會造成stream.Close()
        
            /*
            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                DataContractSerializer serializer = new DataContractSerializer(objType);
                serializer.WriteObject(writer, it);
                writer.Close();
            }
             */
        }


        override public object Deserialize(Stream stream, Type type)
        {
            DataContractSerializer wcfSerializer = new DataContractSerializer(type);
            //XmlTextWriter writer = new XmlTextWriter(stream, TheEncoding);
            return wcfSerializer.ReadObject(stream);
        }
    }

}
