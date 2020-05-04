////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/23/2008
// Description: DataTransactionHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;
using System.Collections;
using Support.ErrorHandling;

namespace Support.DataTransaction
{

    public static class DataTransactionHelper
    {
        static class TFuncHandler<T>
        where T : new()
        {
            static public T TempValue { get { return tempValue; } }
            static public bool OnValidate(IDataTransaction ta)
            {
                return ta.Validate(out TFuncHandler<ValidatingExceptionMessage>.tempValue);
            }
          
            static private T tempValue;
        }

        static public bool IsUpdating(object it) 
        {            
            //needAllTrue=false,只要有一個是true就return true,default是return false
            
            //return Exec(it, ta => ta.IsUpdating, false);
            return Exec(it, delegate(IDataTransaction ta) { return ta.IsUpdating; }, false);
        }

        static public void SetReadOnly(object it, bool value)
        {
            //Exec(it, ta => ta.IsReadOnly=value);
            Exec(it, delegate(IDataTransaction ta) { ta.IsReadOnly = value; return true; }, true);
        }
        static public void PropagateParent(object it, IDataTransaction parent)
        {
            Exec(it, delegate(IDataTransaction ta) { ta.PropagateParent(parent); return true; }, true);
        }
        static public void BeginUpdate(object it,IDataTransaction parent, params object[] ids)
        {
            //Exec(it, ta => ta.BeginUpdate(ids));            
            Exec(it, delegate(IDataTransaction ta) { ta.BeginUpdate(parent, ids); return true; }, true);
        }
        static public void BeginUpdate(object it, params object[] ids)
        {
            //Exec(it, ta => ta.BeginUpdate(ids));            
            Exec(it, delegate(IDataTransactionBasic ta) { ta.BeginUpdate(ids); return true; }, true);
        }
        static public void CommitUpdate(object it)
        {
            //Exec(it, ta => ta.CommitUpdate());            
            Exec(it, delegate(IDataTransactionBasic ta) { ta.CommitUpdate(); return true; },true);
        }

        static public void CancelUpdate(object it) 
        {
            //Exec(it, ta => ta.CancelUpdate());            
            Exec(it, delegate(IDataTransactionBasic ta) { ta.CancelUpdate(); return true; }, true);
        }              

        static public void SetAsOriginal(object it)
        {
            //Exec(it, ta => ta.SetAsOriginal());
            Exec(it, delegate(IDataTransaction ta) { ta.SetAsOriginal(); return true; }, true);
        }

        static public bool IsOriginalContent(object it)
        {
            //needAllTrue=true, 只要有一個是false就return false,default是return true
            
            //return Exec(it, ta => ta.IsOriginal(), true);
            return Exec(it, delegate(IDataTransaction ta) { return ta.IsOriginal;},true);
        }

        static public void RollbackOriginal(object it)
        {

            Exec(it, delegate(IDataTransaction ta) { ta.RollbackOriginal(); return true; }, true);
        }

        static public bool Validate(object it, out ValidatingExceptionMessage exceptionMessage)
        {
            bool rlt = Exec<IDataTransaction>(it, TFuncHandler<ValidatingExceptionMessage>.OnValidate, true);
            exceptionMessage = TFuncHandler<ValidatingExceptionMessage>.TempValue;            
            return rlt;
        }
        
        static public List<DataDifference>  GetChangeDetails(object it)
        {
            List<DataDifference> changeSet = new List<DataDifference>();
            Exec(it, delegate(IDataTransaction ta) { changeSet.AddRange(ta.ChangeDetails); return true; }, true);
            return changeSet;
        }

        public delegate bool TFunc<TTrans>(TTrans it) where TTrans : IDataTransactionBasic;
        static public bool Exec<TTrans>(object it, TFunc<TTrans> func, bool needAllTrue)
            where TTrans : class,IDataTransactionBasic
        {            
            if (it == null) return needAllTrue;

            TTrans dataTransaction = it as TTrans;
            if (dataTransaction != null)
            {
                return func(dataTransaction);                
            }
            IList list = it as IList;
            if (list == null) return needAllTrue;
            
            foreach(object item in list)
            {                
                //needAllTrue=true, 只要有一個是false就return false,default是return true
                //needAllTrue=false,只要有一個是true就return true,default是return false                
                if (DataTransactionHelper.Exec(item, func, needAllTrue) != needAllTrue)
                    return !needAllTrue;
            }
            return needAllTrue;
        }

        static public bool IsDataTransactionObject(object it)
        {
            return it is IDataTransactionBasic || (!(it is string) && it is IEnumerable);
        }

        static public bool IsEnumerableProperty(object value)
        {
            return (value != null) && !(value is string) && (value is IEnumerable);
        } 
    }
}
