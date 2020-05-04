////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 2/26/2015 4:12:15 PM 
// Description: DynamicJson.cs  
// Revisions  :            		
// **************************************************************************** 
//using Support.Net.Proxy;
namespace Support.Open.Net.Proxy
{
    //public class DynamicJsonProperty : DynamicXmlBase<JProperty>
    //{
    //    public DynamicJsonProperty(JProperty property)
    //        : base(property)
    //    { }
    //}
    //public class DynamicJsonArray : DynamicXmlBase<JArray>
    //{
    //    public DynamicJsonArray(JArray array)
    //        : base(array)
    //    { }
    //}
    //public class DynamicJson : DynamicXmlBase<JObject>
    //{
    //    override public string Value
    //    {
    //        get { return base.Node.ToString(); }
    //        set { Node = JObject.Parse(value); }
    //    }

    //    public override bool TryGetMember(GetMemberBinder binder, out object result)
    //    {
    //        JValue result = null;
    //        JToken it;
    //        if (!Node.TryGetValue(binder.Name, out it))
    //            return false;
    //        switch (it.Type)
    //        {
    //            case JTokenType.Property:
    //                result = new DynamicJsonProperty(it as JProperty);
    //                break;
    //            case JTokenType.Array:
    //                result = new DynamicJsonArray(it as JArray);
    //                break;
    //            case JTokenType.Boolean:
    //                break;
    //            case JTokenType.Bytes:
    //                break;
    //            case JTokenType.Comment:
    //                break;
    //            case JTokenType.Constructor:
    //                break;
    //            case JTokenType.Date:
    //                break;
    //            case JTokenType.Float:
    //                break;
    //            case JTokenType.Guid:
    //                break;
    //            case JTokenType.Integer:
    //                break;
    //            case JTokenType.None:
    //                break;
    //            case JTokenType.Null:
    //                break;
    //            case JTokenType.Object:
    //                break;
    //            case JTokenType.Raw:
    //                break;
    //            case JTokenType.String:
    //                break;
    //            case JTokenType.TimeSpan:
    //                break;
    //            case JTokenType.Undefined:
    //                break;
    //            case JTokenType.Uri:
    //                break;
    //            default:
    //                break;
    //        }

    //        return true;
    //    }
    //    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    //    {
    //        object result;
    //        if (!TryGetIndex(null, indexes, out result))
    //            return false;
    //        var attri = result as XAttribute;
    //        attri.Value = (value == null) ? null : value.ToString();
    //        return true;
    //    }
    //    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    //    {
    //        var idxStr = indexes[0] as string;
    //        if (idxStr == null)
    //        {
    //            int ndx = (int)indexes[0];
    //            result = Node.Attributes().ElementAt(ndx).Value;
    //            return true;
    //        }
    //        result = Node.Attribute(XName.Get(idxStr));
    //        return result != null;
    //    }
    //}
}
