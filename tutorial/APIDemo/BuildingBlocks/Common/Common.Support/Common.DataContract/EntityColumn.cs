////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/4/2011 10:55:43 AM 
// Description: EntityColumn.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class EntityColumn
    {
        public EntityColumn() { }
        public EntityColumn(string columnName, Type dataType, bool unique = false)
        {
            this.DataType = dataType.FullName;
            this.ColumnName = columnName;
            this.Unique = unique;
        }
        [DataMember(Name = "Type")]
        [XmlAttribute("Type")]
        public  string DataType { get;  set; }

        public Type GetDataType() { return (DataType==null)?null:Type.GetType(DataType); }
        
        [DataMember(Name = "Name",IsRequired=true)]
        [XmlAttribute("Name")]
        public string ColumnName { get; set; }

        [DataMember(Name = "Unique")]
        [XmlAttribute("Unique")]
        public bool Unique { get; set; }

        [DataMember(Name = "Attributes", IsRequired = false)]
        [XmlAttribute("Attributes")]
        public string Attributes { get; set; }

        [DataMember(Name = "DefaultValue")]
        public object DefaultValue { get; set; }

        [XmlIgnore]
        public Action<object> ValidationMethod { set;get; }
    }
}
