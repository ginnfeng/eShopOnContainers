////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 2/10/2010 11:58:03 AM 
// Description: AccountHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Security.Principal;

namespace Support.Net.Security
{
    public static class AccountHelper
    {
        static public bool IsDomainUserLogOn()
        {
            WindowsIdentity user = WindowsIdentity.GetCurrent();
            return user.AuthenticationType == "Kerberos";
            //Match match = domainUserRegex.Match(user.Name);
            //return match.Success;
        }
        //static Regex domainUserRegex = new Regex("([^\\\\]{1,})\\\\([^\\\\]{1,})");
    }
}
