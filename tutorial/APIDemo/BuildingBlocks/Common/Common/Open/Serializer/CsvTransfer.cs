////*************************Copyright © 2013 Feng 豐**************************
// Created    : 10/6/2016 9:26:27 AM
// Description: CsvTransfer.cs
// Revisions  :
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using Common.Support.Net.Util;
using Common.Support.Helper;
using Common.Support.Serializer;
using System.Linq;
using System.Globalization;
using Common.Support;

namespace Common.Open.Serializer
{
    public class CsvTransfer : BaseTransfer
    {
        
        public event Func<object> CreateInstanceEvent;
        public event Func<string,bool> LineStrinCheckEvent;
        public event Func<string, string> LineStrinReplaceEvent;
        public Type ObjectType { get; set; }
        public string DateFormat { get; set; }
        public override object Deserialize(Stream stream, Type type)
        {
            var list = (IList)Activator.CreateInstance(type);
            var typeArgument = type.GetGenericArguments()[0];
            var indexingAttributes = TakeIndexingAttributesByIdx(typeArgument);
            bool isAllHeaderMatchPropertyName = false;
            var head = new List<string>();
            
            using (var reader = new StreamReader(stream, TheEncoding))
            {                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    
                    if (head.Count == 0)
                    {
                        if(!line.Contains(symbol)) continue;
                        line.Split(str => { head.Add(str); return true; }, symbol);
                        isAllHeaderMatchPropertyName = head.TrueForAll(it => typeArgument.GetProperty(it) != null);
                        if (!isAllHeaderMatchPropertyName && indexingAttributes == null)
                            throw new FieldAccessException("Deserialize: CSV Columns not match object propertys!");
                        continue;
                    }
                    if (LineStrinCheckEvent != null && !LineStrinCheckEvent(line)) continue;
                    if (LineStrinReplaceEvent != null )
                        line = LineStrinReplaceEvent(line);
                    var item = (CreateInstanceEvent != null) ? CreateInstanceEvent() : Activator.CreateInstance(typeArgument);
                    var values = new List<object>();
                    line.Split(str => { values.Add(str); return true; }, symbol);
                    int colNum = (!isAllHeaderMatchPropertyName) ? indexingAttributes.Count : head.Count;
                    for (int i = 0; i < colNum; i++)
                    {
                        var protertyInfo = (!isAllHeaderMatchPropertyName) ? 
                            indexingAttributes[i].OriginalPropertyInfo
                            : typeArgument.GetProperty(head[i]);
                        if (protertyInfo != null && i<values.Count)
                        {
                            protertyInfo.SetValue(item, CommonExtension.ToObject(values[i], protertyInfo.PropertyType));
                        }
                    }
                    list.Add(item);
                }

                return list;
            }
        }

        public override void Serialize(object it, Type type, Stream stream)
        {
            var objs = (IList)it;
            var indexingAttributes = TakeIndexingAttributes(it.GetType().GetGenericArguments()[0]);
            using (var writer = new StreamWriter(stream, TheEncoding, 120, true))
            {
                StringBuilder csvSb = new StringBuilder();

                foreach (var item in objs)
                {
                    if (csvSb.Length == 0 && stream.Position == 0)
                    {
                        ProcessPorperty(item, ref csvSb, indexingAttributes, pInfo => pInfo.Id);
                        csvSb.AppendLine();
                    }
                    StringBuilder itemSb = new StringBuilder();
                    ProcessPorperty(item, ref itemSb, indexingAttributes, pInfo => pInfo.OriginalPropertyInfo.GetValue(item));
                    csvSb.AppendLine(itemSb.ToString());
                }

                writer.Write(csvSb.ToString());
                writer.Flush();
            }
        }

        private void ProcessPorperty<T>(T it, ref StringBuilder sb, IndexingAttribute[] indexingAttributes, Func<IndexingAttribute, object> act)
        {
            for (int i = 0; i < indexingAttributes.Length; i++)
            {
                var pInfo = indexingAttributes[i];
                if (sb.Length != 0 || i > 0) sb.Append(symbol);
                var itValue = act(pInfo);
                if (itValue == null) sb.Append("");
                else
                {                   
                    var str=(DateFormat != null && typeof(DateTime).IsInstanceOfType(itValue)) ? ((DateTime)itValue).ToString(DateFormat, CultureInfo.InvariantCulture) : itValue.ToString();
                    sb.Append(str);
                }                
            }
        }
        private List<IndexingAttribute> TakeIndexingAttributesByIdx(Type typeArgument)
        {
            var idxAttributes=TakeIndexingAttributes(typeArgument);
            if (idxAttributes.Length == 0) return null;
            //var rlt=new List<IndexingAttribute>(idxAttributes);
            return idxAttributes.OrderBy(item=>item.Idx).ToList();
            
        }
        private List<PropertyInfo> GetAllProperties(Type typeArgument)
        {
            var propertyInfos = new List<PropertyInfo>();
            
            propertyInfos.AddRange(typeArgument.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            
            var baseInterfaces = typeArgument.GetInterfaces();
            foreach (var baseInterface in baseInterfaces)
            {
                var parentProps=GetAllProperties(baseInterface);
                foreach (var parentProp in parentProps)
                {
                    if (propertyInfos.Find(it => it.Name == parentProp.Name)!=null) continue;
                }
            }
            //if (baseInterfaces.Length > 0)
            //{
            //    baseInterfaces.ForEach(it => propertyInfos.AddRange(GetAllProperties(it)));
            //}
            //var distinctCompare = new GenericEqualityCompare<PropertyInfo>(it => it.Name);
            //propertyInfos = propertyInfos.Distinct(distinctCompare).ToList();
            return propertyInfos;
        }
        private IndexingAttribute[] TakeIndexingAttributes(Type typeArgument)
        {
           
            var properties = GetAllProperties(typeArgument);
            //var Properties = typeArgument.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //IndexingAttribute[] indexs = new IndexingAttribute[properties.Count];
            var indexs = new List<IndexingAttribute>();
            
            int idx = 0;
            foreach (var prop in properties)
            {
                var cAttri = prop.GetCustomAttribute<IndexingAttribute>(true);
                cAttri = cAttri ?? new IndexingAttribute();
                cAttri.Id = cAttri.Id ?? prop.Name;
                cAttri.OriginalPropertyInfo = prop;
                if (!cAttri.IsIdxAssigned())
                    cAttri.Idx = idx;                
                indexs.Add(cAttri);
                idx++;
            }
            return indexs.ToArray();
        }

        private const char symbol = ',';
    }
}