////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/17/2020 10:13:40 AM 
// Description: LogMessage.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Support.Net.Logger
{
    internal readonly struct LogMessage
    {
        public LogMessage(DateTimeOffset timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }

        public DateTimeOffset Timestamp { get; }
        public string Message { get; }
    }

}
