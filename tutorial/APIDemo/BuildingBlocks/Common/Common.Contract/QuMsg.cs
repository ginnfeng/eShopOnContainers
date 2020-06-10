////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 1:34:23 PM 
// Description: SvcMsg.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Common.Contract
{
    public class QuMsg : IQuCorrleation
    {
        public QuMsg()
        {
        }
        public QuMsg(object parameter)
            : this(new object[] { parameter })
        {
        }
        public QuMsg(object[] parameters)
        {
            if (parameters != null)
                Params = new List<object>(parameters);
        }
        public string CorrleationId { get; set; }
        public string TargetQueue { get; set; }
        public string ReplyQueue { get; set; }
        public string Id { get; set; }
        public string MethodName { get; set; }
        public List<object> Params { get; set; }

        
    }
    
}
