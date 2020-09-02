////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/14/2011 9:44:40 AM 
// Description: IEntityTableSourceMgr.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;
using Common.Support.DataTransaction;

namespace Common.DataCore
{
    public interface IEntityTableSourceMgr
    {
        EntityTable CreateTable<TEntity>(string ns, string tableName);
        EntityTableProxy<TEntity> CreateTableProxy<TEntity>(string ns, string tableName);
#if !SILVERLIGHT
        void SaveAll();
#else
        void CleanAll();
#endif
    }
    public interface IEntityTableSourceTransMgr : IEntityTableSourceMgr
    {
        void Regist(EntityTable entityTable, OPStatus opStatus);
        void Regist(EntityTable srcTable, EntityRow row, OPStatus opStatus);
        IDataTransactionBasic Transaction { get; }
    }
}
