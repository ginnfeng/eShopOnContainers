using System;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Support.Serializer;

namespace Common.Support
{
    public class MsgTransfer
    {

        static public string Object2XmlString(object it)
        {
            return xmlTransfer.ToXml(it);            
        }

        static public XmlDocument Object2Xml(object it)
        {            
            bool isXmlFormatterContract = IsXmlFormatterContract(it.GetType());
            return Object2Xml(it, it.GetType(), isXmlFormatterContract);
        }

        static public XmlDocument Object2Xml(object it,Type type)
        {            
            bool isXmlFormatterContract = IsXmlFormatterContract(it.GetType());
            return Object2Xml(it, type, isXmlFormatterContract);
        }

        static public XmlDocument Object2Xml(object it, bool isXmlFormatterContract)  
        {            
            return Object2Xml(it, it.GetType(),isXmlFormatterContract);
        }

        static public XmlDocument Object2Xml(object it, Type type, bool isXmlFormatterContract)
        {
            BaseXmlTransfer xmlTransfer = GetXmlTransfer(isXmlFormatterContract);
            return xmlTransfer.ToXmlDocument(it, type);
            /*
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(it, type, ms, isXmlFormatterContract);
                ms.Position = 0;
                XmlTextReader xReader = new XmlTextReader(ms);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xReader);
                return xmlDoc;
            }
            */
        }

        static public object Xml2Object(XmlDocument xmlDoc, Type type)
        {
            bool isXmlFormatterContract = IsXmlFormatterContract(type);
            return Xml2Object(xmlDoc, type, isXmlFormatterContract);
        }

        static public object Xml2Object(XmlDocument xmlDoc, Type type, bool isXmlFormatterContract)
        {
            BaseXmlTransfer xmlTransfer = GetXmlTransfer(isXmlFormatterContract);
            return xmlTransfer.ToObject(xmlDoc, type);
            //return Deserialize(xmlDoc, type, isXmlFormatterContract);            
        }

        static public object Xml2Object(string xml, Type type)
        {
            bool isXmlFormatterContract = IsXmlFormatterContract(type);
            return Xml2Object(xml,type,isXmlFormatterContract);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <param name="isXmlFormatterContract">true:XmlSerializer , false:DataContractSerializer </param>
        /// <returns></returns>
        static public object Xml2Object(string xml, Type type, bool isXmlFormatterContract)
        {
            if (string.IsNullOrEmpty(xml))
                return null;
            BaseXmlTransfer transfer=GetXmlTransfer(isXmlFormatterContract);
            return transfer.ToObject(xml,type);
            /*
            XmlDocument xmlDoc = new XmlDocument();            
            xmlDoc.LoadXml(xml);
            return Xml2Object(xmlDoc, type, isXmlFormatterContract);
            */
        }


        
        public static string Object2BinaryString(object it)
        {            
            using (MemoryStream ms = new MemoryStream())
            {                
                Object2BinaryStream(it, ms);
                string binaryString = Convert.ToBase64String(ms.ToArray());
                return binaryString;
            }         
        }

        public static void Object2BinaryStream(object it, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();                        
            formatter.Serialize(stream,it);            
        }

        /// <summary>
        /// 如果不能轉回原type,則必須自訂SerializationBinder,並指定給formatter.Binder
        /// </summary>
        /// <param name="binaryString"></param>
        /// <returns></returns>
        public static object BinaryString2Object(string binaryValue)
        {
            byte[] bts = Convert.FromBase64String(binaryValue);
            using (MemoryStream ms = new MemoryStream(bts))
            {                
                return BinaryStream2Object(ms);                
            }            
        }

        public static object BinaryStream2Object(Stream binaryStream)
        {
            return binaryTransfer.Deserialize(binaryStream);
            
        }

        public static string ConvertToString(MemoryStream stream)
        {
            return Convert.ToBase64String(stream.ToArray());
        }

        static public bool IsXmlFormatterContract(Type objType)
        {
            return ! Attribute.IsDefined(objType, typeof(DataContractAttribute));
        }
        static XmlSerializer CreateXmlSerializer(Type objType)
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
        /*
        static void Serialize(object it, Type objType, Stream stream, bool isXmlFormatterContract)
        {
            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                
                if (isXmlFormatterContract)
                {
                    //XmlSerializer serializer = new XmlSerializer(objType);
                    XmlSerializer serializer = CreateXmlSerializer(objType);
                    serializer.Serialize(writer, it);
                }
                else
                {
                    DataContractSerializer serializer = new DataContractSerializer(objType);
                    serializer.WriteObject(writer, it);
                }
                writer.Flush();
                writer.Close();
            }
        }
         */
        /*
        static object Deserialize(XmlDocument xmlDoc, Type objType,bool isXmlFormatterContract)
        {
            if (isXmlFormatterContract)
            {                   
                using (MemoryStream ms =new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms))
                {                    
                    xmlDoc.Save(sw);
                    using (MemoryStream msForAry = new MemoryStream(((MemoryStream)sw.BaseStream).ToArray()))
                    {
                        XmlSerializer serializer = CreateXmlSerializer(objType);
                        return serializer.Deserialize(msForAry);
                    }
                }                
            }
            else
            {
                using (XmlReader xmlReader = new XmlNodeReader(xmlDoc.DocumentElement))
                {
                    DataContractSerializer serializer = new DataContractSerializer(objType);
                    return serializer.ReadObject(xmlReader);
                }
            }
        }
        */
        static BaseXmlTransfer GetXmlTransfer(bool isXmlFormatterContract)
        {
            return isXmlFormatterContract ? (BaseXmlTransfer)xmlTransfer : dataContractTransfer;
        }

        static XmlTransfer xmlTransfer=new XmlTransfer();
        static DataContractTransfer dataContractTransfer = new DataContractTransfer();
        static BinaryTransfer binaryTransfer = new BinaryTransfer();
    }
}
