////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/22/2008 2:37:58 PM 
// Description: NetHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Net;

namespace Support
{
    static public class NetHelper
    {
        static public string GetLocalhostName()
        {
            return Dns.GetHostName();
        }
        static public IPHostEntry GetLocalIP()
        {
            return GetIPHost(GetLocalhostName());         
        }
        static public IPHostEntry GetIPHost(string hostName)
        {
            return Dns.GetHostEntry(hostName);
        }
    }
}
