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
        //static Func<string,string> contentProcessMethod = content => "[" + content.Replace("}{", "},{").Replace("}\r\n{", "},{") + "]";
        static public DynamicRestRequest<JArray> ReqListImages(this RestClient client, bool all = true)
        {
            dynamic request = TakeRequest<JArray>("ListImages",null,"json");
            request.Parameter.all = all;

            return request;
        }
        static public DynamicRestRequest<List<Image>> ReqListImages2(this RestClient client, bool all = true)
        {
            return client.ReqListImages(all).Clone<List<Image>>();
        }
        static public DynamicRestRequest<JArray> ReqCreateImage(this RestClient client, string fromImage, string tag = "latest")
        {
            dynamic request = TakeRequest<JArray>("CreateImage");
            request.Parameter.fromImage = fromImage;
            request.Parameter.tag = tag;
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;

            return request;
        }
        static public DynamicRestRequest<List<ImageRlt>> ReqCreateImage2(this RestClient client, string fromImage, string tag = "latest")
        {
            return client.ReqCreateImage(fromImage, tag).Clone<List<ImageRlt>>();
        }
        static public DynamicRestRequest<JObject> ReqInspectImage(this RestClient client, string id)
        {
            return TakeRequest<JObject>("QueryOneImage", id, "json");
        }
        static public DynamicRestRequest<InspectImage> ReqInspectImage2(this RestClient client, string id)
        {
            return client.ReqInspectImage(id).Clone<InspectImage>();
        }
        static public DynamicRestRequest<JArray> ReqGetHistoryOfImage(this RestClient client, string name)
        {
            return  TakeRequest<JArray>("QueryOneImage", name, "history");
          
        }
        static public DynamicRestRequest<List<HistoryOfImage>> ReqGetHistoryOfImage2(this RestClient client, string name)
        {
            return client.ReqGetHistoryOfImage(name).Clone<List<HistoryOfImage>>();
        }
      
        /// <param name="client"></param>
        /// <param name="name">ex: "registry.acme.com:5000/test" or "test"</param>
        /// <returns></returns>
        static public DynamicRestRequest<JArray> ReqPushImageOnRegistry(this RestClient client, string name)
        {
            dynamic request = TakeRequest<JArray>("ActionOneImage", name, "push");
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;
            return request;
        }
        static public DynamicRestRequest<List<ImageRlt>> ReqPushImageOnRegistry2(this RestClient client, string name)
        {
            return client.ReqPushImageOnRegistry(name).Clone<List<ImageRlt>>();
        }
        static public DynamicRestRequest<JObject> ReqTagImageIntoRepository(this RestClient client, string name)
        {
            dynamic request = TakeRequest<JObject>("ActionOneImage", name, "tag");
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;
            return request;
        }
        static public DynamicRestRequest<string> ReqTagImageIntoRepository2(this RestClient client, string name)
        {
            return client.ReqTagImageIntoRepository(name).Clone<string>();
        }

        static public DynamicRestRequest<JArray> ReqRemoveImage(this RestClient client, string name, bool force, bool noprune = false)
        {
            dynamic request = TakeRequest<JArray>("RemoveImage", name);
            request.Parameter.noprune = noprune;
            request.Parameter.force = force;
            return request;
        }
        static public DynamicRestRequest<List<ImageRlt>> ReqRemoveImage2(this RestClient client, string name, bool force, bool noprune=false)
        {
            return client.ReqRemoveImage(name,force,noprune).Clone<List<ImageRlt>>();
        }
        static public DynamicRestRequest<JArray> ReqSearchImages(this RestClient client, string term)
        {
            dynamic request = TakeRequest<JArray>("ListImages", null, "search");
            request.Parameter.term = term;

            return request;
        }
        static public DynamicRestRequest<List<Image>> ReqSearchImages2(this RestClient client, string term)
        {
            return client.ReqSearchImages(term).Clone<List<Image>>();
        }
        static public DynamicRestRequest<JArray> ReqBuildImage(this RestClient client, string repository )
        {
            dynamic request = TakeRequest<JArray>("PostAction",null,"build");            
            ((DynamicRestRequest)request).ContentProcessMethod = ContentProcessMethod;
            request.Parameter.t = repository;  
            return request;
        }
        static public DynamicRestRequest<List<ImageRlt>> ReqBuildImage2(this RestClient client, string repository)
        {
            return client.ReqBuildImage(repository).Clone<List<ImageRlt>>();
        }
        
    }

}
