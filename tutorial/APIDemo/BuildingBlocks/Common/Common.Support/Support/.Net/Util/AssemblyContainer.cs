using Support.Log;
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 9/26/2013 10:27:10 AM 
// Description: AssemblyContainer.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Support.Net.Util
{
    /// <summary>
    /// 為跨AppDomain設計,每個Type被Load自不同Assembly(已被AppDomain Loaded Assemly)會被視為不同
    /// </summary>
    public class AssemblyContainer : MarshalByRefObject
    {
        /// <summary>
        /// 每個AppDomain都會擁有獨立的static global變數
        /// </summary>
        static public AssemblyContainer Instance
        {
            get { return Support.Singleton<AssemblyContainer>.Instance; }
        }

        /// <param name="type">可能是來自其他AppDomain之Type</param>
        /// <returns>轉成本AssemblyContainer所在之Type</returns>
        public Type GetType(Type type)
        {
            return GetType(type.FullName);
        }
        public Type GetType(string typeFullName)
        {
            Type type;
            typeMap.TryGetValue(typeFullName, out type);
            return type;
        }
        public void Regist(ICollection<Byte[]> asmRaws)
        {
            ////http://stackoverflow.com/questions/1588490/assembly-loadbyte-and-assembly-location-assembly-codebase
            ///直接Load Byte[]會失去Assembly的重要資訊,如CodeBase,導致同一Type在run time時被視為不同,解決方法只能先寫入File
            ///再Assembly.LoadFrom(file)才能正常
            List<Assembly> asms = new List<Assembly>();
            asmRaws.ForEach(asmRaw => asms.Add(Assembly.Load(asmRaw)));
            Regist(asms);
        }
        public void Regist(ICollection<Assembly> asms)
        {
            foreach (var asm in asms)
            {
                foreach (var type in asm.GetTypes())
                {
                    typeMap[type.FullName] = type;
                }
                asmMap[asm.FullName] = asm;
            }
        }
        public void Regist(string packDir, bool ingoreError=true)
        {
            List<Assembly> asms = new List<Assembly>();
            foreach (var filePath in Directory.GetFiles(packDir, "*.dll"))
            {
                try
                {
                    //應該改用Assembly.Load(),因LoadFrom會將相關Dll一併load,效能較差
                    asms.Add(Assembly.LoadFrom(filePath));
                }catch(Exception e)
                {
                    if (ingoreError)
                    {
                        DailyLogger.Instance.Write<AssemblyContainer>("Warning! unable load {0}", filePath);
                        continue;
                    }
                    DailyLogger.Instance.Write<AssemblyContainer>(e);
                }
            }
            Regist(asms); 
        }
        public Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                Assembly asm;
                return asmMap.TryGetValue(args.Name, out asm) ? asm : Assembly.Load(args.Name);
            }
            catch (Exception err)
            {
                DailyLogger.Instance.Write<AssemblyContainer>(err);
                throw err;
            }

        }
        static public List<KeyValuePair<string, byte[]>> LoadDllFromDirectory(string packDir)
        {

            var dlls = new List<KeyValuePair<string, byte[]>>();
            foreach (var filePath in Directory.GetFiles(packDir, "*.dll"))
            {
                using (FileStream inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    dlls.Add(new KeyValuePair<string, byte[]>(Path.GetFileName(filePath),inStream.ReadAllBytes()));
                    
                    inStream.Close();
                }
            }
            return dlls;
        }
        public List<Assembly> Assemblies { get {return new List<Assembly>(asmMap.Values) ;} }
        private Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
        private Dictionary<string, Assembly> asmMap = new Dictionary<string, Assembly>();
    }
}
