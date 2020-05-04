////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/1/2008 9:41:08 AM 
// Description: BinaryTransfer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Support.Serializer
{
    public class BinaryTransfer:BaseTransfer
    {
        override public void Serialize(object it, Stream stream)
        {
            formatter.Serialize(stream, it);
        }
        public object Deserialize(Stream stream)
        {
            return formatter.Deserialize(stream);
        }

        override public void Serialize(object it,Type type, Stream stream)
        {
            Serialize(it, stream);
        }

        override public object Deserialize(Stream stream, Type type)
        {            
            return Deserialize(stream);
        }


        BinaryFormatter formatter = new BinaryFormatter();
    }
}
