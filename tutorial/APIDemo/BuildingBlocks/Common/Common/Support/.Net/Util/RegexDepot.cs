////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/2/2010 4:51:00 PM 
// Description: RegexDepot.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text.RegularExpressions;

namespace Common.Support.Net.Util
{
    public class RegexDepot
    {
        static public void ForeachMatchesGroup(Regex regex, string target, Func<int,int,string,bool> proc)
        {
            foreach (Match match in regex.Matches(target))
            {
                int groupCount=match.Groups.Count;
                for (int i = 0; i < groupCount;i++ )
                {
                    if (!proc(groupCount, i, match.Groups[i].Value)) break;
                }
            }
        }

        /// <summary>
        /// http://www.xyz.com/a.html   => $1= http  $2=www.xyz.com   $3=/doc/public  $4=/xxx.html
        /// </summary>
        static public readonly Regex URL = new Regex(@"^(ftp|http|file)://([^/]+)(/.*)?(/.*)");
    }
}
