using Newtonsoft.Json;
using RestSharp;
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/5/2015 2:46:08 PM 
// Description: ApiSetting.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
using Common.Open;
namespace Common.Open.Docker.Setting
{
    public class ApiSetting
    {
        static private ApiSetting instance;
        static public ApiSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiSetting();
                    instance.AddApiDefs(RC.DockerAPIList);                   
                }
                return instance;
            }
        }
        
        public  bool TryGet(string key,out ApiDef apiDef)
        {
            return apiDefMap.TryGetValue(key,out apiDef);
        }
        public void AddApiDefs(string jsonstring)
        {
            var defList = JsonConvert.DeserializeObject<List<ApiDef>>(jsonstring);
            AddApiDefs(defList);
        }
        public void AddApiDefs(IList<ApiDef> defList)
        {
            foreach (var def in defList)
            {
                apiDefMap[def.Name] = def;
            }
        }
        private Dictionary<string, ApiDef> apiDefMap=new Dictionary<string,ApiDef>();
    }

    public class ApiDef
    {        
        public string Name { get; set; }
        public Method Method { get; set; }
        public string Path { get; set; }
        //public DataFormat RequestFormat { get; set; }        
    }

}
