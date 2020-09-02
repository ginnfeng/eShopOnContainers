////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/1/2009 1:46:51 PM 
// Description: Extension.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Xml.Linq;
using System.Xml;

namespace Common.Support.Net.XML
{
    static public class Extension
    {
        public static XDocument GetXDocument(this XmlDocument doc)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
            {
                doc.WriteTo(xmlWriter);
            }
            return xDoc;
        }

        public static XElement GetXElement(this XmlNode node)
        {
            return node.OwnerDocument.GetXDocument().Root;            
        }

        public static XmlDocument GetXmlDocument(this XDocument doc)
        {
            using (XmlReader xmlReader = doc.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }

        }
        public static XmlNode GetXmlNode(this XElement element)
        {
            return element.Document.GetXmlDocument().DocumentElement;            
        }
    }
}
