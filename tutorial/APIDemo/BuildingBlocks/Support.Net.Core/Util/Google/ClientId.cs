////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/14/2016 2:05:53 PM 
// Description: ClientId.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Support.Open.Google
{
    public class ClientId
    {
        public Installed installed { get; set; }
    }
    public class Installed
    {
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public string[] redirect_uris { get; set; }
        public DateTime token_expiry { get; set; }
    }


    public class ServiceAccount
    {
        public string type { get; set; }
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_x509_cert_url { get; set; }
    }

}

