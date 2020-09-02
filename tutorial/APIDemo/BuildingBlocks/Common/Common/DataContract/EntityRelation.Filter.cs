////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/20/2011 11:32:25 AM 
// Description: EntityRelation.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Common.DataContract
{
    public partial class EntityRelation : IEntityTableInfo
    {
        public class FilterCollection
        {
            public FilterCollection()
            {                
            }
            public bool Match(Func<Filter,bool> func)
            {
                foreach (var filter in filterMap.Values)
                {
                    if (func(filter)) return true;
                }
                return false;
            }
            public void SetAsSelectAll()
            {
                Add(Filter.selectAllSymbol.ToString());
            }
            public Filter Add(object filterExp)
            {
                return (filterExp == null) ? null : Add(filterExp.ToString());
            }
            public Filter Add(string filterExp)
            {
                if (IsSelectAll) return null;
                return DoAdd(filterExp);                
            }
            public Filter DoAdd(string filterExp)
            {
                var filter = Filter.Create(filterExp);
                if (filter == null) return null;
                if (filter.Mode == Filter.FilterMode.All)                
                    Clear();
                filterMap[filterExp] = filter;
                return filter; 
            }
            public void AddRange(IEnumerable<string> filterExps)
            {
                if (IsSelectAll) return;
                foreach (var filterExp in filterExps)
                {
                    var filter = Add(filterExp);
                    if (filter.Mode == Filter.FilterMode.All)
                        break;
                }
            }
            public int Count { get { return filterMap.Count; } }
             public bool Remove(string key)
            {
                if (IsSelectAll) return true;
                return filterMap.Remove(key);
             }
        
            public bool Contains(string filterExp)
            {
                return filterMap.ContainsKey(filterExp);
            }
            public bool IsSelectAll 
            {
                get
                {
                    if (filterMap.Values.Count > 0)
                        return filterMap.Values.First().Mode == Filter.FilterMode.All;
                    return false;
                }
            }
            public List<string> ToList()
            {                
                return new List<string>(filterMap.Keys);                
            }
            public void Clear()
            {
                filterMap.Clear();
            }
            public FilterCollection Clone()
            {
                var filterCollection = new FilterCollection();
                foreach (var pair in filterMap)
	            {
		            filterCollection.filterMap[pair.Key]=pair.Value.Clone();
	            }
                return filterCollection;
            }
            private Dictionary<string,Filter> filterMap = new Dictionary<string,Filter>();
        }

        public class Filter
        {
            public enum FilterMode
            {
                All,
                Key,
                Like,
                EQ
            }
            public Filter(string filterExp)
            {
                FilterExp = filterExp;
            }
            public string FilterExp { get; set; }
            public FilterMode Mode { get; set; }
            public string ColumnName { get; set; }
            public string Key { get; set; }

            public Filter Clone()
            {
                return new Filter(FilterExp) { ColumnName = ColumnName, Key = Key, Mode = Mode };
            }

            public bool Match(object target)
            {
                return (target == null) ? false : Match(target.ToString());
            }
            public bool Match(string target)
            {
                switch (Mode)
                {
                    case FilterMode.EQ:
                    case FilterMode.Key:
                        return Key.Equals(target);
                    case FilterMode.Like:
                        Regex regex = new Regex(Key);
                        return regex.Match(target).Success;
                    default:
                        return true;
                }
            }

            static public  Filter Create(string keyExp)
            {
                if (string.IsNullOrEmpty(keyExp))
                    return null;
                Filter filter = new Filter(keyExp) { Mode = FilterMode.Key, Key = keyExp };
                switch (keyExp[0])
                {
                    case filterSymbol:
                        var match = filterRegex.Match(keyExp);
                        if (match.Success)
                        {
                            filter.ColumnName = match.Groups[1].Value;
                            if (!string.IsNullOrEmpty(match.Groups[3].Value))
                            {
                                filter.Key = match.Groups[3].Value;
                                filter.Mode = FilterMode.EQ;
                            }
                            else if (!string.IsNullOrEmpty(match.Groups[4].Value))
                            {
                                filter.Key = match.Groups[4].Value;
                                filter.Mode = FilterMode.Like;
                            }
                        }

                        break;
                    case selectAllSymbol:
                        filter.Mode = FilterMode.All;
                        break;
                    default:
                        break;
                }
                return filter;
            }
            

            //#([^#=]*)(=(.*)|\.Like[\s\(]{1,}(.*)\))
            private static readonly Regex filterRegex = new Regex(filterSymbol + "([^#=]*)(=(.*)|\\.Like[\\s\\(]{1,}(.*)\\))", RegexOptions.IgnoreCase);
            internal const char selectAllSymbol = '*';
            private const char filterSymbol = '#';
        };
    }
}
