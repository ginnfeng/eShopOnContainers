////*************************Copyright © 2013 Feng 豐**************************
// Created    : 3/5/2015 11:42:29 AM
// Description: RestClientExt.cs
//http://restsharp.org/
//https://docs.docker.com/reference/api/docker_remote_api/     1.17
// Revisions  :
// ****************************************************************************

using RestSharp;
using Support.Open.RestSharp;
using System;
using System.Text;
using Support.Open.Docker.Setting;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Support.Open.RestSharp
{
    //****   TaskCompletionSource   http://stackoverflow.com/questions/15434983/waiting-for-executeasync-result
    //http://ianobermiller.com/blog/2012/07/23/restsharp-extensions-returning-tasks/

    static public partial class RestClientExt
    {
        static internal Func<string, string> ContentProcessMethod
        {
            get { return content => "[" + content.Replace("}{", "},{").Replace("}\r\n{", "},{") + "]"; }
        }

        static public T BuildImage<T>(this RestClient client, DynamicRestRequest<T> request, string filePath)
        where T : class
        {
            Task<T> task = client.BuildImageAsync<T>(request, filePath);
            task.Wait(client.Timeout);
            return task.Result;
        }

        static private string BuildUrlQueryString(IRestRequest request)
        {
            StringBuilder queryStringBuilder = new StringBuilder();
            //var queryParameters=request.Node.Parameters.FindAll(parameter=>parameter.Type==ParameterType.QueryString);
            string urlPath = request.Resource + "?";
            foreach (var parameter in request.Parameters)
            {
                switch (parameter.Type)
                {
                    case ParameterType.QueryString:
                        if (queryStringBuilder.Length != 0) queryStringBuilder.Append("&");
                        queryStringBuilder.Append(parameter.Name).Append("=").Append(parameter.Value);
                        break;

                    case ParameterType.UrlSegment:
                        urlPath = urlPath.Replace("{" + parameter.Name + "}", parameter.Value == null ? "" : parameter.Value.ToString());
                        break;
                    //case ParameterType.Cookie:case ParameterType.GetOrPost:case ParameterType.HttpHeader:case ParameterType.RequestBody:
                    default:
                        break;
                }
            }
            queryStringBuilder.Insert(0, urlPath);
            return queryStringBuilder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        static async public Task<T> BuildImageAsync<T>(this RestClient client, DynamicRestRequest<T> request, string filePath)
        //where T : class
        {
            using (HttpClient httpClient = new HttpClient() { BaseAddress = client.BaseUrl })
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/tar"));
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, BuildUrlQueryString(request.Node));
                req.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                var responseTask = await httpClient.SendAsync(req);
                var rltMsgTask = await responseTask.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(request.ContentProcessMethod(rltMsgTask));
            }
        }

        static public void DownloadFile(this RestClient client, DynamicRestRequest<byte[]> request, string filePath)
        {// ContentType=="application/octet-stream"
            var buffer = client.Execute(request);
            using (FileStream outFileStream = new FileStream(filePath, FileMode.Create))
            {
                outFileStream.Write(buffer, 0, buffer.Length);
                outFileStream.Close();
            }
        }

        private static Task<T> DoExecuteAsync<T>(this RestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {// to async-await pattern
            var tcs = new TaskCompletionSource<T>();
            var loginResponse = client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException == null)
                    tcs.SetResult(selector(response));
                else
                    tcs.SetException(response.ErrorException);
            });
            return tcs.Task;
        }

        static public Task<IRestResponse> ExecuteAsync(this RestClient client, DynamicRestRequest request)
        {
            return client.ExecuteTaskAsync((RestRequest)request);
            //使用DoExecuteAsync同ExecuteTaskAsync,只是為練功用
            //return client.DoExecuteAsync<IRestResponse>((RestRequest)request,rp=>rp);
        }

        //static public Task<T> ExecuteAsync<T>(this RestClient client, DynamicRestRequest<T> request)
        //    //where T : class
        //{
        //    return client.ExecuteAsync<T>(request);
        //}
        static async public Task<T> ExecuteAsync<T>(this RestClient client, DynamicRestRequest<T> request)
        //where T : class
        {
            var response = await client.ExecuteAsync((RestRequest)request);
            return ToEntity<T>(request, response);
        }

        static public IRestResponse Execute(this RestClient client, DynamicRestRequest request)
        {
            return client.Execute((RestRequest)request);
        }

        //static public T Execute<T>(this RestClient client, DynamicRestRequest<T> request)
        //    //where T : class
        //{
        //    return client.Execute<T>((DynamicRestRequest)request);
        //}
        static public T Execute<T>(this RestClient client, DynamicRestRequest<T> request)
        //where T : class
        {
            var response = client.Execute(request.Node);
            return ToEntity<T>(request, response);
            /* 某些情況只支援Single Thread不能用非同步去模擬同步，否則會卡死
            var rltTask = client.ExecuteAsync<T>(request);
            rltTask.Wait(client.Timeout);
            return rltTask.Result;
            */
        }

        static public T ToEntity<T>(DynamicRestRequest<T> request, IRestResponse response)
        {
            Exception err;
            if (!request.IgnoreException && TryCheckException(response, out err))
                throw err;
            if (request.ContentConvertMethod != null)
                return request.ContentConvertMethod(response);
            return response.JsonToObject<T>(request.ContentProcessMethod);
        }

        public static DynamicRestRequest TakeRequest(this RestClient client, string defKey, string id = null, string act = null)
        {
            return TakeRequest(defKey, id, act);
        }

        public static DynamicRestRequest TakeRequest(string defKey, string id = null, string act = null)
        {
            ApiDef apidef;
            if (defKey.Contains("/"))
            {
                apidef = new ApiDef() { Path = defKey };
            }
            else if (!ApiSetting.Instance.TryGet(defKey, out apidef))
                throw new NotSupportedException("ListContainers");
            dynamic request = new DynamicRestRequest(apidef.Path, apidef.Method);
            //request.Node.RequestFormat = DataFormat.Json;
            //request.JsonBody = new JObject();
            request.UrlSegment.id = id;
            request.UrlSegment.action = act;
            return request;
        }

        /// <param name="defKey">法一:定義RC.resx檔,TakeRequest("key")取得url定義,例如 Util\Support.Open\RC.resx 或 Sgx\Sgx.Data.Mgr\RC.resx</param>
        /// <param name="defKey">法二:傳url path ,例如 TakeRequest("/knowledgebases/18155/generateAnswer")</param>
        public static DynamicRestRequest<T> TakeRequest<T>(string defKey, string id = null, string act = null)
        //where T : class
        {
            DynamicRestRequest request = TakeRequest(defKey, id, act);
            var it = request.Clone<T>();
            if (typeof(string).IsAssignableFrom(typeof(T)))
                it.ContentConvertMethod = rp => (T)(dynamic)rp.Content;
            else if (typeof(byte[]).IsAssignableFrom(typeof(T)))
                it.ContentConvertMethod = rp => (T)(dynamic)rp.RawBytes;
            else if (typeof(bool).Equals(typeof(T)))
                it.ContentConvertMethod = rp => (T)(dynamic)(string.IsNullOrEmpty(rp.Content) ? false : rp.Content.Contains("OK"));
            return it;
        }

        public static DynamicRestRequest<T> TakeRequest<T>(this RestClient client, string defKey, string id = null, string act = null)
        {
            return TakeRequest<T>(defKey, id, act);
        }

        private static bool TryCheckException(IRestResponse response, out Exception err)
        {
            err = null;
            switch (response.StatusCode)
            {
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.Found:
                case HttpStatusCode.Continue:
                case HttpStatusCode.OK:
                //case HttpStatusCode.Redirect:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.RedirectMethod:
                    break;                    
                default:
                    err = (response.ErrorException != null) ?
                        response.ErrorException
                        : new WebException(response.Content);//原HttpException .net core不支援
                    break;
            }
            return err != null;
        }
    }
}