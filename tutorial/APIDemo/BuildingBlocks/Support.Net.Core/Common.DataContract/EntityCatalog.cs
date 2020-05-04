////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/30/2011 2:04:36 PM 
// Description: EntityCatalog.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Common.DataContract
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class EntityCatalog
    {
        public EntityCatalog()
        {
            Digests = new List<EntityDigest>();
        }
        [DataMember(Name = "NS")]
        [XmlAttribute("NS")]
        public string Name { get; set; }

        [DataMember(Name = "PID")]
        [XmlAttribute("PID")]
        public string ParentId { get; set; }

        [DataMember]
        public List<EntityDigest> Digests { get; set; }

        [DataMember]
        public List<EntityCatalog> SubCatalogs { get; set; }
    }
}
