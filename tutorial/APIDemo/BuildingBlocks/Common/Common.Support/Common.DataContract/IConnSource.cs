////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/30/2020 10:05:53 AM 
// Description: IConnSource.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DataContract
{
    public interface IConnSource
    {
        public string ConnString { get;}
        
    }
    public interface IConnSource<T>: IConnSource
    {
        T Entity { get; }
    }
}
