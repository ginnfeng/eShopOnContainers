////*************************Copyright © 2013 Feng 豐**************************
// Created    : 11/1/2017 9:35:08 AM
// Description: DynamicRestClient.cs
// Revisions  :
// ****************************************************************************
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Support.Open.RestSharp
{
    public class DynamicRestClient : RestClient
    {
        public DynamicRestClient()
        {
        }

        public DynamicRestClient(string url)
            : base(url)
        {
        }

        public DynamicRestClient(Uri url)
            : base(url)
        {
        }

        override public IRestResponse Execute(IRestRequest request)
        {
            return Execute2(request, true);
        }

        public IRestResponse Execute2(IRestRequest request, bool autoRetry)
        {
            try
            {
                var startTm = DateTime.Now;
                var rlt = base.Execute(request);
                TimeSpan timespan = DateTime.Now - startTm;
                var hasException = rlt.ErrorException != null;
                if (rlt.StatusCode != HttpStatusCode.OK || hasException)
                {
                    if (hasException) throw rlt.ErrorException;
                    throw new WebException();
                }
                if (CheckContentEvent != null && !CheckContentEvent(rlt.RawBytes))
                    throw new WebException();
                if (ProxyServerEntity.FailedNum > 0)
                    ProxyServerEntity.FailedNum = 0;
                else if (timespan.Milliseconds <= this.GoodTimespan && ProxyServerEntity.FailedNum > -9)
                    ProxyServerEntity.FailedNum = ProxyServerEntity.FailedNum - 1;
                errRetryNum = 0;
                return rlt;
            }
            catch (Exception e)
            {
                if (autoRetry && (ExecuteErrorEvent != null && errRetryNum < 3))
                {
                    var newProxy = ExecuteErrorEvent(ProxyServerEntity, e);
                    if (newProxy != null)
                    {
                        ProxyServerEntity = newProxy;
                        errRetryNum++;
                        return Execute(request);
                    }
                }
                throw;
            }
        }

        public WebProxyServerEntity ProxyServerEntity
        {
            get { return proxyServerEntity; }
            set
            {
                proxyServerEntity = value;
                if (value != null && !string.IsNullOrEmpty(value.IP))
                    base.Proxy = new WebProxy(value.IP, value.Port);
            }
        }

        public event Func<WebProxyServerEntity, Exception, WebProxyServerEntity> ExecuteErrorEvent;

        public event Func<byte[], bool> CheckContentEvent;

        private WebProxyServerEntity proxyServerEntity;
        private int errRetryNum = 0;
        public int GoodTimespan { get; set; }
    }
}