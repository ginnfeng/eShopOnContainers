////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/26/2011 10:16:09 AM 
// Description: SVCParameter.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class CmdParameter
    {
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public string Serializer { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
