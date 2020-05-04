////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/7/2011 2:05:43 PM 
// Description: DataBean.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Support.DataTransaction;
using System.Collections.ObjectModel;
using Common.DataContract;

namespace Common.DataCore
{
    public class UiDataWrap<TSpec> : ObjectTransaction
        where TSpec : ISpecBase
    {
        public UiDataWrap(TSpec spec)
        {
            Spec = spec;
            dataValue = (ValueType.IsValueType) ? Activator.CreateInstance(ValueType) : null;
        }
        
        public UiDataWrap(TSpec spec,object dataValue)
        {
            Value = dataValue;
            Spec = spec;
        }
        public TSpec Spec { get; private  set; }

        [DataTransaction] 
        public object Value
        {
            get { return dataValue; }
            set 
            {
                if (dataValue ==value) return;
                dataValue = value; 
                base.OnPropertyChanged("Value"); 
            }
        }
        public Type ValueType 
        {
            get { return Type.GetType(Spec.Type); } 
        }

        public ObservableCollection<object> ValueCandidates
        {
            get { return valueCandidates; }
            set
            {
                valueCandidates = value;
                OnPropertyChanged("CandidateList");                
            }
        }
        private ObservableCollection<object> valueCandidates;
        private object dataValue;
    }
}
