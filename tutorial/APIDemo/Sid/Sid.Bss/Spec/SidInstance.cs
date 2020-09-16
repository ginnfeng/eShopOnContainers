////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 9/14/2020 4:50:12 PM 
// Description: SpecInstance.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;


namespace Sid.Bss.Spec
{
    public class SvcCharacteristic<TValue>
    {
        public SvcCharacteristic(string key, TValue value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public object Value { get; set; }
    }
    public class SvcCharacteristic : SvcCharacteristic<object>
    {
        public SvcCharacteristic(string key,object value)
            :base(key,value)
        {

        }
    }
   
    public class SvcInstance
    {
        public SvcInstance()
        {
            Characteristics = new List<SvcCharacteristic>();
        }
        public string SpecId { get; set; }
        public List<SvcCharacteristic> Characteristics { get; set; }
        
        private List<SvcCharacteristic<SvcInstance>> childrens;
        
    }
}
