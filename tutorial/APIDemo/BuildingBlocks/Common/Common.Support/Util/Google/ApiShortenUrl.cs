////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/16/2017 11:24:51 AM
// Description: ApiShortenUrl.cs
// Revisions  :
// ****************************************************************************
using Newtonsoft.Json.Linq;
using RestSharp;
using Support.Open.Docker.ContainerApi;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Support.Open.Google
{
    public class ApiShortenUrl : ApiGoogleBase
    {
        public ApiShortenUrl(string apiKey)
            : base("https://www.googleapis.com", "/{0}/v1/url?key={1}", apiKey)
        {
            //var req = client.TakeRequest<JObject>("/urlshortener/v1/url" + "?key=" + shortUrlApiKey);
        }

        public string UrlShortener(string longUrl)
        {
            try
            {
                return UrlTinyShortener(longUrl);
            }
            catch 
            {
                return UrlPicsShortener(longUrl);
            }
        }
        public string UrlPicsShortener(string longUrl)
        {// TODO: Add Testing logic here
            RestClient client = new RestClient("https://api.pics.ee");
            var req = RestClientExt.TakeRequest<JObject>("/v1/links/?access_token=73db788f9c3b110be62e4422b5e3b32506a7a4ce");
            req.Node.Method = Method.POST;
            ((dynamic)req).Parameter.url = HttpUtility.UrlPathEncode(longUrl);
            ((dynamic)req).Parameter.top = 20;
            var rlt = client.Execute(req);
            return rlt.First.First["picseeUrl"].Value<string>();            
        }
        public string UrlTinyShortener(string longUrl)
        {// TODO: Add Testing logic here
            //http://tinyurl.com/api-create.php?url=http://yourdomain.com/with/a/very/long/url
            RestClient client = new RestClient("http://tinyurl.com");
            var req = client.TakeRequest("/api-create.php");
            req.Node.Method = Method.GET;
            var dReq = ((dynamic)req);
            dReq.Parameter.url = HttpUtility.UrlPathEncode(longUrl);
            var rlt = client.Execute(req);
            var shortUrl= encoding.GetString(rlt.RawBytes);
            if (!shortUrl.Contains("//tinyurl.com"))
                throw new Exception("TinyShortener Error!");
            return shortUrl;
            //var req = base.TakeRequest<JObject>("urlshortener");
            //req.Node.AddJsonBody(new { longUrl = longUrl });
            //JObject jObject = Execute(req);
            //return jObject["id"].Value<string>();           
        }
        private Encoding encoding = Encoding.GetEncoding("big5");
    }
}