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

namespace Support.Open.RestSharp
{
    static public partial class RestClientExt
    {
        static public DynamicRestRequest<JArray> ReqListContainers(this RestClient client, bool all = true, string before = null)
        {
            dynamic request = TakeRequest<JArray>("ListContainers");
            request.Parameter.all = all;
            request.Parameter.before = before;
            return request;
        }
        static public DynamicRestRequest<List<Container>> ReqListContainers2(this RestClient client, bool all = true, string before = null)
        {
            return client.ReqListContainers(all, before).Clone<List<Container>>();
        }
        static public DynamicRestRequest<JObject> ReqCreateContainer(this RestClient client, string name, ContainerArg arg)
        {
            dynamic request = TakeRequest<JObject>("CreateContainer");
            request.Parameter.name = name;
            request.JsonBody = arg;
            return request;
        }
        static public DynamicRestRequest<ContainerRlt> ReqCreateContainer2(this RestClient client, string name, ContainerArg arg)
        {
            return client.ReqCreateContainer(name, arg).Clone<ContainerRlt>();
        }
        static public DynamicRestRequest<JObject> ReqInspectContainer(this RestClient client, string id)
        {
            return TakeRequest<JObject>("QueryOneContainer", id, "json");
        }
        static public DynamicRestRequest<InspectContainer> ReqInspectContainer2(this RestClient client, string id)
        {
            return client.ReqInspectContainer(id).Clone<InspectContainer>();
        }
        static public DynamicRestRequest<JObject> ReqListProcessesOfContainer(this RestClient client, string id)
        {
            return TakeRequest<JObject>("QueryOneContainer", id, "top");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <param name="stderr">show stderr log</param>
        /// <param name="stdout">how stdout log</param>
        /// <param name="timestamps">print timestamps for every log line</param>
        /// <param name="follow">return stream</param>
        /// <returns></returns>
        static public DynamicRestRequest<string> ReqGetContainerLogs(this RestClient client, string id, bool follow = false, bool stderr = true, bool stdout = true, bool timestamps = true)
        {
            dynamic request = TakeRequest<string>("QueryOneContainer", id, "logs");
            request.Parameter.stderr = stderr;
            request.Parameter.stdout = stdout;
            request.Parameter.timestamps = timestamps;
            request.Parameter.follow = follow;
            return request;
        }
        static public DynamicRestRequest<JArray> ReqInspectChangesOnFileSystem(this RestClient client, string id)
        {
            return TakeRequest<JArray>("QueryOneContainer", id, "changes");
        }
        static public DynamicRestRequest<List<InspectChangesOnFileSystem>> ReqInspectChangesOnFileSystem2(this RestClient client, string id)
        {
            return client.ReqInspectChangesOnFileSystem(id).Clone<List<InspectChangesOnFileSystem>>();
        }

        static public DynamicRestRequest<byte[]> ReqExportContainer(this RestClient client, string id)
        {
            return TakeRequest<byte[]>("QueryOneContainer", id, "export");
        }
        static public DynamicRestRequest ReqResizeContainerTTY(this RestClient client, string id, int wSize, int hSize)
        {
            dynamic request = TakeRequest("ActionOneContainer", id, "resize");
            request.Parameter.h = hSize;
            request.Parameter.w = wSize;
            return request;
        }
        static public DynamicRestRequest ReqStartContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "start");
        }
        static public DynamicRestRequest ReqRestartContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "restart");
        }
        static public DynamicRestRequest ReqStopContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "stop");
        }
        static public DynamicRestRequest ReqKillContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "kill");
        }
        static public DynamicRestRequest ReqPauseContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "pause");
        }
        static public DynamicRestRequest ReqUnpauseContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "unpause");
        }
        static public DynamicRestRequest ReqAttachContainer(this RestClient client, string id, bool useWebsocket = false)
        {
            return TakeRequest("ActionOneContainer", id, useWebsocket ? "attach/ws" : "attach");
        }
        static public DynamicRestRequest ReqWaitContainer(this RestClient client, string id)
        {
            return TakeRequest("ActionOneContainer", id, "wait");
        }

        static public DynamicRestRequest ReqRemoveContainer(this RestClient client, string id, bool rmVolume, bool force)
        {
            dynamic request = TakeRequest("RemoveContainer", id);
            request.Parameter.v = rmVolume;
            request.Parameter.force = force;

            return request;
        }
        static public DynamicRestRequest<byte[]> ReqCopyFromContainer(this RestClient client, string id,CopyFromContainerArg target)
        {
            dynamic request = TakeRequest<byte[]>("ActionOneContainer", id, "copy");
            request.JsonBody = target;
            return request;
        }
    }    

}
