////*************************Copyright © 2008 Feng 豐**************************
// Created    : 10/1/2008 9:43:16 AM
// Description: BaseTransfer.cs
// Revisions  :
// ****************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if !SILVERLIGHT

using Support.Helper;

#endif

using Support.Net.Util;

namespace Support.Serializer
{
    abstract public class BaseTransfer
    {
        public BaseTransfer()
        {
            TheEncoding = Encoding.UTF8;
        }

        //public BaseTransfer(IEnumerable<Type> knownTypes)
        //{
        //}
#if !SILVERLIGHT

        public byte[] Compress<T>(T it)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(it, stream);
                stream.Flush();
                stream.Position = 0;
                return GZipHelper.Compress(stream);
            }
        }

        public T Decompress<T>(byte[] zipBuffer)
        {
            byte[] buffer = GZipHelper.Decompress(zipBuffer);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return Deserialize<T>(stream);
            }
        }

        public void SaveZip<T>(T it, string filePath)
        {
            byte[] buffer = Compress(it);
            using (FileStream outFileStream = new FileStream(filePath, FileMode.Create))
            {
                outFileStream.Write(buffer, 0, buffer.Length);
                outFileStream.Close();
            }
        }

        public T LoadZip<T>(string filePath)
        {
            using (FileStream inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = inStream.ReadAllBytes();
                T it = Decompress<T>(buffer);
                inStream.Close();
                return it;
            }
        }

#endif

        public void Save<T>(T it, string filePath, FileMode fileMode = FileMode.Create)
        {
            using (FileStream outFileStream = new FileStream(filePath, fileMode))
            {
                Serialize(it, outFileStream);
                outFileStream.Close();
            }
        }

        public T Load<T>(string filePath)
        {
            using (FileStream inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MemoryStream tempStream = new MemoryStream(inStream.ReadAllBytes()))
            {
                T it = Deserialize<T>(tempStream);
                inStream.Close();
                tempStream.Close();
                return it;
            }
        }

        public byte[] ToBytes<T>(T it)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(it, stream);
                return stream.ToArray();
            }
        }

        public T ToObject<T>(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return Deserialize<T>(stream);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        public T Serialize<T>(object it)
            where T : Stream, new()
        {
            T stream = new T();
            Serialize(it, it.GetType(), stream);
            return stream;
        }

        virtual public void Serialize(object it, Stream stream)
        {
            Serialize(it, it.GetType(), stream);
        }

        virtual public T AutoToObject<T>(object text, bool asFieldMember = false)
        {
            if (text == null) return default(T);

            return typeof(T).IsAssignableFrom(text.GetType()) ? (T)text : ToObject<T>(text.ToString());
        }

        virtual public T ToObject<T>(string text)
        {
            return (T)ToObject(text, typeof(T), RetriveTextEncoding(text));
        }

        public T ToObject<T>(string text, Encoding encoding)
        {
            return (T)ToObject(text, typeof(T), encoding);
        }

        public object ToObject(string text, Type type, Encoding encoding)
        {
            Byte[] buffer = encoding.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return Deserialize(stream, type);
            }
        }

        public string ToText<T>(T it)
        {
            return ToText<T>(it, TheEncoding);
        }

        public string ToText<T>(T it, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(it, it.GetType(), ms);
                return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Position);
            }
        }

        public Encoding TheEncoding { get; set; }

        abstract public void Serialize(object it, Type type, Stream stream);

        abstract public object Deserialize(Stream stream, Type type);

        virtual protected Encoding RetriveTextEncoding(string text)
        {
            return TheEncoding;
        }

        public IEnumerable<Type> KnownTypes { get; set; }
    }
}