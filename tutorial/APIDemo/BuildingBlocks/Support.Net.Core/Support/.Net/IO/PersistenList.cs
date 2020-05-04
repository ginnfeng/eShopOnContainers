////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/25/2008 2:30:35 PM 
// Description: VirtualSet.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.IO;
using Support.Serializer;
using Support.Helper;

namespace Support.Net.IO
{
    [Serializable]
    public class PersistenList<T> : VirtualCollection<T>
    {        
        public PersistenList(string tempFile)
            : base(tempFile, new BinaryTransfer(), FileMode.OpenOrCreate)
        {
            
        }

        public PersistenList(FileStream tempStream)
            : base(tempStream, new BinaryTransfer())
        {
            
        }
        public PersistenList(string tempFile, BaseTransfer transfer)
            : base(tempFile, transfer, FileMode.OpenOrCreate)
        {

        }

        public PersistenList(FileStream tempStream, BaseTransfer transfer)
            : base(tempStream, transfer)
        {

        }
        
        override public void Flush()
        {
            base.Flush();
            indexStream.Flush();
        }
        
        override public void CopyTo(VirtualCollection<T> target)
        {
            if (!this.GetType().IsAssignableFrom(target.GetType()))
            {
                base.CopyTo(target);
                return;
            }
            PersistenList<T> targetPersisten=(PersistenList<T>)target;
            GZipHelper.CopyStream(indexStream, targetPersisten.indexStream);            
            base.CopyTo(target);
        }

        public IList<IndexBlock> Indexer 
        {
            get
            {
                return Index.AsReadOnly();
            }
        }        
        
        override public void Clear()
        {
            base.Clear(); 
            indexStream.Close();
            File.Delete(indexStream.Name);
            Init();
        }
        protected override void DoRegistIndex(IndexBlock indexBlock)
        {
            indexTransfer.Serialize(indexBlock, indexStream);
        }
        override protected void Init()
        {
            string indexPath = TakeIndexPath();
            Index = new List<IndexBlock>();
            if (File.Exists(indexPath))
            {
                indexStream = new FileStream(indexPath, FileMode.Open);
                var length = indexStream.Length;
                if (length > 0)
                {
                    while (indexStream.Position != length)
                    {
                        var indexBlock = indexTransfer.Deserialize<IndexBlock>(indexStream);
                        Index.Add(indexBlock);
                    }
                }
              
                return;
            }
            indexStream = new FileStream(indexPath, FileMode.Create);     
           
        }
        private string TakeIndexPath()
        {
            return StoreStream.Name + ".idx";
        }
       
        override protected  void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                base.Dispose(disposing);
                indexStream.Close();
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;        
        private FileStream indexStream;
        private BaseTransfer indexTransfer=new BinaryTransfer();
    }
}
