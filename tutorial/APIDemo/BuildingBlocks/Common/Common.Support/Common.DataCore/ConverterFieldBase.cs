////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/1/2009 11:28:11 AM 
// Description: ConverterField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Common.DataContract;

namespace Common.DataCore
{
    public class ConverterFieldBase : IConverterField
    {
        public string Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    Initializing();
                    if (ContentUpdatedEvent != null)
                        ContentUpdatedEvent(this, content);
                }
            }
        }
        public EntityColumn Column { get; set; }
        public EntityRow Row { get; set; }
        public IEntityTableSource SourceProvider { get; set; }
        public event Action<IConverterField, string> ContentUpdatedEvent;
        virtual public void Initializing() { }
       
        private string content;

    }
}
