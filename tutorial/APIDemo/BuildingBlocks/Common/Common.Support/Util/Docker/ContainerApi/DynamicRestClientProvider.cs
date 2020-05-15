////*************************Copyright © 2013 Feng 豐**************************
// Created    : 10/31/2017 3:29:03 PM
// Description: DynamicWebProxy.cs
// Revisions  :
// ****************************************************************************
using RestSharp;
using Support.Log;
using Support.Open.Serializer;
using Support.ThreadExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Support.Open.RestSharp
{
    public class WebProxyServerEntity
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public int FailedNum { get; set; }
    }
    public class FreeProxySource
    {
        public string Host { get; set; }
        public string[] Paths { get; set; }
        public Regex ContentRegex { get; set; }
        public bool IsHexPortType { get; set; }
        public string RemoveString { get; set; }
        public string ReplaceDotString { get; set; }
    }

    public class DynamicRestClientProvider
    {
        public DynamicRestClientProvider(string cfgPath, int failedNumForAutoRemove = 10, int goodTimespan = 1200, int connectionTimeout = 7000)
        {
            this.csvFilePath = cfgPath;
            Timeout = connectionTimeout;
            GoodTimespan = goodTimespan;
            this.failedNumForAutoRemove = failedNumForAutoRemove;
            AutoRemoveProxyServer = false;
        }

        public DynamicRestClientProvider(List<WebProxyServerEntity> proxies, int failedNumForAutoRemove = 10, int goodTimespan = 1200, int connectionTimeout = 7000)
            : this("", failedNumForAutoRemove, goodTimespan, connectionTimeout)
        {
            ProxyServers = proxies;
        }

        public List<WebProxyServerEntity> LoadProxyServers(string csvPath)
        {
            if (string.IsNullOrEmpty(csvPath))
                return new List<WebProxyServerEntity>();
            var proxyMap = new Dictionary<string, WebProxyServerEntity>();
            List<WebProxyServerEntity> proxies;
            using (NewLocker(csvPath))
                proxies = csvTransfer.Load<List<WebProxyServerEntity>>(csvPath);
            WebProxyServerEntity proxy;
            foreach (var item in proxies)
            {
                if (!proxyMap.TryGetValue(item.IP, out proxy) || item.FailedNum < proxy.FailedNum)
                    proxyMap[item.IP] = item;
            }

            return proxyMap.Values.ToList();
        }

        public int HealthProxyNum()
        {
            int ct = 0;
            var proxyEntitys = ProxyServers;
            proxyEntitys.ForEach(proxy => { if (proxy.FailedNum < 0) ct++; });
            return ct;
        }

        public int ProxyNum { get { return ProxyServers.Count; } }
        public void AutoAddFreeProxy(DailyLogger logger=null)
        {           
            var freeProxySrcList=GetFreeProxySrc();
            foreach (var src in freeProxySrcList)
            {
                try
                {
                    DoAutoAddFreeProxy(src);
                }
                catch(Exception e) {
                    if (logger != null) logger.Write(e, src.Host);
                }
            }
            
        }
        private void DoAutoAddFreeProxy(FreeProxySource src)
        {
            foreach (var path in src.Paths)
            {
                RestClient client = new RestClient(src.Host);
                
                var req = client.TakeRequest(path);
                req.Node.Method = Method.GET;
                req.Node.AddHeader("User-Agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 63.0) Gecko / 20100101 Firefox / 63.0");
                req.Node.AddHeader("Accept","text / html,application / xhtml + xml,application / xml; q = 0.9,*/*;q=0.8");
                req.Node.AddHeader("Accept-Language","zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
                req.Node.AddHeader("Accept-Encoding","gzip, deflate");    
                

                var dReq = ((dynamic)req);
                //dReq.Parameter.url = HttpUtility.UrlPathEncode(longUrl);
                var rlt = client.Execute(req);
                var htmlText = encoding.GetString(rlt.RawBytes);
                var matchs = src.ContentRegex.Matches(htmlText);
                foreach (Match match in matchs)
                {
                    if (match.Success)
                    {
                        var ip = match.Groups[1].Value;
                        if (string.IsNullOrEmpty(ip)) continue;
                        if (!string.IsNullOrEmpty(src.ReplaceDotString))
                            ip = ip.Replace(src.ReplaceDotString, "");
                        if (!string.IsNullOrEmpty(src.ReplaceDotString))
                            ip = ip.Replace(src.ReplaceDotString, ".");
                        var port = Convert.ToInt32(match.Groups[2].Value, src.IsHexPortType ? 16 : 10);
                        var newProxy = new WebProxyServerEntity() { FailedNum = 0, IP = ip, Port = port };
                        if (this.ProxyServers.Find(pxy => pxy.IP == newProxy.IP) == null)
                            if (CheckProxy(newProxy))
                                ProxyServers.Add(newProxy);
                    }
                }
            }
            
        }
        public void AutoRetestProxy()
        {
            var proxyEntitys = ProxyServers;            
            ProxyServers.ForEach(proxyEntity => CheckProxy(proxyEntity)) ;           
            SaveProxyServers();
        }

        public bool CheckProxy(WebProxyServerEntity proxyEntity,int timeout=1000)
        {
            var proxyEntitys = ProxyServers;
            var url = "http://humanstxt.org";
            try
            {
                var client = new DynamicRestClient(url);
                var req = client.TakeRequest("/humans.txt");                
                var now = DateTime.Now;
                client.Timeout = 5000; //本機
                if (!string.IsNullOrEmpty(proxyEntity.IP))
                {                    
                    client.Proxy = new WebProxy(proxyEntity.IP, proxyEntity.Port);
                    client.Timeout = timeout;
                    client.ReadWriteTimeout = timeout * 3;//避免某些會卡住 124.219.176.139:47801
                }
                client.ProxyServerEntity = proxyEntity;
                int ct = proxyEntity.FailedNum;
                var rlt = client.Execute2((RestRequest)req, false);
                var info = encoding.GetString(rlt.RawBytes);
                TimeSpan sp = DateTime.Now - now;
                if (!info.Contains("* TEAM *"))
                    proxyEntity.FailedNum = ct + 1;
                else if (proxyEntity.FailedNum >= 0)
                    proxyEntity.FailedNum = -1;
                return true;
            }
            catch//(Exception e)
            {
                proxyEntity.FailedNum = proxyEntity.FailedNum + 1;
                return false;
            }
            
        }
        public DynamicRestClient CreateRestClient(string url, Func<byte[], bool> CheckContent = null)
        {
            var restClient = new DynamicRestClient(url);
            
            restClient.GoodTimespan = GoodTimespan;
            restClient.ProxyServerEntity = NextProxyServer(failedNumForAutoRemove, 0);
            restClient.Timeout = 5000;//本機為proxy
            if (!string.IsNullOrEmpty(restClient.ProxyServerEntity.IP))
            {
                restClient.ReadWriteTimeout = Timeout * 2;//避免某些會卡住 124.219.176.139:47801 
                restClient.Timeout = Timeout;
            }

            restClient.CheckContentEvent += (CheckContent != null) ? CheckContent : CheckHtmlContent;
            restClient.ExecuteErrorEvent += OnRestClientExecuteErrorEvent;
            return restClient;
        }

        private WebProxyServerEntity OnRestClientExecuteErrorEvent(WebProxyServerEntity oldProxy, Exception e)
        {
            oldProxy.FailedNum = oldProxy.FailedNum + 1;
            SaveProxyServers();
            for (int i = 0; i < ProxyServers.Count; i++)
            {
                WebProxyServerEntity proxyServer = NextProxyServer(failedNumForErrorClient, 0);
                if (proxyServer != null)
                    return proxyServer;
            }
            return null;
        }

        private Locker NewLocker(string filePath)
        {
            return new Locker(filePath.Replace(@"\", "-"));
        }

        private List<WebProxyServerEntity> proxyServersList;

        private List<WebProxyServerEntity> ProxyServers
        {
            get
            {
                lock (this)
                {
                    if (proxyServersList == null || proxyServersList.Count == 0)
                    {
                        proxyServersList = LoadProxyServers(csvFilePath);
                        if (this.ResetProxyFailedNum)
                            proxyServersList.ForEach(it => it.FailedNum = 0);
                    }
                }
                return proxyServersList;
            }
            set { proxyServersList = value; }
        }

        private WebProxyServerEntity NextProxyServer(int failedNumContrain, int retryCount)
        {
            lock (this)
            {
                if (pos >= ProxyServers.Count)
                    pos = 0;
                var svc = ProxyServers[pos];
                pos++;
                if (retryCount >= ProxyServers.Count || svc.FailedNum <= failedNumContrain)
                    return svc;
                return NextProxyServer(failedNumContrain, retryCount + 1);
            }
        }

        public void SaveProxyServers()
        {
            for (int i = ProxyServers.Count - 1; i >= 0; i--)
            {
                var item = ProxyServers[i];
                if (AutoRemoveProxyServer && item.FailedNum > this.failedNumForAutoRemove)
                    ProxyServers.RemoveAt(i);
            }
            ProxyServers = ProxyServers.OrderBy(item => item.FailedNum).ToList();
            using (NewLocker(csvFilePath))
                csvTransfer.Save(ProxyServers, csvFilePath);
        }

        public int Timeout { get; set; }
        public int GoodTimespan { get; set; }
        public bool AutoRemoveProxyServer { get; set; }

        public bool ResetProxyFailedNum { get; set; }
        private List<string> errorContents = new List<string>() { "Lightspeed Systems", "License Warning", "Bad Request", "Unable to connect", "not valid", "timed out", "fba_login.cgi" };
        private Encoding encoding = Encoding.GetEncoding("big5");

        private bool CheckHtmlContent(byte[] rltBytes)
        {
            var info = encoding.GetString(rltBytes);
            foreach (var keyWord in errorContents)
            {
                if (info.Contains(keyWord)) return false;
            }
            return true;
        }
        private List<FreeProxySource> GetFreeProxySrc()
        {
            var list = new List<FreeProxySource>(){
                new FreeProxySource()
                {
                    Host = "http://free-proxy.cz",
                    Paths = new string[] { "/zh/proxylist/country/JP/http/ping/all", "/zh/proxylist/country/JP/http/ping/all/2" },
                    ContentRegex = new Regex("(\\d{1,}-\\d{1,}-\\d{1,}-\\d{1,}).{1,500}<span class=\"fport\" style=''>(\\d{1,})</span>"),
                    RemoveString="-"
                },
                new FreeProxySource()
                {
                    Host = "http://www.gatherproxy.com",
                    Paths = new string[] { "/proxylist/country/?c=Japan" },
                    //"PROXY_IP":"(\d{1,4}\.\d{1,4}\.\d{1,4}\.\d{1,4})".{5,50}"PROXY_PORT":"([A-F\d]{2,5})"                    
                    ContentRegex = new Regex("\"PROXY_IP\":\"(\\d{1,4}\\.\\d{1,4}\\.\\d{1,4}\\.\\d{1,4})\".{5,50}\"PROXY_PORT\":\"([A-F\\d]{2,5})\"")
                    ,IsHexPortType=true
                },
                new FreeProxySource()
                {
                    Host = "http://www.gatherproxy.com",
                    Paths = new string[] { "/proxylist/country/?c=United%20States" },
                    //"PROXY_IP":"(\d{1,4}\.\d{1,4}\.\d{1,4}\.\d{1,4})".{5,50}"PROXY_PORT":"([A-F\d]{2,5})"                    
                    ContentRegex = new Regex("\"PROXY_IP\":\"(\\d{1,4}\\.\\d{1,4}\\.\\d{1,4}\\.\\d{1,4})\".{5,50}\"PROXY_PORT\":\"([A-F\\d]{2,5})\""),
                    IsHexPortType = true
                }/*,
                new FreeProxySource()
                {
                    Host = "http://www.freeproxylists.net",
                    Paths = new string[] { "/jp.html" },
                    //%78%79%6c%69%73%74%73%2e%6e%65%74%2f(.{4,200})%2e%68%74%6d%6c%22%3e%34%33%2e%32.{4,200}<td align="center">(\d{2,5})</td>                    
                    ContentRegex = new Regex("%78%79%6c%69%73%74%73%2e%6e%65%74%2f(.{4,200})%2e%68%74%6d%6c%22%3e%34%33%2e%32.{4,200}<td align=\"center\">(\\d{2,5})</td>"),
                    RemoveString="%3",
                    ReplaceDotString="%e"
                }*/
            };
            return list;
        }
        private int pos = 0;
        private CsvTransfer csvTransfer = new CsvTransfer();
        private string csvFilePath;
        private int failedNumForAutoRemove = 9;
        private int failedNumForErrorClient = 2;
    }
}