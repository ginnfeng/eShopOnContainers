////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/28/2011 2:12:19 PM 
// Description: DynamicProxy.cs  
//           string xml = "<Books><Book sn='001'><Title>Office365</Title><Publish><Company>MS</Company></Publish></Book></Books>";
//           dynamic xmlProxy = new DynamicXml(xml);
//           print(xmlProxy.Title.Value);
//           print(xmlProxy.Publish.Company.Value);
//           print(xmlProxy["sn"].Value);
//           xmlProxy["sn"] = "AAAAAA";
//           print(xmlProxy["sn"].Value);
//           IEnumerable<XAttribute> attris = xmlProxy.Attributes;  
//            dynamic book=xmlProxy.XPathSelectElement("//Book[@sn='001']");
//            print(book.Publish.Company.Value) // print "MS"
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
using System.Dynamic;
using System.Collections;
using System.Xml.Linq;
using System.Linq;

//use XPath with LINQ to XML
using System.Xml.XPath;

namespace Common.Support.Net.Proxy
{
    public class DynamicXml : DynamicXmlBase<XElement>, IEnumerable
    {
        public DynamicXml(string srcXML)
            : this(XDocument.Parse(srcXML))
    	{  
    	}
        public DynamicXml(XDocument xdoc)
            :this(xdoc.Root)
        {
        }
        public DynamicXml(XElement xRoot)
            :base(xRoot)
        {
            Node = xRoot;
        }
        public List<IDynamicXmlBase> XPathEvaluate(string expression)
        {
            return ConvertTo((IEnumerable)Node.XPathEvaluate(expression));          
        }
        public IDynamicXmlBase XPathSelectElement(string expression)
        {           
            return ConvertTo(Node.XPathSelectElement(expression));          
        }
        public List<IDynamicXmlBase> XPathSelectElements(string expression)
        {
            return ConvertTo(Node.XPathSelectElements(expression));
        }
        private List<IDynamicXmlBase> ConvertTo(IEnumerable rlts)
        {
            var list = new List<IDynamicXmlBase>();
            foreach (var item in rlts)
            {
                var it = ConvertTo(item);
                if (it != null) list.Add(it);
            }
            return list;
        }
        private IDynamicXmlBase ConvertTo(object rlt)
        {
            var xElement = rlt as XElement;
            if (xElement != null)
                return new DynamicXml(xElement);
            var xAttribute = rlt as XAttribute;
            if (xAttribute != null)
                return new DynamicXmlAttribute(xAttribute);
            return null;
        }
        override public string Value 
        {
            get { return Node.Value; }
            set { Node.Value = value; }
        }
        public List<XAttribute> Attributes 
        {
            get 
            {
                var attris=new List<XAttribute>();
                attris.AddRange(Node.Attributes());
                return attris; 
            }         
        }
        /// <summary>
        /// Nodes.Count
        /// </summary>
        public int Count 
        { 
            get { return Node.Nodes().Count(); } 
        }              
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {            
            result = null;
            switch (binder.Name)
            {  
                default:
                     var items = Node.Descendants(XName.Get(binder.Name));
                    if (items == null || items.Count() == 0) return false;
                    result = new DynamicXml(items.First());
                    break;
            }            
            return true;
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            object result;
            if (!TryGetIndex(null, indexes, out result))
                return false;
            var attri = result as XAttribute;
            attri.Value = (value==null)?null:value.ToString();
            return true;
        }       
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {  
            var idxStr = indexes[0] as string;
            if (idxStr == null)
            {                
                int ndx = (int)indexes[0];
                result = Node.Attributes().ElementAt(ndx).Value;
                return true;
            }            
            result = Node.Attribute(XName.Get(idxStr));
            return result != null;            
        }

        #region IEnumerable Members
        public IEnumerator GetEnumerator()
        {
            foreach (var node in Node.Nodes())
            {
                yield return new DynamicXml((XElement)node);
            }
        }       
        #endregion       
    }

    
}
