namespace Common.Support.Web
{

    /// <summary>
    /// ServiceDescriptionImporter不在.Net 4.0 Client Profile,故先移除
    /// </summary>
    public class WSProxyFactory
    {
        
    //    public enum Soap
    //    {
    //        Version11,
    //        Version12
    //    }
    //    WSProxyFactory()
    //    {
    //    }

    //    public WSProxy Create(Uri wsdlUri ,Soap version,bool replaceDataContract,string[] importNamespaces,string[] importAssembles)
    //    {            
    //        //string svcName = svcDesc.Services[0].Name;       
    //        Assembly assembly;
    //        if(!uriAssemblyDictionary.TryGetValue(wsdlUri,out assembly))
    //        {                
    //            assembly = CompileAssembly(wsdlUri ,version,replaceDataContract,importNamespaces,importAssembles);
    //            uriAssemblyDictionary[wsdlUri]=assembly;
    //        }
    //        WSProxy wsProxy = new WSProxy(assembly);
    //        wsProxy.WsdlUri = wsdlUri;
    //        return wsProxy;
    //    }

    //    static public ServiceDescription GetServiceDescription(Uri wsdlUri)
    //    {
    //        WebRequest webRequest = WebRequest.Create(wsdlUri);
    //        Stream reqStream = webRequest.GetResponse().GetResponseStream();
    //        ServiceDescription svcDesc = ServiceDescription.Read(reqStream);
    //        reqStream.Close();

    //        Regex regex = new Regex("(.{1,})/", RegexOptions.RightToLeft);
    //        Match mc = regex.Match(wsdlUri.OriginalString);
    //        string baseDir = mc.Groups[0].Value;

    //        XmlSchema schema = svcDesc.Types.Schemas[0];
            
    //        foreach (XmlSchemaObject schemaObject in schema.Includes)
    //        {
    //            XmlSchemaImport schemaImport = schemaObject as XmlSchemaImport;
    //            if (schemaImport != null)
    //            {
    //                if (schemaImport.SchemaLocation != null)
    //                {                           
                        
                        
    //                    Uri uriSchema =regex.Match(schemaImport.SchemaLocation).Success
    //                        ? new Uri(schemaImport.SchemaLocation)
    //                        : new Uri(baseDir + schemaImport.SchemaLocation);
                        

    //                    webRequest = WebRequest.Create(uriSchema);
    //                    reqStream = webRequest.GetResponse().GetResponseStream();
    //                    XmlSchema sc = XmlSchema.Read(reqStream, null);
    //                    reqStream.Close();
    //                    svcDesc.Types.Schemas.Add(sc);
    //                }
    //            }
                
    //        }        
    //        return svcDesc;
    //    }

    //    static public ServiceDescriptionImporter GetServiceDescriptionImporter(ServiceDescription svcDescription, Soap version)
    //    {
    //        ServiceDescriptionImporter svcImport = new ServiceDescriptionImporter();
    //        svcImport.AddServiceDescription(svcDescription, null, null);
    //        svcImport.ProtocolName = (version==Soap.Version12)?"Soap12":"Soap";
    //        svcImport.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;
    //        svcImport.Style = ServiceDescriptionImportStyle.Client;
    //        return svcImport;
    //    }

    //    static CodeCompileUnit GetCodeCompileUnit(ServiceDescriptionImporter svcImport, bool replaceDataContract, string[] importNamespaces)
    //    {
    //        CodeNamespace ns = new CodeNamespace();
    //        CodeCompileUnit ccu = new CodeCompileUnit();
    //        ccu.Namespaces.Add(ns);
    //       // ServiceDescriptionImportWarnings warnings = svcImport.Import(ns, ccu);
    //        if (replaceDataContract)
    //        {
    //            StripContractTypes(ccu, importNamespaces);
    //        }
    //        return ccu;
    //        /*
    //        CodeDomProvider codeProv = CodeDomProvider.CreateProvider("CSharp");            
    //        StringWriter sw = new StringWriter(CultureInfo.CurrentCulture);
    //        codeProv.GenerateCodeFromNamespace(ns, sw, null);           
    //        proxyCode = sw.ToString();
    //        return codeProv;
    //        */
    //    }
    //    /*
    //    static void LogProxyCode(CodeDomProvider codeProv,CodeCompileUnit ccu)
    //    {
    //        TextWriter writer = File.CreateText("_Proxy.cs"); // 指定你所需的源代碼文件名。            
    //        codeProv.GenerateCodeFromCompileUnit(ccu, writer, null);            
    //        writer.Flush();
    //        writer.Close();
         
    //        //CodeDomProvider codeProv = CodeDomProvider.CreateProvider("CSharp");
    //        //StringWriter sw = new StringWriter(CultureInfo.CurrentCulture);
    //        //codeProv.GenerateCodeFromNamespace(ns, sw, null);
    //        //proxyCode = sw.ToString();
           
    //    }
    //*/
    //    CompilerParameters GetCompilerParameters(string[] importAssembles)
    //    {
    //        CompilerParameters cpParam = new CompilerParameters(referenceDlls);
    //        cpParam.GenerateExecutable = false;
    //        cpParam.GenerateInMemory = true;
    //        cpParam.TreatWarningsAsErrors = false;
    //        cpParam.WarningLevel = 4;
    //        if (importAssembles != null)
    //        {
    //            foreach (string asm in importAssembles)
    //            {
    //                // 自訂的Assembly不管是否在GAC都要給full path
    //                cpParam.ReferencedAssemblies.Add(asm);
    //            }
    //        }
    //        return cpParam;
    //    }

    //    Assembly CompileAssembly(Uri wsdlUri, Soap ver, bool replaceDataContract, string[] importNamespaces, string[] importAssembles)
    //    {
    //        ServiceDescription svcDesc = GetServiceDescription(wsdlUri);
    //        ServiceDescriptionImporter svcImport = GetServiceDescriptionImporter(svcDesc, ver);
    //        CodeCompileUnit ccu=GetCodeCompileUnit(svcImport, replaceDataContract, importNamespaces);
    //        CodeDomProvider codeProv = CodeDomProvider.CreateProvider("CSharp");
    //        //LogProxyCode(codeProv,ccu);
    //        CompilerResults rlt = codeProv.CompileAssemblyFromDom(GetCompilerParameters(importAssembles),ccu);
    //        //CompilerResults rlt = codeProv.CompileAssemblyFromSource(GetCompilerParameters(), proxyCode);
    //        return rlt.CompiledAssembly;
    //    }


    //    static private void StripContractTypes(CodeCompileUnit codeCompileUnit, string[] importNamespaces)
    //    {
    //        foreach (CodeNamespace codeNamespace in codeCompileUnit.Namespaces)
    //        {
    //            // Remove anything that isn't the proxy itself
    //            for (int i = codeNamespace.Types.Count; i > 0; i--)
    //            {
    //                CodeTypeDeclaration codeType = codeNamespace.Types[i-1];
    //                bool webDerived = false;
    //                foreach (CodeTypeReference baseType in codeType.BaseTypes)
    //                {
    //                    if (baseType.BaseType == "System.Web.Services.Protocols.SoapHttpClientProtocol")
    //                    {
    //                        webDerived = true;
    //                        break;
    //                    }
    //                }
    //                if (!webDerived)
    //                {
    //                    codeNamespace.Types.RemoveAt(i - 1);
    //                }
    //            }
    //            if (importNamespaces != null)
    //            {
    //                foreach (string ns in importNamespaces)
    //                {
    //                    //產生 using xxx; 之code
    //                    codeNamespace.Imports.Add(new CodeNamespaceImport(ns));
    //                }
    //            }
    //        }
    //    }

    //    static public WSProxyFactory Instance
    //    {
    //        get
    //        {
    //            return instance;
    //        }
    //    }

    //    static WSProxyFactory instance = new WSProxyFactory();

    //    string[] referenceDlls = new string[] { "System.dll", "System.Xml.dll", "System.Web.Services.dll", "System.Data.dll" };
    //    Dictionary<Uri, Assembly> uriAssemblyDictionary = new Dictionary<Uri, Assembly>();
    }
}
