////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/31/2011 3:07:05 PM 
// Description: StringPairProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
using Common.Support.Net.Proxy;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common.DataCore
{
    /// <summary>
    /// ex: 
    ///     var proxy=StringRegularPairProxy("$name_$tel","([^_]{1,})");
    ///     proxy.Input="Jhon_02324678";
    ///     var name=proxy["$name"]  ;//"Jhon"
    /// </summary>
    public class StringRegularPairProxy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spec">ex:"$name_$tel"</param>
        /// <param name="regexStr"> ex: "([^_]{1,})"</param>
        public StringRegularPairProxy(string spec,string regexStr="([^_]{1,})")            
        {
            regex=new Regex(regexStr);
            keys=regex.Matches(spec);
        }
       
        public string Input
        {
            get { return input; }
            set
            {                
                input =value;
                map.Clear();
                var matches = regex.Matches(input);
                for (int i = 0; i < matches.Count; i++)
                {
                    if (i >= (keys.Count - 1)) break;                    
                    map[keys[i].Groups[1].Value] = matches[i].Groups[1].Value;
                }                
            }
        }

        public object this[string propertyKey]
        {
            get { return GetPropertyValue(null, propertyKey); }
            //set { SetPropertyValue(null, propertyKey, value); }
        }
        public TEntity GenEntityProxy<TEntity>()
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += GetPropertyValue;            
            return realProxy.Entity;
        }

        

        private object GetPropertyValue(MethodInfo methodInfo, string propertyName)
        {
            string value=null;
            map.TryGetValue(propertyName, out value);
            return value;            
        }

        private string input="";
       
        private Regex regex;
        private MatchCollection keys;
        private Dictionary<string, string> map = new Dictionary<string, string>();
    }
}
