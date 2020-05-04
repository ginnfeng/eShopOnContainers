////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/11/2008 2:57:12 PM 
// Description: StringExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.Globalization;
using System.Collections;
//using Support.Helper;

namespace Support
{
    static public class CommonExtension
    {
        static public string StringFormat(string format,params object[]  args)
        {
            return  string.Format(CultureInfo.CurrentCulture, format, args);
        }
        static public string FormatSplit(IEnumerable values, bool needQuote, string stringQuote,params char[] separators)
        {
            StringBuilder builder = new StringBuilder();
            int idx = 0;
            foreach (object item in values)
            {                
                if (++idx!=1 && separators.Length>0) builder.Append(separators[0]);
                builder.Append(ToString(item, needQuote, stringQuote));
            }
            return builder.ToString();
        }
        static public string ToString(DateTime t)
        {
            return t.ToString("yyyy/MM/dd HH:mm:ss ");
        }
        static public string ToString(object it,bool needQuote, string stringQuote)
        {           
            if(it==null || it is System.DBNull) return "";
            Type type = it.GetType();
            return (!type.IsValueType && needQuote) ? stringQuote + it.ToString() + stringQuote : it.ToString();
        }
        static public T ToObject<T>(string sourceString)
        {
            return (T)ToObject(sourceString, typeof(T));
        }
        static public T ConvertTo<T>(object source)
        {
            if(typeof(T).Equals(typeof(string)))
            {
                dynamic str=(source == null) ? null : source.ToString();
                return (T)str ;
            }
            return (T)ToObject(source, typeof(T));
        }

        static public object ToObject(object src, Type targetType)
        {            
            return (src is string) ? ToObject((string)src, targetType) :
                (targetType.IsValueType && src==null) ? Activator.CreateInstance(targetType) : src;
        }
        static public object ToObject(string sourceString,Type targetType)
        {
            if (targetType.IsAssignableFrom(typeof(string)) ) return sourceString;
            if (string.IsNullOrEmpty(sourceString))
                return Activator.CreateInstance(targetType);
            if (targetType.IsEnum) return Enum.Parse(targetType, sourceString,false);
            if (targetType == typeof(int)) return Convert.ToInt32(sourceString, CultureInfo.InvariantCulture);
            if (targetType == typeof(long)) return Convert.ToInt64(sourceString, CultureInfo.InvariantCulture);
            if (targetType == typeof(float)) return Convert.ToSingle(sourceString, CultureInfo.InvariantCulture);
            if (targetType == typeof(double)) return Convert.ToDouble(sourceString, CultureInfo.InvariantCulture); ;
            if (targetType == typeof(bool)) return Convert.ToBoolean(sourceString, CultureInfo.InvariantCulture);
            if (targetType == typeof(DateTime)) return Convert.ToDateTime(sourceString);
            if (targetType == typeof(TimeSpan)) return TimeSpan.Parse(sourceString);
            if (targetType == typeof(Type)) return Type.GetType(sourceString);
            return null;
        }
        static public bool IsPrimitiveXmlType(Type type)
        {
            bool isPrimitive = type.IsPrimitive || type.IsValueType || (type == typeof(string));
            return isPrimitive;
        }
    }
}
