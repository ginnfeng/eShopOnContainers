////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/30/2011 4:59:19 PM 
// Description: EntityDigest.cs  
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
    public class EntityDigest
    {
        public EntityDigest()
        {
        }
        [DataMember(Name = "Key")]
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [DataMember(Name = "Ver")]
        [XmlAttribute("Ver")]
        public string Version { get; set; }

        [DataMember(Name = "Digest")]
        public string Digest { get; set; }


        public override bool Equals(object it)
        {
            if (it == null || GetType() != it.GetType())
                return false;
            if (object.ReferenceEquals(this, it))
                return true;
            return GetUID() == ((EntityDigest)it).GetUID();
        }

        public override int GetHashCode()
        {
            if (Key == null)
                return base.GetHashCode();
            return GetUID().GetHashCode();
        }
        private string GetUID()
        {
            return Key + "_" + Version;
        }
    }
}
