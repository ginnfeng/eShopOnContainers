////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/6/2011 4:43:13 PM 
// Description: IDataTransactionBasic.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Collections.Generic;

namespace Common.Support.DataTransaction
{
    public enum DataTransactionState
    {
        None,
        Begin,
        Commit,
        Cancel
    }
    public interface IDataTransactionBasic
    {
        [NotDataTransactionAttribute]
        DataTransactionState State { get; }
        void BeginUpdate(params object[] ids);
        void CommitUpdate();
        void CancelUpdate();
        List<DataDifference> ChangeDetails { get; }
        //event Action<IDataTransactionBasic> StateChangedEvent;
    }
}
