////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/14/2016 3:00:43 PM 
// Description: ClientAgentV3.cs =>舊版
//	    1. 到Google API管理員，建立存取ClientID憑證
//      2. 利用存取憑證，產生OAuth認證授權URL取得AccessCode， 再利用AccessCode產生AccessToken，後續以此token獲得api存取權
//      參考 https://developers.google.com/google-apps/spreadsheets/authorize
//          var svcAgent = new ServiceAgent();
//          svcAgent.LoadOAuth2Parameters("../../Test/clientId.json", clientSecret);
//          string authUrl = svcAgent.CreateOAuth2AuthorizationUrl();//用Browser打開authUrl，取得accessCode來
//          svcAgent.SetupAccessToken(accessCode);//產生AccessToken(會自動過期)
// Revisions  :            		
// **************************************************************************** 
using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Google.GData.Client;
//using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Common.Open.Google
{
    public class ServiceAgentV3
    {
        public ServiceAgentV3()
        {
            parameters = new OAuth2Parameters();
            requestFactory = new GOAuth2RequestFactory(null, "SpreadsheetIntegration-v1", parameters);
            
        }
        public void LoadOAuth2Parameters(string jsonFilePath,string clientSecret=null)
        {            
            parameters.LoadFromJsonClienIdFile(jsonFilePath, clientSecret);
        }
        public string CreateOAuth2AuthorizationUrl()
        {
            return OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);            
        }
        public void SetupAccessToken(string accessCode)
        {
            parameters.AccessCode = accessCode;
            OAuthUtil.GetAccessToken(parameters);            
        }
        public T TakeService<T>(string applicationName)
            where T: Service//,new()
        {
            T svc = (T)Activator.CreateInstance(typeof(T), new object[] { applicationName });             
            svc.RequestFactory = requestFactory;
            return svc;
        }
        public string AccessToken
        {
            get {return parameters.AccessToken; }
            set { parameters.AccessToken = value; }
        }
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "TestSpreadsheet";

        public void Foo()
        {
            GoogleCredential credential;
            var keyFilePath = @"C:\ProgNet\Open\Test\Test\SgxProject-0ba80a93f181.json";// "../../Test/SgxProject-0ba80a93f181.json";
            using (var stream =new FileStream(keyFilePath, FileMode.Open, FileAccess.Read))
            {              
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);                
            }
            // Create Google Sheets API service.
            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            };

            using (var service = new SheetsService(initializer))
            { 
                // Define request parameters.
                String spreadsheetId = "1u3wBMRwj09MvRaXd817p2zcqwTmRgjeYKYNsUKMwsnc";// Demo2
                String range = "Sheet1!A5";  // single cell D5
            
                ValueRange valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { new List<object> { 99,100,101 } }; //{ new List<object>{ 1, 2, 3 } , new List<object> { 4, 5, 6 } };            

                SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                // Prints the names and majors of students in a sample spreadsheet:
                // https://docs.google.com/spreadsheets/d/1u3wBMRwj09MvRaXd817p2zcqwTmRgjeYKYNsUKMwsnc/edit
                var response = request.Execute();
                //IList<IList<Object>> values = response..Values;
                //Console.WriteLine(values);
            }

        }
        public void Foo5()
        {
            //GoogleCredential credential;
            var keyFilePath = "../../Test/SgxProject-c88a95424dbd.p12";
            const string user = "sgx-737@sgxproject-1342.iam.gserviceaccount.com";
            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(user)
            {
                //Scopes = new[] { SheetsService.Scope.Drive, SheetsService.Scope.Spreadsheets, DriveService.Scope.Drive, "https://docs.google.com/feeds", "https://spreadsheets.google.com/feeds" }
                Scopes = new[] { SheetsService.Scope.Drive, SheetsService.Scope.Spreadsheets, "https://docs.google.com/feeds", "https://spreadsheets.google.com/feeds" }
            }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token request failed.");

           

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            
            // Define request parameters.
            String spreadsheetId = "1u3wBMRwj09MvRaXd817p2zcqwTmRgjeYKYNsUKMwsnc";//Demo2
            //String range = "Sheet1!D5";  // single cell D5
            String range = "Sheet1!A5";
            //String myNewCellValue = "Tom";
            ValueRange valueRange = new ValueRange();
            //valueRange.Range = "Sheet1!A5";
            valueRange.Values = new List<IList<object>> { new List<object> { "A","B" } }; //{ new List<object>{ 1, 2, 3 } , new List<object> { 4, 5, 6 } };
            //IList<IList<object>> xx = new List<IList<object>>();
            //xx.Add(new List<object> { "test" });
            //valueRange.Values = xx;
            
            var request = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            //var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            var response = request.Execute();
            
            //IList<IList<Object>> values = response..Values;
            //Console.WriteLine(values);


        }



        /*
        //http://stackoverflow.com/questions/37462887/google-sheets-api-v4-c-sharp-update-a-cell
        // {{::ctrl.pkcs12Password}}
        public SheetsService Foo2()
        {
            string[] scopes = new string[] { "https://spreadsheets.google.com/feeds" }; // Put your scopes here
            var keyFilePath = "../../Test/svcAccount.json";

            //Console.WriteLine("Key File: " + keyFilePath);

            var stream = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read);
            var certificate = GoogleCredential.FromStream(stream);

            ServiceAccountCredential credential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer("ServiceAccount")
              {
                  Scopes = new[] { DriveService.Scope.Drive, "https://spreadsheets.google.com/feeds" }
              }.FromCertificate(certificate)
            );

            credential = credential.CreateScoped(scopes);
            var s=new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "<Your App Name here>"
            };
            return new SpreadsheetsService(s);
        }
        public void Foo()
        {
            //GoogleCredential.FromStream();
            var certificate = new X509Certificate2(@"c:\Diamto Test Everything Project.p12", "notasecret", X509KeyStorageFlags.Exportable);

            const string user = "XXX@developer.gserviceaccount.com";

            var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(user)
            {
                Scopes = new[] { "https://spreadsheets.google.com/feeds" }
            }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token request failed.");

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            var service = new SpreadsheetsService(null) { RequestFactory = requestFactory };

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
                Console.WriteLine("There are no sheets");
            }

            // Iterate through all of the spreadsheets returned
            foreach (SpreadsheetEntry sheet in feed.Entries)
            {
                // Print the title of this spreadsheet to the screen
                Console.WriteLine(sheet.Title.Text);

                // Make a request to the API to fetch information about all
                // worksheets in the spreadsheet.
                WorksheetFeed wsFeed = sheet.Worksheets;

                // Iterate through each worksheet in the spreadsheet.
                foreach (WorksheetEntry entry in wsFeed.Entries)
                {
                    // Get the worksheet's title, row count, and column count.
                    string title = entry.Title.Text;
                    var rowCount = entry.Rows;
                    var colCount = entry.Cols;

                    // Print the fetched information to the screen for this worksheet.
                    Console.WriteLine(title + "- rows:" + rowCount + " cols: " + colCount);

                    // Create a local representation of the new worksheet.
                    WorksheetEntry worksheet = new WorksheetEntry();
                    worksheet.Title.Text = "New Worksheet";
                    worksheet.Cols = 10;
                    worksheet.Rows = 20;

                    // Send the local representation of the worksheet to the API for
                    // creation.  The URL to use here is the worksheet feed URL of our
                    // spreadsheet.
                    WorksheetFeed NewwsFeed = sheet.Worksheets;
                    service.Insert(NewwsFeed, worksheet);
                }
            }
        }
        */
        private OAuth2Parameters parameters;
        private GOAuth2RequestFactory requestFactory;
    }
}
