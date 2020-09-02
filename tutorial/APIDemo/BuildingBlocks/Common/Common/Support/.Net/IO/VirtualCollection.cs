////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/9/2009 5:45:28 PM 
// Description: VirtualCollection.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.Support.Serializer;
using System.IO;
using Common.Support.Helper;

namespace Common.Support.Net.IO
{
    [Serializable]    
    public class IndexBlock
    {        
        public object Key { get; set; }        
        public long StartPos { get; set; }        
        public long EndPos { get; set; }        
        internal bool IsDel { get; set; }
    }

    public abstract class VirtualCollection<T>:IDisposable
    {        
        protected VirtualCollection(string tempFile,BaseTransfer transfer,FileMode fileMode)
            : this(new FileStream(tempFile, fileMode), transfer)
        {              
        }

        protected VirtualCollection(FileStream tempStream, BaseTransfer transfer)
        {            
            Transfer = transfer;
            this.StoreStream = tempStream;
            this.StoreStream.Position = this.StoreStream.Length;
            KeySelector = it => it.GetHashCode();
            Init();
        }
        public void AddRange(IEnumerable<T> list)
        {
            
            AddRange(list, KeySelector);
        }
        public void AddRange(IEnumerable<T> list, Func<T, object> keySelector)
        {
            foreach (var item in list)
            {
                Add(item, keySelector);
            }
        }
        public void Add(T item)
        {
            Add(item, KeySelector);
        }
        virtual public void Add(T item,Func<T,object> keySelector)
        {   
            long startPosition = StoreStream.Length;
            Transfer.Serialize(item, StoreStream);
            long endPosition = StoreStream.Position;
            RegistIndex(keySelector(item), startPosition, endPosition);
        }
        virtual public void Clear()
        {
            StoreStream.Close();
            File.Delete(StoreStream.Name);
            StoreStream = new FileStream(StoreStream.Name,FileMode.Create);
        }
        public int BufferSize { get; set; }
        public Func<T, object> KeySelector { get; set; }
        public BaseTransfer Transfer { get;  set; }

        virtual public void CopyTo(VirtualCollection<T> target)            
        {
            Flush();
            GZipHelper.CopyStream(StoreStream,target.StoreStream);
            IndexBlock[] indexList=new IndexBlock[Index.Count];
            Index.CopyTo(indexList);
            target.Index = new List<IndexBlock>(indexList);
            target.Transfer = Transfer;
        }

        public T Find(Predicate<IndexBlock> selector)
        {
            IndexBlock indexBlock = Index.Find(selector);
            if (indexBlock == null) return default(T);
            return Find(indexBlock);
        }
        public T Find(IndexBlock indexBlock)
        {
            StoreStream.Position = indexBlock.StartPos;
            int count = (int)(indexBlock.EndPos - indexBlock.StartPos);
            byte[] buffer = new byte[count];
            StoreStream.Read(buffer, 0, count);
            using (var stream = new MemoryStream(buffer))
            {
                return Transfer.Deserialize<T>(stream);
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (IndexBlock indexBlock in Index)
            {
                yield return Find(indexBlock);
            }
        }
        public int Count { get { return Index.Count; } }
        
        abstract protected void DoRegistIndex(IndexBlock indexBlock);
        abstract protected void Init();
        virtual public void Flush()
        {
            StoreStream.Flush();
        }

        private void RegistIndex(object key, long startPosition, long endPosition)
        {
            IndexBlock block = Index.Find(it => it.Key == key);
            if (block == null)
            {
                block = new IndexBlock() { Key = key, StartPos = startPosition, EndPos = endPosition };
                DoRegistIndex(block);
                Index.Add(block);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                    StoreStream.Close();
                    StoreStream.Dispose();  
            }           
            disposed = true;
        }

        private bool disposed;
        protected List<IndexBlock> Index { get; set; }
        protected FileStream StoreStream { get; set; }
                
    }
}
