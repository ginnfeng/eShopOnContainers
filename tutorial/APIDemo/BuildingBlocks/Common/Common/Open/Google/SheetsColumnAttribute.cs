////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/21/2016 2:36:18 PM 
// Description: SheetsEntityAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Open.Google
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class SheetsColumnAttribute : System.Attribute
    {
        public SheetsColumnAttribute()
        {
        }
        public char Idx { get; set; }
        public string Name { get; set; }
        public int GetIdxNum() { return Idx - 'A'; }
        public Type ValueType { get; internal set; }
    }
    
}
