using System.Xml;
using System.Text.RegularExpressions;

namespace Support
{
    public class XmlHelper
    {
        ///去除namespace
        static public string RemoveNamespace(XmlDocument xmlDoc)
        {
            string result = RemoveNamespace(xmlDoc.InnerXml);
            return result;
        }

        static public string RemoveNamespace(string innerXml)
        {
            Regex regex = new Regex("<(.*?) xmlns[:=].*?>");
            string result = regex.Replace(innerXml, "<$1>");
            return result;
        }
    }
}
