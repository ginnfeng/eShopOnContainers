////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/16/2009 10:44:31 AM 
// Description: ResourceHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Reflection;
using System.IO;

namespace Common.Support.Net.Util
{
    static public class ResourceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm">ex: Assembly.GetExecutingAssembly()</param>
        /// <param name="rcPath">ex: ex: "Folder.DataPackageTemplates.xml"</param>
        /// <returns></returns>
        static public Stream LoadFromManifestResource(Assembly asm, string rcPath,string ns=null)
        {
            ns = ns ?? asm.GetName().Name;
            string xamlResouce = Support.CommonExtension.StringFormat("{0}.{1}",ns ,rcPath);
            return asm.GetManifestResourceStream(xamlResouce);
        }
    }
}
