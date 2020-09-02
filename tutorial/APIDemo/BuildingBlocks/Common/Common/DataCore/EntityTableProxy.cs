////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/8/2011 11:26:25 AM 
// Description: EntityTableProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.DataContract;
using System.Linq.Expressions;
using CodeExpression = System.Linq.Dynamic.DynamicExpression;
using Common.Support.Net.LINQ;
using Common.Support.DataTransaction;
using Common.Support;

namespace Common.DataCore
{
    public class EntityTableProxy
    {
        public EntityTableProxy(EntityTable entityTable, IEntityTableSource entityTableSource = null)
        {
            LoadTable(entityTable, entityTableSource);           
        }

        [DataTransaction]
        public EntityTable Table { get; protected set; }

        public int Count
        {
            get { return Table.Rows.Count; }
        }
        public IEntityTableSource SourceProvider { get; set; }

        virtual public void LoadTable(EntityTable EntityTable, IEntityTableSource entityTableSource)
        {
            SourceProvider = entityTableSource;
            Table = EntityTable;            
        }        
    }

    public class EntityTableProxy<TEntity> : EntityTableProxy
    {
        public EntityTableProxy()
            : this(EntityTableHelper<TEntity>.CreateDataTable())
        {
        }

        public EntityTableProxy(EntityTable entityTable, IEntityTableSource entityTableSource = null)
            : base(entityTable, entityTableSource)
        {
            if (string.IsNullOrEmpty(this.Table.TableName))
                this.Table.TableName = typeof(TEntity).Name;
        }

        override public void LoadTable(EntityTable entityTable, IEntityTableSource entityTableSource)
        {
            base.LoadTable(entityTable, entityTableSource);
            EntityTableHelper<TEntity>.BindEntity(ref entityTable, SourceProvider);
        }        

        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (EntityRow row in Table.Rows)
            {
                yield return row.GetEntity<TEntity>(SourceProvider);
            }
        }

        public bool Contain(Func<TEntity, bool> func)
        {
            TEntity entity = Find(func);
            return entity != null;
        }
        public bool Contain(string lambdaExpression, params object[] parameters)
        {
            string expression = CommonExtension.StringFormat(lambdaExpression, parameters);
            ParameterExpression api = Expression.Parameter(typeof(TEntity), "entity");
            LambdaExpression e = CodeExpression.ParseLambda(
                new ParameterExpression[] { api }, typeof(bool), expression);
            var exp = e.Compile();
            foreach (var entity in this)
            {
                if ((bool)exp.DynamicInvoke(entity))
                    return true;
            }
            return false;
        }
        public TEntity Find(string key)
        {
            var row=Table.Find(key);
            if (row == null) return default(TEntity);            
            return row.GetEntity<TEntity>();           
            
        }
        public TEntity Find(Func<TEntity, bool> func)
        {
            foreach (var entity in this)
            {
                if (func(entity))  return entity;
            }
            return default(TEntity);
        }

        public List<TEntity> Select(Func<TEntity, bool> func)
        {
            List<TEntity> entityList = new List<TEntity>();
            foreach (var entity in this)
            {
                if (func(entity))
                    entityList.Add(entity);
            }
            return entityList;
        }

        public TEntity Append()
        {
            var entityRow = Table.Append();
            AutoGenKey(ref entityRow);
            //EntityTableHelper<TEntity>.BindRow(Table, entityRow, SourceProvider);
            Regist(entityRow,OPStatus.Add);
            return Last();
        }
        public void Append(TEntity entity)
        {
            object[] valueList = new object[Table.Columns.Count];
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                valueList.SetValue(ObjectDelegate.GetPropertyValue(entity, Table.Columns[i].ColumnName), i);
            }
            var entityRow=Table.Append(valueList);
            Regist(entityRow, OPStatus.Add);
        }
        public void Remove(TEntity entity)
        {
            var proxyInfo=entity as IEntityProxyInfo;
            Table.Remove(proxyInfo.Proxy.Row);
            Regist(proxyInfo.Proxy.Row, OPStatus.Delete);
        }

        public TEntity this[int index]
        {
            get
            {
                return Table.Rows[index].GetEntity<TEntity>(SourceProvider);
            }
        }
        public TEntity First()
        {
            return this[0];
        }
        public TEntity Last()
        {
            return this[Count - 1];
        }
        private void Regist(EntityRow row, OPStatus opStatus)
        {
            var srcProvider = this.SourceProvider as IEntityTableSourceTransMgr;
            if (srcProvider != null)
                srcProvider.Regist(Table, row, opStatus);
        }
        private void AutoGenKey(ref EntityRow row)
        {
            EntityTableHelper.AutoGenKey(ref row);            
        }
    }
}
