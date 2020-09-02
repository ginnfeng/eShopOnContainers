////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/23/2009 3:59:17 PM 
// Description: ListField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Common.DataContract;
using Common.Support;

namespace Common.DataCore
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ListField<T> : ConverterFieldBase
    {
      
        public void AddItem(T item)
        {
            items.Add(item);
            UpdateContent();
        }
        public bool RemoveItem(T item)
        {
            bool isTrue=items.Remove(item);
            if(isTrue) UpdateContent();
            return isTrue;
        }
        public  ReadOnlyCollection<T>  Items 
        {
            get 
            {
                return items.AsReadOnly(); 
            }
        }
        override public void Initializing()
        {
            if (string.IsNullOrEmpty(this.Content) ) return;
            items.Clear();
            Match match = priceRegex.Match(this.Content);
            if (match.Success)
            {
                foreach (Capture item in match.Groups[2].Captures)
                {
                    items.Add(CommonExtension.ToObject<T>(item.Value));
                }
            }
        }
        private void UpdateContent()
        {
            StringBuilder sb = new StringBuilder();
                items.ForEach(item => { if (sb.Length != 0) sb.Append(','); sb.Append(item.ToString()); });
            Content=sb.ToString();
        }
        private List<T> items= new List<T>();
        static Regex priceRegex = new Regex(@"[[\s\t]{0,}(([^,]{1,})[,\s\t]{0,})*");
    }

    public class NSGroupList : ListField<string>
    {
        override public void Initializing()
        {
            base.Initializing();
            Column.GetAttributesEntity().FieldType = EntityFieldType.NSGrp;
        }
    }
}
