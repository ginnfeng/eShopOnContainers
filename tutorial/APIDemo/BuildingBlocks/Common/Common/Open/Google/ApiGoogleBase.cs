////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/16/2017 11:25:37 AM
// Description: ApiGoogleBase.cs
// Revisions  :
// ****************************************************************************
using RestSharp;
//using Support.Open.Docker.ContainerApi;
using Common.Open.RestSharp;


namespace Common.Open.Google
{
    public class ApiGoogleBase
    {
        public ApiGoogleBase(string baseUrl, string actTemplateUrl, string apiKey)
        {
            ApiKey = apiKey;
            this.actTemplateUrl = actTemplateUrl;
            TheRestClient = new RestClient(baseUrl);
        }

        protected DynamicRestRequest<T> TakeRequest<T>(string act)
        {
            var req = TheRestClient.TakeRequest<T>(string.Format(actTemplateUrl, act, ApiKey));
            req.Node.AddHeader("Content-Type", "application/json");
            req.Node.Method = Method.POST;
            return req;
        }

        protected T Execute<T>(DynamicRestRequest<T> request)
        {
            return TheRestClient.Execute<T>(request);
        }

        public string ApiKey { get; set; }
        protected RestClient TheRestClient { get; private set; }
        private string actTemplateUrl;
    }
}