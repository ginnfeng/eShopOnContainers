////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/21/2010 1:42:43 PM 
// Description: EnumParser.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Support.Helper
{
    public class EnumParser
    {
        public static bool IsEquals(object value1,object value2)
        {
            return (value1 == null || value2 == null)
            ? value1 == value2
            : value1.ToString().Equals(value2.ToString());            
        }
    }
    public class EnumParser<TEnum>            
    {
        static public EnumParser<TEnum> Instance 
        { 
            get { return Singleton0<EnumParser<TEnum>>.Instance; } 
        }

        public EnumParser()
        {
            EnumType = typeof(TEnum);
        }
        public string GetName(object value)
        {
            return GetName((TEnum)value);
        }
        public string GetName(TEnum value)
        {
            return Enum.GetName(EnumType,value);
        }
        public string[] GetNames()
        {
            return Enum.GetNames(EnumType);
        }
        public Array GetValues()
        {
            return Enum.GetValues(EnumType);
        }
       
        public bool IsDefined(object value)
        {
            if (value.GetType() != EnumType)
            {
                return false;
            }
            return Enum.IsDefined(EnumType, value);
        }
        public TEnum Parse(string value)
        {
            return Parse(value,true);
        }
        public TEnum Parse(string value, bool ignoreCase)
        {
            return (TEnum)Enum.Parse(EnumType, value, ignoreCase);
        }
        public Type EnumType
        {
            get{return enumType;}
            set 
            {
                if (enumType != value) fieldInfoMap.Clear();
                enumType = value;
            }
        }        
        public TAttribute GetCustomAttributes<TAttribute>(string name)
          where TAttribute : System.Attribute, new()
        {
            return GetCustomAttributes<TAttribute>(name,true);
        }
        public TAttribute GetCustomAttributes<TAttribute>(string name, bool inherit)
            where TAttribute : System.Attribute, new()
        {
            return GetCustomAttributes<TAttribute>(Parse(name), inherit);
        }
        public TAttribute GetCustomAttributes<TAttribute>(TEnum value)
            where TAttribute : System.Attribute, new()
        {
            return GetCustomAttributes<TAttribute>(value,true);
        }
        public bool TryGetCustomAttributes<TAttribute>(string name, bool inherit, out TAttribute attribute)
           where TAttribute : System.Attribute, new()
        {
            return TryGetCustomAttributes<TAttribute>(Parse(name),  inherit, out  attribute);
        }
        public bool TryGetCustomAttributes<TAttribute>(TEnum value, bool inherit, out TAttribute attribute)
            where TAttribute : System.Attribute, new()
        {
            attribute=GetCustomAttributes<TAttribute>(value, inherit);
            return attribute != null;
        }
        public TAttribute GetCustomAttributes<TAttribute>(TEnum value,bool inherit)
            where TAttribute:System.Attribute,new()
        {
            if (fieldInfoMap.Count == 0) BuildAttributeMap();
            FieldInfo fieldInfo;
            if (!fieldInfoMap.TryGetValue(value, out fieldInfo))
                throw new MemberAccessException("EnumParser.GetCustomAttributes<TAttribute>()");
            object[] atributes = fieldInfo.GetCustomAttributes(typeof(TAttribute), inherit);
            return (atributes.Length == 0) ? null : (TAttribute)atributes[0]; 
        }
        private void BuildAttributeMap()
        {            
            FieldInfo[] fieldInfos = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                fieldInfoMap[Parse(fieldInfo.Name)]=fieldInfo;               
            }
        }
        private Type enumType;
        private Dictionary<TEnum, FieldInfo> fieldInfoMap = new Dictionary<TEnum, FieldInfo>();        
    }
}
