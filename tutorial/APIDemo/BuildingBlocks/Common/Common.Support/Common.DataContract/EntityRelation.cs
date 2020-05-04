////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/11/2011 1:54:47 PM 
// Description: ForeignKey.cs  
// Revisions  :           
//samples:產生selectKey 的方式有二
////一.從字串轉來
//string selectKeyJsonString=  {"Keys":["id_1"],"NS":"Demo","Table":"CTenant","Version":null}
//selectKey =Singleton<JsonTransfer>.Instance.ToObject<EntityRelation>( selectKeyJsonString);

////二.程式自兜
//EntityRelation selectKey = new EntityRelation();
//            selectKey.Keys.Add ("id_1");
//            selectKey.TableName = " CTenant ";
//            selectKey.Namespace = "Demo";

//EntityTableProxy<IYourEntity> entityProxy;
//if (!CacheSourceProvider.TryGetTable<IYourEntity>(selectKey, out entityProxy))
//{

//            IYourEntity entity= entityProxy.First();
//} 		
// **************************************************************************** 


using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public partial class EntityRelation : IEntityTableInfo
    {        
        public EntityRelation()
        {                
        }
        /// <summary>
        /// SetAsSelectAll
        /// </summary>        
        public EntityRelation(Type type)
            : this(type.Namespace,type.Name,null)
        {    
        }
        public EntityRelation(string ns,string name,params string[] keys)
        {
            Namespace = ns;
            TableName = name;
            if(keys!=null)  Keys.AddRange(keys);
        }
        [XmlIgnore]
        public string FullName
        {
            get { return Namespace + "." + TableName; }
        }
        [DataMember(Name = "NS")]
        [XmlAttribute("NS")]
        public string Namespace { get; set; }

        [DataMember(Name = "Table")]
        [XmlAttribute("Table")]
        public string TableName { get; set; }

        [DataMember(Name = "Version")]
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [DataMember(Name = "Keys")]
        public List<string> XmlKeys 
        {
            get { return Keys.ToList(); }
            set 
            { 
                Keys.Clear();
                Keys.AddRange(value); 
            }
        }

        public FilterCollection Keys 
        { 
            get
            {
                if (keys == null)
                    keys = new FilterCollection();
                return keys; 
            }
            private set { keys = value; }
        }

        public bool MatchKey(string rowKey,EntityRow row)
        {
            return Keys.Match
                (
                    (filter) => { return (filter.Mode == Filter.FilterMode.Key) ? filter.Match(rowKey) : filter.Match(row[filter.ColumnName]); }
                );
            
        }
        
        public void SetAsSelectAll()
        {            
            Keys.SetAsSelectAll();
        }
        public bool IsSelectAll()
        {
            return Keys.IsSelectAll;
        }
        public bool IsEmptyKeys()
        {
            return Keys.Count == 0;
        }
       
        public void SetParent(string ns, string table)
        {
            ParentNS = ns;
            ParentTable = table;
        }
        public EntityRelation Clone()
        {            
            var cloneObject = new EntityRelation() { Namespace = Namespace, TableName = TableName, Version = Version, ParentNS = ParentNS, ParentTable = ParentTable };
            cloneObject.Keys=Keys.Clone();
            return cloneObject;
        }
        
        internal string ParentNS { get; private set; }
        internal string ParentTable { get; private set; }
        private FilterCollection keys;
    }
    
}
