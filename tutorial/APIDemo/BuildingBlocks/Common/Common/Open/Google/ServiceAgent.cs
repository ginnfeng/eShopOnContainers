////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/21/2016 10:43:38 AM 
// Description: ServiceAgent.cs  ；NuGet Install Google.Apis.Sheets.v4
// Revisions  :            		
// **************************************************************************** 
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Common.Open.Google
{
    public class ServiceAgent
    {
        public T TakeService<T>(string applicationName)
            where T : BaseClientService,new()
        {            
            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credential,
                ApplicationName = applicationName,
            };            
            T svc = (T)Activator.CreateInstance(typeof(T), new object[] { initializer });            
            return svc;
        }
        static public IConfigurableHttpClientInitializer LoadCredentialFromJson(string jsonFilePath, string[] scopes = null)
        {
            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                return LoadCredentialFromJson(stream, scopes);                
            }
        }
       
        /// <param name="stream">ex: var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText));</param>       
        static public IConfigurableHttpClientInitializer LoadCredentialFromJson(Stream stream, string[] scopes = null)
        {
            return GoogleCredential.FromStream(stream).CreateScoped(scopes ?? defaultScopes);            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p12FilePath"></param>
        /// <param name="user">"sgx-737@sgxproject-1342.iam.gserviceaccount.com"</param>
        /// <param name="pwd"></param>
        /// <param name="scopes"></param>
        static public IConfigurableHttpClientInitializer LoadCredentialFromP12(string p12FilePath,string user,string pwd= null, string[] scopes = null)
        {
            string password = (pwd == null) ? "notasecret" : pwd;
            var certificate = new X509Certificate2(p12FilePath, password, X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(user)
            {
                Scopes = scopes?? defaultScopes
            }.FromCertificate(certificate);
            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);
            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token request failed.");
            return  credential;
        }
        public IConfigurableHttpClientInitializer Credential { get; set; }
        readonly static  string[] defaultScopes =new string[] { "https://docs.google.com/feeds", "https://spreadsheets.google.com/feeds" };
    }
}
