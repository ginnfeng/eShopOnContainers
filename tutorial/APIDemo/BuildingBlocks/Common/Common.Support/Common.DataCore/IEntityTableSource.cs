////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/8/2011 9:58:02 AM 
// Description: IDataTableSource.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.DataContract;

namespace Common.DataCore
{
    public interface IEntityTableSource
    {
        //bool TryGetTable(string providerName, string tableName, out EntityTable table);
        bool TryGetTable(EntityRelation keys, out EntityTable table);
        bool TryGetTable<TEntity>(EntityRelation keys, out EntityTableProxy<TEntity> tableProxy);
        List<EntityCatalog> TakeAllEntityCatalog(bool autoRebuild);
        
        /// <param name="ns"></param>
        /// <returns>若沒找到,Return new EntityCatalog(){Name=ns}</returns>
        EntityCatalog TakeEntityCatalog(string ns);
    }
    public interface IAsynEntityTableSource : IEntityTableSource
    {
        IAsyncResult BeginTryGetTable(EntityRelation keys, AsyncCallback callback, object asyncState);
        EntityTable EndTryGetTable(IAsyncResult result);
        IAsyncResult BeginSync(IList<EntityCatalog> nsCatalogs, AsyncCallback callback, object asyncState);
        List<EntityTable> EndSync(IAsyncResult ar);
    }

    static public class IEntityTableSourceExtension
    {
        static public bool TryGetTable(this IEntityTableSource it,out EntityTable table,string ns,string tableName,params string[] keys)
        {
            var selectCond = new EntityRelation(ns, tableName,keys);
            return it.TryGetTable(selectCond, out table);
        }
        static public bool TryGetTable<TEntity>(this IEntityTableSource it, out EntityTableProxy<TEntity> tableProxy, string ns, string tableName, params string[] keys)
        {
            var selectCond = new EntityRelation(ns, tableName, keys);
            return it.TryGetTable(selectCond, out tableProxy);
        }
    }
}
