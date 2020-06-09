namespace Support.Log
{
    internal class SystemSetting
    {
        static private SystemSetting instance;
        static public SystemSetting Instance
        {
            get
            {
                if (instance == null)
                    instance = new SystemSetting();
                return instance;
            }
        }
        public SystemSetting()
        {
            LoadSettings();
        }

        internal void LoadSettings()
        {
            StorageDirectory = SupportSetting.Instance.GetConfig(SupportSetting.ConfigKey.LogPath).Value;            
        }

        public string StorageDirectory 
        {
            get { return storageDirectory; }
            set 
            {
                if (string.IsNullOrEmpty(value) )
                    return;
                if(!System.IO.Directory.Exists(value))
                    System.IO.Directory.CreateDirectory(value);
                storageDirectory = value;                
            }
        }
        
        private string storageDirectory;
    }
}
