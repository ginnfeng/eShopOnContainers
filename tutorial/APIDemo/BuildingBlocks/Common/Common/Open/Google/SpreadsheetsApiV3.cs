////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/14/2016 10:11:34 AM 
// Description: SpreadsheetsApi.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using Google.GData.Client;
//using Google.GData.Spreadsheets;
using Google.Apis.Services;
//using Google.Apis.Drive.v2;


namespace Common.Open.Google
{
         

    public class SpreadsheetsApiV3
    {
        public SpreadsheetsApiV3()
        {

        }
        public void Init(ServiceAgentV3 svcAgent)
        {
            
            /*
            service = svcAgent.TakeService<SpreadsheetsService>("SpreadsheetsApi");
            
            query = new SpreadsheetQuery();
            oSF = service.Query(query);
            
            foreach (SpreadsheetEntry entity in oSF.Entries)
            {                
                var title = entity.Title.Text;
                foreach (WorksheetEntry entry in entity.Worksheets.Entries)
                {
                    var title2=entry.Title.Text;
                }
            }*/
        }
        //private SpreadsheetsService service;
        //rivate SpreadsheetQuery query;
        //private SpreadsheetFeed oSF;
    
    
    }
}
