////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/30/2011 4:46:34 PM 
// Description: IsolatedStorageFileSourceProvider.cs  
// Revisions  :       
// reference: http://msdn.microsoft.com/zh-tw/magazine/dd458794.aspx   
//  IsolatedStorageFile存在於 C:\Users\[username]\AppData\LocalLow\Microsoft\Silverlight\is

// **************************************************************************** 
using System;
using Support.Serializer;
using System.IO;
using Support.ThreadExt;
using System.Threading;
#if SILVERLIGHT
using Support.SilverLight.ThreadExt;
#endif
namespace Common.DataCore
{

    /// <summary>
    /// IsolatedStorageFileSourceProvider--------------------------------------------------------------------------
    /// </summary>
    /// <typeparam name="TTransfer"></typeparam>
    public class IsolatedStorageFileSourceProvider<TTransfer> : FileSourceProviderBase<TTransfer>
        where TTransfer : BaseTransfer, new()
    {
        public IsolatedStorageFileSourceProvider(System.IO.IsolatedStorage.IsolatedStorageFile isolatedStorageFile)
            : this(isolatedStorageFile,".")
        {
            
        }
        public IsolatedStorageFileSourceProvider(System.IO.IsolatedStorage.IsolatedStorageFile isolatedStorageFile,string baseDir)
            : base(baseDir)
        {
            this.StorageFile = isolatedStorageFile;
        }
        override protected void CreateDirectory(string path)
        {
            StorageFile.CreateDirectory(path);
        }
        override protected FileStream TakeFileStream(string filePath, FileMode mode, FileAccess access)
        {
            var fileStream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(filePath, mode, access, StorageFile);
            return fileStream;
        }
        override protected bool FileExists(string path)
        {            
            return StorageFile.FileExists(path);
        }
        override protected void DeleteFile(string path)
        {
            StorageFile.DeleteFile(path);
        }
        protected void DeleteDirectory(string dirName)
        {
            String pattern = dirName + @"\*";
            String[] files = StorageFile.GetFileNames(pattern);
            foreach (String fName in files)
            {
                DeleteFile(Path.Combine(dirName, fName));
            }
            String[] subDirs = StorageFile.GetDirectoryNames(pattern);
            foreach (String dName in subDirs)
            {
                DeleteDirectory(Path.Combine(dirName, dName));
            }
            StorageFile.DeleteDirectory(dirName);
        }
        /// <returns>只回傳dir names</returns>
        override protected string[] GetDirectoryNames()
        {
            return StorageFile.GetDirectoryNames();
        }
        override protected void DeleteDirectories()
        {
            var dirs=GetDirectoryNames();          
            foreach (var dirName in dirs)
            {
                DeleteDirectory(dirName);
            }
        }
        override protected bool DirectoryExists(string path)
        {
            return StorageFile.DirectoryExists(path);
        }
        override protected string[] GetFileNames(string dir, string searchPattern)
        {
            return StorageFile.GetFileNames(dir + "\\" + searchPattern);
        }
        override protected IDisposable CreateLocker(bool isReadLocker)
        {
            // TODO 有問題就恢復 return null;
            //return null;           
           if( isReadLocker) 
               return new AutoReaderLock(rwLock);
            return  new AutoWriterLock(rwLock);        
        }
        protected override void Close()
        {
            StorageFile.Dispose();
        }
        public System.IO.IsolatedStorage.IsolatedStorageFile StorageFile { get; private set; }
        //允許同時間讓執行緒進行讀取，讓一條執行緒進行寫入
        static ReaderWriterLock rwLock = new ReaderWriterLock();
    }
    public class IsolatedStorageFileSourceProvider : IsolatedStorageFileSourceProvider<JsonTransfer>
    {
        public IsolatedStorageFileSourceProvider(System.IO.IsolatedStorage.IsolatedStorageFile isolatedStorageFile)
            : base(isolatedStorageFile)
        {
        }
    }
}
