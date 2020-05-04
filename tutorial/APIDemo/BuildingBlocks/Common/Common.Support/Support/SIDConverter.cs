using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Support.ErrorHandling;
using System.Text.RegularExpressions;

namespace Support
{
    public interface IXmlConverter
    {
        void Xml2Object(string xml);
        string Object2Xml();
    }

    [Serializable]
    public class SidConverter
    {

        public SidConverter() { }
        
        public SidConverter(TypeHelpBase typeHelper) 
        {
            RegisterTypeHelper(typeHelper);
        }
         static public string Obj2Xml(object result)
        {
            if (result == null) return null;
            IXmlConverter converter = result as IXmlConverter;
            if (converter != null)
                return converter.Object2Xml();
            string rltString=MsgTransfer.Object2XmlString(result);
            if (!IsPrimitiveXmlType(result.GetType()))
                return rltString;
            Match match= xmlTextRegex.Match(rltString);
            return match.Groups[1].Value;
            
        }

         public object Xml2Obj(string xml, Type type)
        {
            if (xml == null) return null;
            if(type.IsSubclassOf(typeof(IXmlConverter) ) )
            {
                IXmlConverter converter=GetDefaultValue(type) as IXmlConverter;
                converter.Xml2Object(xml);
                return converter;
            }
            if (IsPrimitiveXmlType(type) && !IsXmlString(xml))
            {
                if (type == typeof(string)) return xml;

                if (string.IsNullOrEmpty(xml))
                {
                    return GetDefaultValue(type);
                }
                if (type == typeof(int))
                    return Convert.ToInt32(xml, CultureInfo.InvariantCulture);
                if (type == typeof(double))
                    return Convert.ToDouble(xml, CultureInfo.InvariantCulture); ;
                if (type == typeof(bool))
                    return Convert.ToBoolean(xml, CultureInfo.InvariantCulture);
                if (type == typeof(DateTime))
                {
                    xml = string.Format(CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", "dateTime", xml);
                    return MsgTransfer.Xml2Object(xml, type, true);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(xml))
                {
                    return GetDefaultValue(type);
                }
                return MsgTransfer.Xml2Object(xml, type, true);
            }
            Exception < TypeConvertExceptionMessage > exception= new Exception<TypeConvertExceptionMessage>();
            exception.Reference = xml;
            exception.ErrorInfo.TargetTypeName = type.FullName;
            throw exception;
        }
        
         public object Xml2Obj(string xml, string typeName)
        {             
             Type type = GetType(typeName);
             if (type != null)
             {                 
                 return Xml2Obj(xml, type);
             }
             Exception<TypeConvertExceptionMessage> exception = new Exception<TypeConvertExceptionMessage>();
             exception.Reference = xml;
             exception.ErrorInfo.TargetTypeName = type.FullName;
             throw exception;
            
        }

         public object GetDefaultValue(string typeName)
         {
             Type type = GetType(typeName);
             return GetDefaultValue(type);
         }

        public object GetDefaultValue(Type type)
        {
            if (TypeDefaultTypeValues != null)
            {
                foreach (object defaultValue in TypeDefaultTypeValues)
                {
                    if (defaultValue.GetType() == type)
                        return ObjectHelper.Clone( defaultValue);
                }    
            }
            if (type == typeof(string))
                return "";
            return Activator.CreateInstance(type);
        }
        public Type[] GetTypes(string[] typeNames)
        {
            Type[] types = new Type[typeNames.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types.SetValue(GetType(typeNames[i]), i);
            }
            return types;
        }
        public Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "System.Int32":
                case "Int32":
                case "int":
                case "integer":
                    return typeof(int);

                case "System.String":
                case "String":
                case "string":
                    return typeof(string);
                case "System.Double":
                case "System.Float":
                    return typeof(double);
                case "System.Boolean":
                    return typeof(bool);
                case "System.DateTime":
                case "DateTime":                
                    return typeof(DateTime);
                default:
                    Type type = Type.GetType(typeName);
                    if(type != null) 
                        return  type;
                    foreach (TypeHelpBase ith in typeHelps.Values)
                    {
                        type = ith.GetType(typeName);
                        if (type != null) break;                            
                    }                    
                    return type;
            }
        }

        public object CreateSidObject(string typeFullName)
        {
            Type type = GetType(typeFullName);
            if (type == null)
                throw new Exception("SIDConvert.CreateSIDObject() typeName:" + typeFullName);
            return Activator.CreateInstance(type);            
        }

        public List<object> GenRangeValue(string itFrom,string itTo,int offset)
        {
            if (string.IsNullOrEmpty(itFrom) ||string.IsNullOrEmpty (itTo))
                return null;
            List<object> values = new List<object>();

            int from = (IsXmlString(itFrom)) ? (int)Xml2Obj(itFrom, typeof(int)) : Convert.ToInt32(itFrom, CultureInfo.CurrentCulture);
            int to = (IsXmlString(itTo)) ? (int)Xml2Obj(itTo, typeof(int)) : Convert.ToInt32(itTo, CultureInfo.CurrentCulture);
            for (int i = from; i <= to; i = i + offset)
            {
                values.Add(i);
            }
            return values;
        }
        public void RegisterTypeHelper(params Type[] types)
        {
            TypeHelpBase[] typeHelpers = new TypeHelpBase[types.Length];
            for(int i=0;i<typeHelpers.Length;i++)
            {
                typeHelpers.SetValue(new TypeHelper(types[i]), i);                
            }
            RegisterTypeHelper(typeHelpers);
        }
        public void RegisterTypeHelper<T>() where T:new()
        {
            RegisterTypeHelper(new TypeHelper<T>());
        }
        public void RegisterTypeHelper(params TypeHelpBase[] typeHelpers)
        {
            foreach (TypeHelpBase typeHelper in typeHelpers)
            {
                if(!typeHelps.ContainsKey(typeHelper.GetCurrentAssembly()))
                    typeHelps[typeHelper.GetCurrentAssembly()] = typeHelper;
            }
        }

        public bool IsPrimitiveXmlType(string typeName)
        {
            Type type = GetType(typeName);
            if(type==null)
                throw new Exception("SIDConvert.IsPrimitiveXmlType() typeName:" + typeName);
            return IsPrimitiveXmlType(type);
        }

        
        static public bool IsPrimitiveXmlType(Type type)
        {            
            bool isPrimitive = type.IsPrimitive || type.IsValueType || (type == typeof(string));
            return isPrimitive;
        }

        static public bool IsXmlString(string it)
        {
            return it.Contains("<?xml");
        }       

        public List<object> TypeDefaultTypeValues
        {
            get { return typeDefaultValues; }
            set { typeDefaultValues = value; }
        }

        private List<object> typeDefaultValues;
        Dictionary<Assembly, TypeHelpBase> typeHelps = new Dictionary<Assembly, TypeHelpBase>();

        // <\?.*\?>.*>(.*)<
        static readonly Regex xmlTextRegex = new Regex("<\\?.*\\?>.*>(.*)<");
    }
}
