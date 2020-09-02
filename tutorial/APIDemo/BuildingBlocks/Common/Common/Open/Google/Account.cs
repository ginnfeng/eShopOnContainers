////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/14/2016 10:20:49 AM 
// Description: Account.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Open.Google
{
    public class Account
    {
        public Account(string id,string pwd)
        {
            Id = id;
            Password = pwd;
        }
        public string Id { get; set; }
        public string Password { get; set; }
    }
}
