////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/21/2016 2:59:28 PM
// Description: SheetsEntityHelper.cs
// Revisions  :
// ****************************************************************************
using Google.Apis.Sheets.v4.Data;
using Support.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Support.Open.Google
{
    public class SheetsEntityHelper
    {
        public SheetsEntityHelper()
        {
            HasColumnNames = true;
            ColumnRowIdx = 1;//zero base空出2個row，其一當Column Names
        }

        private bool hasColumnNames;

        public bool HasColumnNames
        {
            get { return hasColumnNames; }
            set
            {
                hasColumnNames = value;
                if (!hasColumnNames) ColumnRowIdx = 0;
            }
        }

        /// <summary>
        /// Zero based
        /// </summary>
        public int ColumnRowIdx { get; set; }

        public List<T> ToEntities<T>(IList<IList<object>> values)
            where T : new()
        {
            if (values == null) return null;
            Type type = typeof(T);
            Dictionary<string, IndexingAttribute> attributeMap;
            int maxIdx = BuildAttriMap(type, out attributeMap);
            List<T> entities = new List<T>();
            List<string> colNames = new List<string>();
            for (int i = ColumnRowIdx; i < values.Count; i++)
            {
                if (i == ColumnRowIdx && HasColumnNames)
                {
                    values[i].ToList().ForEach(it => colNames.Add(it.ToString()));
                    continue;
                }
                var row = values[i];
                T entity = new T();
                for (int j = 0; j < row.Count; j++)
                {
                    var propInfo = GetPropertyInfo(j, colNames, attributeMap);
                    if (propInfo == null) continue;
                    var value = CommonExtension.ToObject(row[j], propInfo.PropertyType);
                    propInfo.SetValue(entity, value);
                }
                entities.Add(entity);
            }
            return entities;
        }

        public List<IList<object>> ToValues<T>(IList<T> entities)
        {
            Type type = typeof(T);
            Dictionary<string, IndexingAttribute> attributeMap;
            int maxIdx = BuildAttriMap(type, out attributeMap);
            var valueList = new List<IList<object>>();
            for (int i = 0; i <= ColumnRowIdx; i++)
            {
                List<object> cols = (i < ColumnRowIdx) ?
                    new List<object>()
                    : BuildValues(maxIdx + 1, ref attributeMap, key => attributeMap[key].Id ?? key);
                valueList.Add(cols);
            }
            //if (HasColumnNames)
            //{
            //    var cols = BuildValues(maxIdx + 1, ref attributeMap, key => attributeMap[key].Id ?? key);
            //    valueList.Add(cols);
            //}
            for (int i = 0; i < entities.Count; i++)
            {
                var values = BuildValues(maxIdx + 1, ref attributeMap, key => type.GetProperty(key).GetValue(entities[i]));
                valueList.Add(values);
            }
            return valueList;
        }

        private int BuildAttriMap(Type type, out Dictionary<string, IndexingAttribute> attributeMap)
        {
            attributeMap = new Dictionary<string, IndexingAttribute>();
            int maxIdx = 0;
            foreach (var propertyInfo in type.GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(IndexingAttribute), true);
                if (attributes.Length > 0)
                {
                    var attri = (IndexingAttribute)attributes[0];
                    attri.OriginalPropertyInfo = propertyInfo;
                    if (attri.Idx > maxIdx) maxIdx = attri.Idx;
                    attributeMap[propertyInfo.Name] = attri;
                }
            }
            return maxIdx;
        }

        private List<object> BuildValues(int size, ref Dictionary<string, IndexingAttribute> attributeMap, Func<string, object> func)
        {
            var values = new List<object>(new object[size]);
            foreach (var attri in attributeMap)
            {
                values[attri.Value.Idx] = func(attri.Key);

                //var cellValue = func(attri.Key);
                //string sValue = cellValue as string;
                //if (!string.IsNullOrEmpty(sValue) && sValue[0] == '=')
                //{
                //    var formulaValue = new CellData() { Hyperlink = sValue };
                //    values[attri.Value.Idx] = formulaValue;
                //}
                //else
                //{
                //    values[attri.Value.Idx] = cellValue;
                //}

            }
            return values;
        }

        static private PropertyInfo GetPropertyInfo(int colIdx, List<string> colNames, Dictionary<string, IndexingAttribute> attributeMap)
        {
            try
            {
                if (colNames != null && colNames.Count > 0 && colIdx < colNames.Count)
                {
                    IndexingAttribute attri;
                    if (attributeMap.TryGetValue(colNames[colIdx], out attri))
                        return attri.OriginalPropertyInfo;
                }
                foreach (var attri in attributeMap)
                {
                    if (colIdx == attri.Value.Idx)
                        return attri.Value.OriginalPropertyInfo;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}