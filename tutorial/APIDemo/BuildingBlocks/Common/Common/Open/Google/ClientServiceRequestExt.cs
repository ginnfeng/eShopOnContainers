////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/22/2016 11:44:28 AM
// Description: ClientServiceRequestExt.cs
// Revisions  :
// ****************************************************************************
using Common.Support;
using Google.Apis.Requests;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;

namespace Common.Open.Google
{
    public static class ClientServiceRequestExt
    {
        static public List<T> Execute<T>(this ClientServiceRequest<ValueRange> it)
            where T : new()
        {
            var response = it.Execute();
            return Singleton0<SheetsEntityHelper>.Instance.ToEntities<T>(response.Values);
        }
    }
}