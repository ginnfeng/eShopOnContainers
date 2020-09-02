using Common.Support.ErrorHandling;

namespace Common.Support.DataTransaction
{


    public interface IDataTransaction : IDataTransactionBasic
    {
        [NotDataTransactionAttribute]
        IDataTransaction Parent { get; set; }

        [NotDataTransactionAttribute]
        bool IsReadOnly { get; set; }
        
        [NotDataTransactionAttribute]        
        bool IsUpdating { get; }

        void BeginUpdate(IDataTransaction parent, params object[] ids);      

        void RollbackOriginal();
        void SetAsOriginal();        
        bool IsOriginal{get;}

        void PropagateParent(IDataTransaction parent);
        /// <summary>
        /// 當無ChangeRecord時,return 空的List<ChangeRecord>,而非return null
        /// </summary>
        //List<DataDifference> ChangeDetails { get; }

        bool Validate(out ValidatingExceptionMessage exceptionMessage);

        
    }

    
}
