////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/15/2009 3:27:41 PM 
// Description: StringExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Support.Net.Util
{
    static public class StringExtension
    {
        static public void Split(this string it, Func<string, bool> procMethod, params char[] separators)
        {
            foreach (var str in it.Split(separators))
            {
                if (!procMethod(str))
                    break;
            }
        }
        static public void Split(this string it, Func<int ,string, bool> procMethod, params char[] separators)
        {
            int idx = 0;
            foreach (var str in it.Split(separators))
            {
                if (!procMethod(idx++,str))
                    break;
            }
        }
        static public void Split(this string it, Func<string, bool> procMethod, int startIndex, int length)
        {
            int idx = 0;
            do
            {
                var substr = it.Substring(idx, (idx + length) < it.Length ? length : it.Length - idx);
                if (!procMethod(substr)) break;
                idx = idx + length;
            } while (idx < it.Length);

        }

        /// <summary>
        /// 聯集
        /// </summary>        
        static public string UnionListString(this string it, string src, char delimitSymbol)
        {
            HashSet<string> set1 = new HashSet<string>(it.Split(delimitSymbol));
            HashSet<string> set2 = new HashSet<string>(src.Split(delimitSymbol));
            return set1.Union(set2).ToListString(delimitSymbol);
        }
        /// <summary>
        /// 交集
        /// </summary> 
        static public string IntersectListString(this string it, string src, char delimitSymbol)
        {
            HashSet<string> set1 = new HashSet<string>(it.Split(delimitSymbol));
            HashSet<string> set2 = new HashSet<string>(src.Split(delimitSymbol));
            return set1.Intersect(set2).ToListString(delimitSymbol);
        }

        static public string RemoveListString(this string it, string src, char delimitSymbol)
        {
            HashSet<string> set = new HashSet<string>(it.Split(delimitSymbol));
            src.Split(id => { set.Remove(id); return true; }, delimitSymbol);
            return set.ToListString(delimitSymbol);
        }
        static public bool EqualsListString(this string it, string src, char delimitSymbol)
        {
            HashSet<string> set1 = new HashSet<string>(it.Split(delimitSymbol));
            HashSet<string> set2 = new HashSet<string>(src.Split(delimitSymbol));
            return set1.Equals(set2);           
        }
        static public bool ContainsListString(this string it, string src, char delimitSymbol)
        {
            HashSet<string> set1 = new HashSet<string>(it.Split(delimitSymbol));
            HashSet<string> set2 = new HashSet<string>(src.Split(delimitSymbol));
            return set1.Contains(set2);
        }

        
        
        static public string ToListString<T>(this IEnumerable<T> it, char delimitSymbol)
        {
            var sb = new StringBuilder();
            foreach (var item in it)
            {
                if (sb.Length != 0) sb.Append(delimitSymbol);
                sb.Append(item);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 插入字串:例如每3字強制換行  PutIn(0,3,"\n")
        /// </summary>        
        static public string PutIn(this string it, int startIndex, int length, string putInString)
        {
            StringBuilder sb = new StringBuilder();
            it.Split(str => { if (sb.Length > 0)sb.Append(putInString); sb.Append(str); return true; }, startIndex, length);
            return sb.ToString();
        }
        static public List<KeyValuePair<string, string>> ReadPairs(this string it, char pairSymbol, char separatorSymbol)
        {
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
            it.ReadPairs(pair => { keyValues.Add(pair); return true; }, pairSymbol, separatorSymbol);
            return keyValues;
        }

        static public void ReadPairs(this string it, Func<KeyValuePair<string, string>, bool> procMethod, char pairSymbol, char separatorSymbol)
        {
            string regexDef = Support.CommonExtension.StringFormat(keyValueRegexDef, pairSymbol, separatorSymbol);
            Regex regex = new Regex(regexDef);
            foreach (Match match in regex.Matches(it))
            {
                string value = string.IsNullOrEmpty(match.Groups[3].Value) ? match.Groups[2].Value : match.Groups[3].Value;
                if (!procMethod(new KeyValuePair<string, string>(match.Groups[1].Value, TrimSideQuote(value))))
                    break;
            }
        }

        static public string Truncate(this string it,int length)
        {
            if (string.IsNullOrEmpty(it) || it.Length <= length) return it;
            
            return it.Substring(0,length);
        }

        static public List<string> ReadLines(this string it)
        {
            List<string> lines = new List<string>();
            it.ReadLines(line => { lines.Add(line); return true; });
            return lines;
        }

        static public void ReadLines(this string it, Func<string, bool> procMethod)
        {
            StringReader reader = new StringReader(it);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!procMethod(line))
                    break;
            }
        }

        /// <summary>
        /// trim unnecessary whitespace from the start and the end of a string 
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        static public string TrimSideWhitespace(this string it)
        {
            return string.IsNullOrEmpty(it) ? it : trimSideWhitespaceRegex.Replace(it, "");
        }
        static public string TrimSideQuote(this string it)
        {
            return string.IsNullOrEmpty(it) ? it : trimSideQuoteRegex.Replace(it, "");
        }
        static public Regex GenSideReplaceRegex(string leftSideSymbol, string rightSideSymbol)
        {
            var trimSide = CommonExtension.StringFormat(trimSideSymbolDef, leftSideSymbol, rightSideSymbol);
            return new Regex(trimSide);
        }

        static public T ToObject<T>(this string it)
        {
            return (T)ToObject(it, typeof(T));
        }

        static public object ToObject(this string it, Type targetType)
        {
            return CommonExtension.ToObject(it, targetType);
        }

        static public object ToObject(this object it, Type targetType)
        {
            return (it != null && it.GetType() == typeof(string))
            ? ((string)it).ToObject(targetType)
            : (targetType.IsPrimitive || targetType.IsValueType)
            ?Activator.CreateInstance(targetType):it;
        }

        static Regex trimSideWhitespaceRegex = new Regex(@"^[ \t]+|[ \t]+$");
        static string trimSideSymbolDef = @"^[{0}]+|[{1}]+$";
        static readonly Regex trimSideQuoteRegex = GenSideReplaceRegex("\"'", "\"'");
        //([^=,\s\t]*)[\s\t]{0,}=[\s\t]{0,}([^,\s\t]*)
        const string keyValueRegexDef = @"([^{0}{1}\s\t]*)[\s\t]{{0,}}=[\s\t]{{0,}}([^{1}\s\t]*)";


        static public  char DefaultSeparatorSymbol = ',';
        static public char DefaultPairSymbol = '=';
        static public StringBuilder AddKeyValue(this StringBuilder it, string key, object value)
        {            
            return it.AddKeyValue(key, value, DefaultSeparatorSymbol, DefaultPairSymbol);
        }
        
        static public StringBuilder AddKeyValue(this StringBuilder it, string key, object value, char separatorSymbol, char pairSymbol)
        {
            if (it.Length != 0) it.Append(separatorSymbol);
            it.Append(key)
              .Append(pairSymbol)
              .Append((value == null) ? null : value.ToString());
            return it;
        }
        static public StringBuilder AppendObject(this StringBuilder it, object obj,bool isReturnXml)
        {
            if (obj == null) return it;
            var type = obj.GetType();
            string sb = (isReturnXml) ? "\"" : "";
            
                
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance))
            {
                //if (!prop.CanRead) continue;
                var value = prop.GetValue(obj, null);
                it.AddKeyValue(prop.Name, sb + value + sb);
            }
            if (isReturnXml)
            {
                it.Insert(0, " ").Insert(0, type.Name).Insert(0, "<").Append("\\>");
            }
            return it;            
        }
    }
}
