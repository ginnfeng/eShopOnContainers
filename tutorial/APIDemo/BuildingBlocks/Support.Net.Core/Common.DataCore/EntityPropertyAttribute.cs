////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/8/2009 3:52:55 PM 
// Description: EntityPropertyAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.DataCore
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class EntityPropertyAttribute : System.Attribute
    {
        public EntityPropertyAttribute()
        {
            ColumnIndex = -1;
        }

        public int ColumnIndex { get; set; }
        public string ColumnName { get; set; }
        public string ValidationExp { get; set; }
        public string ValidationErrorMsg { get; set; }
        public bool IsKey { get; set; }
        public string KeyGen
        {
            get{ return keyGen;}
            set
            {
                keyGen=value;
                IsKey=!string.IsNullOrEmpty(keyGen);
            }
        }
        private string keyGen;
    }

}
