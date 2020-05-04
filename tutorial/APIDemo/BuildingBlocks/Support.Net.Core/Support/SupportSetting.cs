////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/16/2008 10:32:42 AM 
// Description: SupportSetting.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Support.Helper;

namespace Support
{
    public class SupportSetting
    {
        public enum ConfigKey
        {
            [Naming(Presentation = "Logs")]
            LogPath,
            [Naming(Presentation = "Datas")]
            DataPath
        }

        public static SupportSetting Instance
        {
            get
            {
                return Singleton<SupportSetting>.Instance;
            }
        }

        //public SupportSetting()
        //{
        //    Reload();
        //}

        public string DefaultBaseDirectory { get; set; }

        public Config GetConfig(ConfigKey key)
        {
            string keyName=enumhelper.GetName(key);
            Config cfg=null;
            if (cfgList != null)
            {

                cfg = cfgList.Find(delegate(Config config) { return keyName.Equals(config.Key); });                
            }
            if (cfg == null)
            {
                string baseDir = string.IsNullOrEmpty(DefaultBaseDirectory) ? AppDomain.CurrentDomain.BaseDirectory : DefaultBaseDirectory;
                NamingAttribute namingAttribute;
                if (enumhelper.TryGetNamingAttributeByValue(key, out namingAttribute))
                {
                    cfg = new Config() { Value = baseDir + namingAttribute.Presentation };
                }
            }
            return cfg;
        }

        //public void Reload()
        //{
        //    const string sectionName = "systemSetting/support";
        //    cfgList = ConfigMgr.GetWebConfig<Config>(sectionName);            
        //}

        private List<Config> cfgList=null;
        private EnumHelper enumhelper = new EnumHelper(typeof(ConfigKey));
    }
}
