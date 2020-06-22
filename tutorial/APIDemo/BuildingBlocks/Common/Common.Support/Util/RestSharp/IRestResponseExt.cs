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
        public static T Json2Entity<T>(this IRestResponse it, Func<string, string> contentMethod)
        //where T:class
        {
            return (T)Json2Object<T>(it, contentMethod);
            return JsonConvert.DeserializeObject<T>((contentMethod == null) ? it.Content : contentMethod(it.Content));
        }
        public static object Json2Object<T>(this IRestResponse it, Func<string, string> contentMethod)        
        {
            if (!it.IsSuccessful)
                throw it.ErrorException;
            var returnType = typeof(T);
            if (typeof(bool).Equals(returnType))
                return string.IsNullOrEmpty(it.Content) ? false : it.Content.Contains("true")||it.Content.Contains("OK");
            if (returnType.IsPrimitive || typeof(string).IsAssignableFrom(returnType))
                return it.Content;
            if (typeof(byte[]).IsAssignableFrom(returnType))
                return it.RawBytes;
            return JsonConvert.DeserializeObject<T>((contentMethod == null) ? it.Content : contentMethod(it.Content));
            //return Json2Entity<T>(it, contentMethod);
        }
       
        public static T XmlToObject<T>(this IRestResponse it, Func<string, string> contentMethod = null)
        //where T:class
        {
            return Singleton0<XmlTransfer>.Instance.ToObject<T>((contentMethod == null) ? it.Content : contentMethod(it.Content));
        }
    }
}