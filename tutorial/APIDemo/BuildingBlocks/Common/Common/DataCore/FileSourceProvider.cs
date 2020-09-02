////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/11/2011 3:35:07 PM 
// Description: FileSourceProvider.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.IO;
using Common.Support.Serializer;
using System.Threading;
using Common.Support.ThreadExt;
#if SILVERLIGHT
using Support.Net.Util;
#endif
namespace Common.DataCore
{
    public class FileSourceProvider : FileSourceProvider<JsonTransfer>
    {
        public FileSourceProvider(string storeDirectory)
            : base(storeDirectory)
        {
        }
    }

    public class FileSourceProvider<TTransfer> : FileSourceProviderBase<TTransfer>
        where TTransfer : BaseTransfer, new()
    {
        public FileSourceProvider(string storeDirectory)
            : base(storeDirectory)
        {
        }
        override protected void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        override protected FileStream TakeFileStream(string filePath, FileMode mode, FileAccess access)
        {
            return new FileStream(filePath, mode, access);
        }
        override protected bool FileExists(string path)
        {
            return File.Exists(path);
        }
        override protected bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        override protected void DeleteDirectories()
        {
            var dirs = Directory.GetDirectories(base.StoreDirectory);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir, true);
            }
        }
                
        override protected string[] GetDirectoryNames()
        {
            var dirs= Directory.GetDirectories(base.StoreDirectory); 
            string[] dirNames=new string[dirs.Length];
            for (int i = 0; i < dirs.Length;i++ )
            {
                //ex: Path.GetFileName(@"c:\A\B") return "B" ; Path.GetFileName(@"c:\A\B\") return "" 
                dirNames.SetValue(Path.GetFileName(dirs[i]),i);
            }
            return dirNames;
        }
        override protected void DeleteFile(string path)
        {
            File.Delete(path);
            //Directory.Delete(path);
        }
        override protected string[] GetFileNames(string dir, string searchPattern)
        {
            var paths= Directory.GetFiles(dir ,searchPattern);
            var names=new string[paths.Length];
            for (int i = 0; i < names.Length; i++)
                names.SetValue(Path.GetFileName(paths[i]),i);
            return names;
        }
        override protected IDisposable CreateLocker(bool isReadLocker)
        {
           if( isReadLocker) 
               return new AutoReaderLock(rwLock);
            return  new AutoWriterLock(rwLock);
        }
        

        protected override void Close()
        {            
        }
        //允許同時間讓執行緒進行讀取，讓一條執行緒進行寫入
        static ReaderWriterLock rwLock = new ReaderWriterLock();
    }
}
