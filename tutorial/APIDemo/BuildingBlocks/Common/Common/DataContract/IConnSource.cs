////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/30/2020 10:05:53 AM 
// Description: IConnSource.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DataContract
{
    public interface IConnSource
    {
        string ConnString { get;}
        TEntity TakeCache<TEntity>() where TEntity : class,new();
        TEntity GenEntityProxy<TEntity>() where TEntity : class;
        public void ForEachSet<TEntity>(TEntity it) where TEntity : class;
    }
    public interface IConnSource<TEntity> : IConnSource
        where TEntity : class
    {
        TEntity Entity { get; }
    }
}
