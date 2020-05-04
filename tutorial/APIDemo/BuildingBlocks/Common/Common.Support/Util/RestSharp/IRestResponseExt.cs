using Newtonsoft.Json;
using RestSharp;
using Support.Serializer;

////*************************Copyright © 2013 Feng 豐**************************
// Created    : 3/4/2015 4:48:11 PM
// Description: IRestResponse.cs
// Revisions  :
// ****************************************************************************
using System;

namespace Support.Open.RestSharp
{
    public static class IRestResponseExt
    {
        public static T JsonToObject<T>(this IRestResponse it, Func<string, string> contentMethod)
        //where T:class
        {
            return JsonConvert.DeserializeObject<T>((contentMethod == null) ? it.Content : contentMethod(it.Content));
        }

        public static T XmlToObject<T>(this IRestResponse it, Func<string, string> contentMethod = null)
        //where T:class
        {
            return Singleton<XmlTransfer>.Instance.ToObject<T>((contentMethod == null) ? it.Content : contentMethod(it.Content));
        }
    }
}