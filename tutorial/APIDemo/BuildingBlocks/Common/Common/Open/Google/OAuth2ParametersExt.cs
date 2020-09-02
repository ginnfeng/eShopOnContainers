////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/14/2016 2:19:43 PM 
// Description: OAuthUtilExt.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Open.Serializer;
using Google.GData.Client;



namespace Common.Open.Google
{
    public static class OAuth2ParametersExt
    {
        public static void LoadFromJsonClienIdFile(this OAuth2Parameters it,string path,string clientSecret=null)
        {
            //查看及管理您在 Google 雲端硬碟中的試算表 ,查看及管理您在 Google 雲端硬碟中的文件和檔案
            //string scope = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

            //查看及管理您在 Google 雲端硬碟中的試算表 
            string scope = "https://spreadsheets.google.com/feeds";
            var cId =transfer.Load<ClientId>(path);
            it.ClientId = cId.installed.client_id;
            it.ClientSecret = !string.IsNullOrEmpty(clientSecret)? clientSecret:cId.installed.client_secret;
            it.RedirectUri = cId.installed.redirect_uris[0];
            it.TokenExpiry = cId.installed.token_expiry;
            it.Scope = scope;
        }
        static private JsonNetTransfer transfer = new JsonNetTransfer();
    }
}
