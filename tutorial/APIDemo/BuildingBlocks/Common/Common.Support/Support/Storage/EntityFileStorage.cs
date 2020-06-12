using System;
using System.IO;
using System.Threading;
using System.Globalization;
using Support.Serializer;

namespace Support.Storage
{
    public enum FormatMode
    {
        Binary,
        Xml
    }

    public class EntityFileStorage<T>
        where T:new()
    {        
        public  EntityFileStorage()
        {
            
        }
        public EntityFileStorage(string id)
        {
            this.id = id;            
        }
        
        public void DeleteAll()
        {
            string dir = StorageDirectory;
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    File.Delete(file);
                }
            }
        }
        public bool IsSaveAsZip { get; set; }
        public string StorageDirectory 
        {
            get
            {
                if (string.IsNullOrEmpty(BaseDirectory))
                    baseDirectory = GetDefaultBaseBirectory();
                Type type=typeof(T);
                string typeName = (type.IsGenericType) ?
                    (type.GetGenericArguments()[0].Name + "#")
                    : typeof(T).Name;
                return BaseDirectory + "\\" + typeName; 
            }            
        }
        public string BaseDirectory
        {
            get
            {
                return baseDirectory;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                baseDirectory = value;
                Directory.CreateDirectory(baseDirectory);
            }
        }

        public void Delete(T it)
        {
            if (it == null)
                return;
            rwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                string path = GenPath(it, true);
                File.Delete(path);
            }finally
            {
                rwLock.ReleaseWriterLock();
            }
        }
        //int idx = 0;
        public void Save(T it)
        {
            rwLock.AcquireWriterLock(Timeout.Infinite);            
            try
            {                
                string path = GenPath(it, true);
                //Console.WriteLine("Save******{0} {1}",path, idx);
                BaseTransfer transfer = GetTransfer();
                if (IsSaveAsZip)
                {
                    transfer.SaveZip(it, path);
                }
                else
                {
                    transfer.Save(it, path);
                }
            }finally
            {                
                rwLock.ReleaseWriterLock();

            }            
        }

        public bool TryGet(out T it)
        {
            return TryGet(default(T),out it);
        }

        public bool TryGet(T sample,out T it)
        {
            rwLock.AcquireReaderLock(Timeout.Infinite);
            string path=null;
            it = default(T);
            try
            {                
                path = GenPath(sample, false);
                //Console.WriteLine("TryGet******{0} {1}", path, idx);
                if (!File.Exists(path))
                    return false;                
                BaseTransfer transfer=GetTransfer();
                it = (IsSaveAsZip)?transfer.LoadZip<T>(path):transfer.Load<T>(path);                
                return true;
            }
            catch
            {
                if (path != null )//&& (e is SerializationException||e is FileLoadException) )
                    File.Delete(path);
                return false;
            }
            finally
            {                
                rwLock.ReleaseReaderLock();
            }
        }

        private string GenPath(T it, bool isAutoCreateDir)
        {            
            if (isAutoCreateDir)
                Directory.CreateDirectory(StorageDirectory);
            return string.Format(CultureInfo.InvariantCulture, @"{0}\{1}.xml", StorageDirectory, GenId(it));

        }
        private string GenId(T it)
        {
            if (string.IsNullOrEmpty(id))
            {
                IEntity entity = it as IEntity;
                if (entity != null)
                    return entity.Id;
                return typeof(T).Name;

            }
            return id;
        }
        public FormatMode Format
        {
            get { return format; }
            set { format = value; }
        }

        BaseTransfer GetTransfer()
        {
            return (format == FormatMode.Xml)
                    ? (BaseTransfer)Singleton0<XmlTransfer>.Instance
                    : Singleton0<BinaryTransfer>.Instance;
        }
        private string GetDefaultBaseBirectory()
        {
            return defaultBaseDirectory;            
        }

        private string id;
        private string baseDirectory;
        private FormatMode format;        
        /// <summary>
        /// 允許同時間讓執行緒進行讀取，讓一條執行緒進行寫入
        /// </summary>
        static ReaderWriterLock rwLock = new ReaderWriterLock();
        const string defaultBaseDirectory = "EntityFileStorage";
       
    }
}
