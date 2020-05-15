////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/15/2017 3:49:46 PM
// Description: TranslationAPI.cs
// Revisions  :
// ****************************************************************************
using Newtonsoft.Json.Linq;
using RestSharp;
using Support.Open.RestSharp;
using Support.Open.RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Support.Open.Google
{
    /// <summary>
    ///  //https://translation.googleapis.com/language/translate/v2?key={YOUR_API_KEY}
    /// </summary>
    public class ApiTranslation : ApiGoogleBase
    {
        public ApiTranslation(string apiKey)
            : base("https://translation.googleapis.com", "/language/translate/v2/{0}?key={1}", apiKey)
        {
        }

        /// <param name="source"> ""||null 自動偵測語言</param>
        /// <param name="target">ex: en,ja,zh-tw,zh-cn,...,LanguageCodes https://msdn.microsoft.com/zh-tw/library/ms533052(v=vs.85).aspx </param>
        /// <param name="quest"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string Translae(string source, string target, string quest, string format = "text")
        {
            var req = base.TakeRequest<JObject>("");
            req.Node.AddJsonBody(new { source = source, target = target, q = quest, format });
            JObject jObject = Execute(req);

            return jObject["data"]["translations"][0]["translatedText"].Value<string>();
        }

        public string Detect(string quest)
        {
            var req = base.TakeRequest<JObject>("detect");
            req.Node.AddJsonBody(new { q = quest });
            JObject jObject = Execute(req);

            return jObject["data"]["detections"][0][0]["language"].Value<string>();
        }
    }
}