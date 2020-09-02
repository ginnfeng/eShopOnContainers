////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/17/2011 11:26:36 AM 
// Description: EntityTableInfo.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class EntityTableInfo : IEntityTableInfo
    {
        public EntityTableInfo()
        {
        }
        public EntityTableInfo(string ns, string name, string ver)
        {
            Namespace = ns;
            TableName = name;
            Version = ver;
        }

        #region IEntityTableInfo Members
        
        [DataMember]
        public string Namespace { get; set; }
        
        [DataMember]
        public string TableName { get; set; }

        #endregion

        [DataMember]
        public string Version { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Namespace) || string.IsNullOrEmpty(TableName);
        }
    }
}
