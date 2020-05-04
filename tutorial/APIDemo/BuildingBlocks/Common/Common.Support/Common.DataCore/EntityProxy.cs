////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/26/2009 5:05:46 PM 
// Description: EntityProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using Support.Net.Proxy;
using System.Reflection;

namespace Common.DataCore
{

    public class EntityProxy<TEntity> 
    {
        public EntityProxy(IEntityAccess entityAcess)
        {
            realProxy.GetPropertyEvent += new GetPropertyDelegate(GetPropertyValue);
            realProxy.SetPropertyEvent += new SetPropertyDelegate(SetPropertyValue);
            this.EntityAcess = entityAcess;            
        }
        private void SetPropertyValue(MethodInfo methodInfo, string propertyName, object value)
        {
            EntityAcess.SetPropertyValue(propertyName, value);
        }

        private object GetPropertyValue(MethodInfo methodInfo, string propertyName)
         {
             return EntityAcess.GetPropertyValue(propertyName);
         }
       
        public IEntityAccess EntityAcess { get; private set; }
        public TEntity Entity
        {
            get { return realProxy.Entity; }
        }
        private RealProxy<TEntity> realProxy = new RealProxy<TEntity>();
    }
}
