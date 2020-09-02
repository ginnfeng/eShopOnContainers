////*************************Copyright © 2013 Feng 豐**************************
// Created    : 6/21/2016 2:04:14 PM
// Description: ValuesResourceExt.cs
// Revisions  :
// ****************************************************************************
using Common.Support;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;

namespace Common.Open.Google
{
    public static class ValuesResourceExt
    {
        public static SpreadsheetsResource.ValuesResource.UpdateRequest Update2(this ValuesResource it, ValueRange body, string spreadsheetId, string range)
        {
            var request = it.Update(body, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            return request;
        }

        /// <summary>
        /// <param name="preRowNum">前置空row數</param>
        /// <returns></returns>
        public static SpreadsheetsResource.ValuesResource.UpdateRequest Update2<T>(this ValuesResource it, IList<T> entities, string spreadsheetId, string range, int columnRowIdx = 1)
        {
            ValueRange valueRange = new ValueRange();
            valueRange.Values = sheetsEntityHelper.ToValues<T>(entities);
            //sheetsEntityHelper.ColumnRowIdx = columnRowIdx;
            //for (int i = 0; i < sheetsEntityHelper.ColumnRowIdx; i++)
            //{
            //    valueRange.Values.Insert(0, new List<object>());
            //}
            var request = it.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            return request;
        }

        static private SheetsEntityHelper sheetsEntityHelper = Singleton0<SheetsEntityHelper>.Instance;
    }
}