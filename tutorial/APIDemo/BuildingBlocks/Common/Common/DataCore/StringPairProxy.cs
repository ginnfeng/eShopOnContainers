////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/31/2011 3:07:05 PM 
// Description: StringPairProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using Common.Support.Net.Proxy;
using System.Reflection;
using System.Text.RegularExpressions;
using Common.Support.Net.Util;
using Common.Support;
namespace Common.DataCore
{

    public class StringPairProxy
    {
        public StringPairProxy(string stringPair = "")
            : this(stringPair, StringExtension.DefaultSeparatorSymbol, StringExtension.DefaultPairSymbol)
        {
        }
        public StringPairProxy(string stringPair, char separatorSymbol, char pairSymbol)
        {
            this.StringPair = stringPair;
            this.separatorSymbol = separatorSymbol;
            this.pairSymbol = pairSymbol;
        }
        public event Action<string> StringPairUpdatedEvent;
        public string StringPair
        {
            get { return stringPair; }
            set
            {
                if (stringPair == value) return;
                stringPair = (value == null) ? "" : value;
                if (StringPairUpdatedEvent != null)
                    StringPairUpdatedEvent(stringPair);
            }
        }

        public object this[string propertyKey]
        {
            get { return GetPropertyValue(null, propertyKey); }
            set { SetPropertyValue(null, propertyKey, value); }
        }
        public TEntity GenEntityProxy<TEntity>()
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += GetPropertyValue;
            realProxy.SetPropertyEvent += SetPropertyValue;
            return realProxy.Entity;
        }

        private void SetPropertyValue(MethodInfo methodInfo, string propertyName, object value)
        {
            Regex regex = GenRegex(propertyName);
            var valueString = (value == null) ? "" : value.ToString();
            bool isMatch = false;
            StringPair = regex.Replace(StringPair, match => { isMatch = true; return match.Groups[1].Value + valueString; });
            if (!isMatch)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(StringPair)
                    .AddKeyValue(propertyName, value, this.separatorSymbol, this.pairSymbol);
                StringPair = sb.ToString();
            }
        }

        private object GetPropertyValue(MethodInfo methodInfo, string propertyName)
        {
            Regex regex = GenRegex(propertyName);
            var match = regex.Match(StringPair);
            object rlt= (match.Success) ? match.Groups[3].Value : null;
            return (methodInfo != null) ? rlt.ToObject(methodInfo.ReturnType) : rlt.ToObject(typeof(string));
        }

        private Regex GenRegex(string key)
        {
            string matchString = CommonExtension.StringFormat("(({1}|^){0}=)([^{1}{2}]{{0,}})", key, separatorSymbol, pairSymbol);
            return new Regex(matchString);
        }

        private string stringPair="";

        private char separatorSymbol = StringExtension.DefaultSeparatorSymbol;
        private char pairSymbol = StringExtension.DefaultPairSymbol;
    }
}
