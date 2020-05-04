////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/5/2015 11:42:29 AM 
// Description: RestClientExt.cs  
// Revisions  :            		
// **************************************************************************** 

using RestSharp;
using Support.Open.Docker.Entity;
using Support.Open.RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Support.Open.Docker.ContainerApi
{
    static public partial class RestClientExt
    {
        
        static public DynamicRestRequest<bool> ReqCheckAuthConfiguration(this RestClient client, AuthArg authArg)
        {
            dynamic request = TakeRequest<bool>("PostAction", null, "auth");
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;
            request.JsonBody = authArg;
            request.IgnoreException = true;//exception as false
            return request;
        }
        static public DynamicRestRequest<JObject> ReqSystemWideInformation(this RestClient client)
        {
            return TakeRequest<JObject>("GetAction", null, "info");
        }
        static public DynamicRestRequest<SystemWideInformation> ReqSystemWideInformation2(this RestClient client)
        {
            return client.ReqSystemWideInformation().Clone<SystemWideInformation>();
        }
        static public DynamicRestRequest<JObject> ReqDockerVersionInformation(this RestClient client)
        {
            return TakeRequest<JObject>("GetAction", null, "version");
        }
        static public DynamicRestRequest<DockerVersionInformation> ReqDockerVersionInformation2(this RestClient client)
        {
            return client.ReqDockerVersionInformation().Clone<DockerVersionInformation>();
        }
        static public DynamicRestRequest<bool> ReqPingDockerServer(this RestClient client)
        {
            var request= TakeRequest<bool>("GetAction", null, "_ping");
            request.IgnoreException = true;//exception as false
            return request;
        }
        /// <summary>
        /// Create a new image from a container's changes
        /// </summary>
        /// <param name="client"></param>
        /// <param name="containerId"></param>
        /// <param name="repository"></param>
        /// <param name="tag"></param>
        /// <param name="author"></param>
        /// <param name="comment"></param>
        /// <param name="containerArg"></param>
        /// <returns></returns>
        static public DynamicRestRequest<JObject> ReqCreateImage(this RestClient client, string containerId, string repository,string tag,string author ,string comment , ContainerArg containerArg)
        {
            dynamic request = TakeRequest<JObject>("PostAction", null, "commit");
            request.Parameter.container = containerId;
            request.Parameter.repo = repository;
            request.Parameter.tag = tag;
            request.Parameter.author = author;
            request.Parameter.comment = comment;
            request.JsonBody = containerArg;
            return request;
        }
        static public DynamicRestRequest<ImageRlt> ReqCreateImage2(this RestClient client, string containerId, string repository, string tag, string author, string comment, ContainerArg containerArg)
        {
            return client.ReqCreateImage(containerId,repository,tag,author,comment,containerArg).Clone<ImageRlt>();
        }

        static public DynamicRestRequest<JArray> ReqMonitorDockerEvents(this RestClient client, DockerEventParameter parameter)
        {//目前測試都無法回傳。timeout
            dynamic request = TakeRequest<JArray>("GetAction", null, "events");
            request.Parameter.since = parameter.Since; //<=timeStamp convert: http://www.epochconverter.com/
            request.Parameter.until = parameter.Until;
            request.Parameter.filters = parameter.Filters;
            //request.Parameter._event = parameter._event;
            request.Parameter.image = parameter.Image;
            request.Parameter.container = parameter.Container;
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;

            return request;
        }
        static public DynamicRestRequest<List<DockerEventInformation>> ReqMonitorDockerEvents2(this RestClient client, DockerEventParameter parameter)
        {
            return client.ReqMonitorDockerEvents(parameter).Clone<List<DockerEventInformation>>();
        }
        
    }

}
