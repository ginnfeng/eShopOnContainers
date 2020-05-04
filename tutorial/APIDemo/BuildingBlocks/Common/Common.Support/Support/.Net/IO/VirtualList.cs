////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/25/2008 2:30:35 PM 
// Description: VirtualSet.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.IO;
using Support.Serializer;

namespace Support.Net.IO
{


    [Serializable]
    public class VirtualList<T> : VirtualCollection<T>
    {
        public VirtualList()
            :this(Guid.NewGuid().ToString()+".tmp")
        {
        }
        public VirtualList(string tempFile)
            : base(tempFile, new BinaryTransfer(), FileMode.Create)
        {            
            
        }
        public VirtualList(FileStream tempStream)
            : base(tempStream, new BinaryTransfer())
        {            
        }

        override protected void DoRegistIndex(IndexBlock indexBlock)
        {
        }

        override protected void Init()
        {
            Index = new List<IndexBlock>();
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                string file = StoreStream.Name;
                base.Dispose(disposing);
                File.Delete(file);
            }
            disposed = true;
        }
        private bool disposed;
       
    }
}
