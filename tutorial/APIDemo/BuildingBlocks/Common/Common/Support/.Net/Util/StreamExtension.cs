////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 1/6/2011 10:43:00 AM 
// Description: StreamExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System.IO;

namespace Common.Support.Net.Util
{
    static public class StreamExtension
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            // Use this method is used to read all bytes from a stream.
            using (MemoryStream tempStream = new MemoryStream())
            {
                //某些Stream例如GZipStream不提供Position property
                //stream.Position = 0;
                byte[] bytes = new byte[4096];
                int n;
                while ((n = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    tempStream.Write(bytes, 0, n);
                }
                return tempStream.ToArray();
            }
        }
    }
}
