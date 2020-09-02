using RestSharp;
using Common.Support.Net.Proxy;

////*************************Copyright © 2013 Feng 豐**************************
// Created    : 3/4/2015 10:44:03 AM
// Description: DynamicRestRequest.cs
// Revisions  :
// ****************************************************************************
using System;
using System.Dynamic;

namespace Common.Open.RestSharp
{
    public class DynamicRestRequest : DynamicXmlBase<RestRequest>
    {
        public DynamicRestRequest()
            : base(new RestRequest())
        { }

        public DynamicRestRequest(string resource, Method method = Method.GET)
            : base(new RestRequest(resource, method))
        { }

        public DynamicRestRequest(Uri resource, Method method = Method.GET)
            : base(new RestRequest(resource, method))
        { }

        public static implicit operator RestRequest(DynamicRestRequest it)
        {
            return it.Node;
        }

        public static implicit operator DynamicRestRequest(RestRequest it)
        {
            return new DynamicRestRequest() { Node = it };
        }

        public DynamicRestRequest<T> Clone<T>()
        //where T : class
        {
            return new DynamicRestRequest<T>() { Node = Node, ContentProcessMethod = ContentProcessMethod };
        }

        public Func<string, string> ContentProcessMethod { get; set; }

        override public string Value
        {
            get { return base.Node.Resource; }
            set { base.Node.Resource = value; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.Equals("Parameter"))
            {
                //AddParameter會自動根據POST或GET自動擺放parameter
                result = new DynamicProperty<RestRequest>(Node, (node, k, v) => node.AddParameter(k, v.ToString()), null);
                return true;
            }
            if (binder.Name.Equals("Header"))
            {
                result = new DynamicProperty<RestRequest>(Node, (node, k, v) => node.AddHeader(k, v.ToString()), null);
                return true;
            }
            if (binder.Name.Equals("UrlSegment"))
            {
                result = new DynamicProperty<RestRequest>(Node, (node, k, v) => node.AddUrlSegment(k, v.ToString()), null);
                return true;
            }

            if (binder.Name.Equals("File"))
            {
                result = new DynamicProperty<RestRequest>(Node, (node, k, v) => node.AddFile(k, v.ToString()), null);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder.Name.Equals("JsonBody"))
            {
                Node.AddJsonBody(value);
                return true;
            }
            if (binder.Name.Equals("Body") || binder.Name.Equals("XmlBody"))
            {
                Node.AddXmlBody(value);
                return true;
            }
            return false;
        }
    }

    public class DynamicRestRequest<TResult> : DynamicRestRequest
    //where TResult : class
    {
        public Func<IRestResponse, TResult> ContentConvertMethod { get; set; }
        public bool IgnoreException { get; set; }
    }
}