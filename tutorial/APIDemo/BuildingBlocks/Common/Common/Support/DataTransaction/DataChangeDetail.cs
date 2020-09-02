////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/9/2008 6:04:57 PM 
// Description: ChangeRecord.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
//using System.Xml.Serialization;

namespace Common.Support.DataTransaction
{
    public class DataDifference
    {
        public enum ChangeMode
        {
            Update,
            Add,            
            Delete
        }        
        public IDataTransaction Transaction { get; set; }
        public ChangeMode Mode { get; set; }
        public string Name { get; set; }
        public object OriginalContent { get; set; }
        public object CurrentContent 
        {
            get { return currentContent; }
            set { currentContent = value; TimeStamp = DateTime.Now; } 
        }
        public DateTime TimeStamp { get; set; }
        public Type ContentType { get; set; }
        public string Description { get; set; }
        private object currentContent;
    }    
}
