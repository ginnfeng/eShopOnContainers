////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/3/2011 11:08:30 AM 
// Description: RealProxyBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Support.Net.Proxy
{

    public abstract class RealProxyBase2<TEntity> : RealProxyBase
    {
        public RealProxyBase2(DynamicClassDescription clsDescription)
        {
            this.clsDescription = clsDescription;
        }
        /// <summary>
        /// Dummy object
        /// </summary>
        public TEntity Entity {
            get{
                entity ??= Create();
                return entity;
            }
         }

        public TEntity Create()
        {
            Type dummyEntityType;
            lock (this)
            {
                dummyEntityType = dynamicFactory.CreateType(clsDescription);
            }
            object entity = Activator.CreateInstance(dummyEntityType, this);
            return (TEntity)entity;
        }
        private DynamicClassFactory dynamicFactory = DynamicClassFactory.Instance;
        private DynamicClassDescription clsDescription;
        private TEntity entity;
    }
}
