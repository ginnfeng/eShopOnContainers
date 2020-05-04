////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/30/2008 10:36:28 AM 
// Description: GZipHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

using System.Text;
using System.IO;
using System.IO.Compression;
using Support.Net.Util;

namespace Support.Helper
{
    static public class GZipHelper
    {

        static public byte[] Compress(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream())
            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
            {
                gZipStream.Write(data, 0, data.Length);
                //不Close() Decompress會有問題                
                gZipStream.Close();
                return stream.ToArray();
            }
        }

        static public byte[] Compress(Stream stream)
        {
            byte[] data = stream.ReadAllBytes();
            return Compress(data);
        }


        static public byte[] Compress(string source)
        {
            return Compress(TextEncoding.GetBytes(source));
        }

        static public string CompressString(string source)
        {
            byte[] zipData = Compress(source);
            //用ToBase64String才能還原一致的byte[]
            return Convert.ToBase64String(zipData);
        }

        static public void CompressFile(string sourceFilePath, string targetPath)
        {
            using (FileStream inStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream outFileStream = new FileStream(targetPath, FileMode.Create))
            {
                byte[] buffer = Compress(inStream);
                outFileStream.Write(buffer, 0, buffer.Length);
                outFileStream.Close();
                inStream.Close();
            }
            #region 網路範例
            /*
		    byte[] buffer = ReadAllBytes(fileStream);
            //byte[] buffer = new byte[fileStream.Length];
            //fileStream.Read(buffer, 0, buffer.Length);
            //fileStream.Close();

            FileStream outFileStream = new FileStream(targetPath, FileMode.Create);
            GZipStream compressedStream = new GZipStream(outFileStream, CompressionMode.Compress);
            compressedStream.Write(buffer, 0, buffer.Length);
            compressedStream.Close();
            outFileStream.Close();
            */
            #endregion

        }

        /// <summary>
        /// TextEncoding.GetString(bytes) 可轉string
        /// </summary>
        /// <param name="zipData"></param>
        /// <returns></returns>
        static public byte[] Decompress(byte[] zipData)
        {
            using (GZipStream gZipStream = new GZipStream(new MemoryStream(zipData), CompressionMode.Decompress))
            {
                return gZipStream.ReadAllBytes();
            }

        }

        static public byte[] Decompress(Stream stream)
        {
            byte[] data = stream.ReadAllBytes();
            return Decompress(data);
        }

        static public string DecompressString(string source)
        {
            byte[] zipData = Convert.FromBase64String(source);
            return TextEncoding.GetString(Decompress(zipData));
        }

        static public void DecompressFile(string sourceFilePath, string targetPath)
        {
            using (FileStream inStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream outFileStream = new FileStream(targetPath, FileMode.Create))
            {
                byte[] buffer = Decompress(inStream);
                //string s=TextEncoding.GetString(buffer);
                outFileStream.Write(buffer, 0, buffer.Length);
                outFileStream.Close();
                inStream.Close();
            }
        }
        public static void CopyStream(Stream src, Stream target)
        {
            src.Position = 0;
            target.Position = 0;
            var buffer = src.ReadAllBytes();
            target.Write(buffer, 0, buffer.Length);
            target.Flush();
        }
        

        static public Encoding TextEncoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}
