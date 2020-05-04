using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
//using System.Web.Configuration;
using System.Reflection;
using System.Globalization;

namespace Support
{
    public class ConfigHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            return section;
        }

        #endregion
    }

    public interface IConfig
    {
         object QueryValue(string key);
         void AssignValue(string key, object value);
    }
    

    public class Config : IConfig
    {
        public Config()
        {
        }
        public string Key
        {
            get
            {
                return this.objectKey;
            }
            set
            {
                this.objectKey = value;
            }
        }

        public string Value
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                this.defaultValue = value;
            }
        }

        public string Action
        {
            get
            {
                return this.action;
            }
            set
            {
                this.action = value;
            }
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment=value;
            }
        }

        public object QueryValue(string key)
        {
            object value;
            valueMap.TryGetValue(key.ToLower(CultureInfo.CurrentCulture), out value);
            return value;
        }

        public void AssignValue(string key,object value)
        {
            valueMap[key.ToLower(CultureInfo.CurrentCulture)] = value;
        }
        Dictionary<string, object> valueMap = new Dictionary<string, object>();
        #region MemberData
        string objectKey;
        string defaultValue;
        string action;
        string comment;
        #endregion
    }

    public class ConfigMgr
    {
        /// <summary>        
        /// </summary>
        /// <example>
        ///     <![CDATA[
        ///         <configuration>
        ///             <configSections>
        ///                 <sectionGroup name="xorderSettings">
        ///                     <section name="selling.Policy.Settings" type="Support.ConfigHandler, Support, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a382df5ba4924ad8"/>
        ///                 </sectionGroup>
        ///             </configSections>
        ///             <xorderSettings>
        ///                 <selling.Policy.Settings>      
        ///                     <SoConfig Key="OMContract.IIPMEntityMgrService" SoType="OMContract.IIPMEntityMgrService" SoAdapterType="ServiceAgent.SoWcfAdapter" ExtConfig="BasicHttpBinding_IIPMEntityMgrService" Comment=""/>
        ///                 </xorderSettings>
        ///             </xorderSettings>
        ///         </configuration>
        /// 
        ///     Sample Code: 
        ///        List<SoConfig2> cfgs = ConfigMgr.GetAppConfig<SoConfig2>("xorderSettings/selling.Policy.Settings", @"./SoConfig");
        ///     ]]>
        ///     
        
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName">ex:"soSetting/soRoutingTable"</param>
        /// <returns></returns>
        /// 
        public static List<T> GetConfig<T>(string sectionName)
            where T : new()
        {
            //return IsWebApp() ? GetWebConfig<T>(sectionName) : GetAppConfig<T>(sectionName);
            return GetAppConfig<T>(sectionName);
        }

        public static List<T> GetConfig<T>(string configFileName, string sectionName)
            where T : new()
        {
            return ToObjectList<T>(GetXmlConfig(configFileName, sectionName));
        }

        //public static XmlNode GetXmlConfig(string sectionName)
        //{
        //    return IsWebApp() ? GetWebXmlConfig(sectionName) : GetAppXmlConfig(sectionName);
        //}


        public static XmlNode GetXmlConfig(string configFileName,string sectionName)
        {            
            Configuration cfg = GetRawConfig(configFileName,sectionName);
            ConfigurationSection sect = cfg.GetSection(sectionName);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sect.SectionInformation.GetRawXml());
            return xmlDoc.DocumentElement;
        }

        //public static Configuration GetRawConfig(string sectionName)
        //{
        //    return GetRawConfig(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, sectionName);
        //}
        public static Configuration GetRawConfig(string configFileName, string sectionName)
        {
            //if (IsWebApp()) throw new NotSupportedException();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFileName;
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            return cfg;
        }
        //public static List<T> GetWebConfig<T>(string sectionName)  
        //    where T:new()
        //{
        //    XmlNode sectionNode = GetWebXmlConfig(sectionName);
            
        //    return ToObjectList<T>(sectionNode);
        //}
        //public static List<T> GetWebConfig<T>(string sectionName, string xmlPathOfNodes)
        //   where T : new()
        //{
        //    XmlNode sectionNode = GetWebXmlConfig(sectionName);
        //    return ToObjectList<T>(sectionNode, xmlPathOfNodes);
        //}

        public static List<T> GetAppConfig<T>(string sectionName)
            where T : new()
        {
            XmlNode sectionNode = GetAppXmlConfig(sectionName);
            return ToObjectList<T>(sectionNode);
        }

        public static List<T> GetAppConfig<T>(string sectionName, string xmlPathOfNodes)
            where T : new()
        {
            XmlNode sectionNode = GetAppXmlConfig(sectionName);
            return ToObjectList<T>(sectionNode, xmlPathOfNodes);
        }

        public static XmlNode GetAppXmlConfig(string sectionName)
        {
            ConfigurationManager.RefreshSection(sectionName);
            return (XmlNode)ConfigurationManager.GetSection(sectionName);
        }

        //public static XmlNode GetWebXmlConfig(string sectionName)
        //{
        //    return (XmlNode)WebConfigurationManager.GetSection(sectionName);
        //}

        //public static bool IsWebApp()
        //{
        //    return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Equals("Web.config");
        //}

        protected static List<T> ToObjectList<T>(XmlNode sectionNode)
            where T : new()
        {
            string xpathOfNodes = string.Format(CultureInfo.CurrentCulture,@"./{0}", typeof(T).Name);
            return ConfigMgr.ToObjectList<T>(sectionNode,xpathOfNodes);
        }

        protected static List<T> ToObjectList<T>(XmlNode sectionNode,string xpathOfNodes)
            where T : new()
        {
            if (sectionNode == null)
                return null;
            Type type = typeof(T);
            bool isStringMap=( type==typeof(Dictionary<string, string>));
            SidConverter converter = new SidConverter();
            List<T> objList = new List<T>();            
            XmlNodeList nodes = sectionNode.SelectNodes(xpathOfNodes);
            if (nodes == null)
                return null;
            //XmlSerializer serializer = new XmlSerializer(type);
            foreach (XmlNode node in nodes)
            {
                //XmlReader xmlReader = new XmlNodeReader(node);
                //T obj = (T)serializer.Deserialize(xmlReader);
                
                dynamic obj = new T();
                foreach (XmlAttribute attri in node.Attributes)
                {
                    if (isStringMap)
                    {
                        obj[attri.Name] = attri.Value.ToString();
                        continue;
                    }
                    PropertyInfo propertyInfo = type.GetProperty(attri.Name);
                    object propertyValue = (propertyInfo==null)?attri.Value.ToString():converter.Xml2Obj(attri.Value.ToString(), propertyInfo.PropertyType);
                    if (propertyInfo != null)
                    {                        
                        propertyInfo.SetValue(obj, propertyValue, null);
                    }
                    
                    IConfig cfg = obj as IConfig;
                    if (cfg != null)
                    {
                        cfg.AssignValue(attri.Name, propertyValue);
                    }
                
                }
                
                objList.Add(obj);
            }
            return objList;
        }

        
        
    }

}
