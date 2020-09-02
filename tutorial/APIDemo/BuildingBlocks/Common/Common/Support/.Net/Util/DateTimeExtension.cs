////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/12/2011 10:04:41 AM 
// Description: DateTimeExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Support.Net.Util
{
    static public class DateTimeExtension
    {
    	static public string ToStandardString(this DateTime it)
    	{  
            return it.ToString("yyyy/MM/dd HH:mm:ss ");
    	}
    }
}
