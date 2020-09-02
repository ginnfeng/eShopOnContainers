////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/17/2009 5:34:49 PM 
// Description: KeyValuePairField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using Common.Support.Net.Util;
using Common.Support.Net.Proxy;


namespace Common.DataCore
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DictionaryField : ConverterFieldBase
    {
        public DictionaryField()
            : this(StringExtension.DefaultPairSymbol, StringExtension.DefaultSeparatorSymbol)
        {
        }
        public DictionaryField(char pairSymbol, char separatorSymbol)
        {
            this.pairSymbol = pairSymbol;
            this.separatorSymbol = separatorSymbol;
        }

        override public void Initializing()
        {
            if (string.IsNullOrEmpty(this.Content)) return;
            this.Content.ReadPairs(pair => ParseProperty(pair), pairSymbol, separatorSymbol);
            
        }
        private void UpdateContent()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var key in Keys)
            {
                stringBuilder.AddKeyValue(key, this[key], this.separatorSymbol, this.pairSymbol);
            }
            Content = stringBuilder.ToString();
        }
        public object this[string propertyKey]
        {
            get { return dictionary[propertyKey]; }
            set 
            { 
                dictionary[propertyKey] = (value == null) ? null : value.ToString();
                UpdateContent();
            }
        }
        private bool ParseProperty(KeyValuePair<string, string> pair)
        {
            dictionary[pair.Key] = pair.Value;
            return true;
        }

        public Dictionary<string, string>.KeyCollection Keys
        {
            get { return dictionary.Keys; }
        }
        public Dictionary<string, string>.ValueCollection Values
        {
            get { return dictionary.Values; }
        }
        public TEntity GenEntityProxy<TEntity>()
        {
            RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
            realProxy.GetPropertyEvent += new GetPropertyDelegate((methodInfo, propertyName) => this[propertyName].ToObject(methodInfo.ReturnType));
            realProxy.SetPropertyEvent += new SetPropertyDelegate((methodInfo, propertyName, value) => this[propertyName] = (value == null) ? null : value.ToString());
            return realProxy.Entity;
        }
        
        private char pairSymbol;
        private char separatorSymbol;
        
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();
    }
}
