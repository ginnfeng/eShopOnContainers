////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/27/2020 2:32:54 PM 
// Description: CfgEntity.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGw.Ocelot
{
    public class CfgRootobject
    {
        public Reroute[] ReRoutes { get; set; }
        public Swaggerendpoint[] SwaggerEndPoints { get; set; }
        public Globalconfiguration GlobalConfiguration { get; set; }
    }

    public class Globalconfiguration
    {
        public string BaseUrl { get; set; }
    }

    public class Reroute
    {
        public string DownstreamPathTemplate { get; set; }
        public string DownstreamScheme { get; set; }
        public Downstreamhostandport[] DownstreamHostAndPorts { get; set; }
        public string UpstreamPathTemplate { get; set; }
        public string[] UpstreamHttpMethod { get; set; }
        public string SwaggerKey { get; set; }
    }

    public class Downstreamhostandport
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class Swaggerendpoint
    {
        public string Key { get; set; }
        public Config[] Config { get; set; }
    }

    public class Config
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
    }
    //***********

    public class ServiceDef
    {
        public string Host { get; set; }
        public List<string> Versions { get; set; }
    }
}
