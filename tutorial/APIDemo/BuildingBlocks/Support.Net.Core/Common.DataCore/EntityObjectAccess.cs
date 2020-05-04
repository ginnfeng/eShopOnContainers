////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/25/2009 3:46:47 PM 
// Description: MethodProviderBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Reflection;
using Support.Net.LINQ;

namespace Common.DataCore
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class EntityObjectAccess : EntityAccessBase
    {
        public EntityObjectAccess()       
        {            
            this.Owner = this;
        }
        public EntityObjectAccess(object owner)               
        {
            this.Owner = owner;
        }      

        public Delegate CreateDelgate(Type delgateType,string methodName)
        {
            Type type = Owner.GetType();
            var methodInfo=type.GetMethod(methodName);
            return Delegate.CreateDelegate(delgateType, methodInfo);
        }

        public Type GetPropertyType(string name)
        {
            var propertyInfo = Owner.GetType().GetProperty(name);
            return propertyInfo.PropertyType;
        }
        public object Owner 
        {
            get { return owner; }
            set
            {
                owner = value;
                base.EntityType = owner.GetType();
            }
        }
        public Delegate CreateMethodDelegate(string methodName)
        {
            return ObjectDelegate.CreateMethodDelegate(Owner.GetType(), methodName);            
        }
        public Delegate CreateGetPropertyDelegate(string propertyName)
        {
            Type type = Owner.GetType();
            PropertyInfo propInfo = type.GetProperty(propertyName);
            if (propInfo == null) throw new MissingMemberException("Not found property " + propertyName);
            return ObjectDelegate.CreateMethodDelegate( propInfo.GetGetMethod());
        }
        public Delegate CreateSetPropertyDelegate(string propertyName)
        {
            Type type = Owner.GetType();
            PropertyInfo propInfo = type.GetProperty(propertyName);
            if (propInfo == null) throw new MissingMemberException("Not found property " + propertyName);
            return ObjectDelegate.CreateMethodDelegate(propInfo.GetSetMethod());
        }

        internal override void DoSetProperty(MethodInfo methodInfo, string propertyName, object value)
        {
            var propertyInfo = Owner.GetType().GetProperty(propertyName);
            propertyInfo.SetValue(Owner, value, null);
        }

        internal override object DoGetProperty(MethodInfo methodInfo, string propertyName)
        {
            var propertyInfo = Owner.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(Owner, null);
        }

        private object owner;
    }
}
