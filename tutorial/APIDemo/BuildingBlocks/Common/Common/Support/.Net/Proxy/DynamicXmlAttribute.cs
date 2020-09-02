////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/29/2011 10:28:07 AM 
// Description: DynamicXmlAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Xml.Linq;

namespace Common.Support.Net.Proxy
{
    public class DynamicXmlAttribute : DynamicXmlBase<XAttribute>
    {
        public DynamicXmlAttribute(XAttribute xAttribute)
            : base(xAttribute)
        {
        }
        override public string Value
        {
            get { return Node.Value; }
            set { Node.Value = value; }
        }
    }

}
