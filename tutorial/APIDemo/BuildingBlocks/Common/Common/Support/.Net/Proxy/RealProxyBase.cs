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
    public abstract class RealProxyBase : IRealProxy
    {
        abstract public object InvokeMethod(MethodInfo methodInfo, ref object[] args);
        /// <summary>
        ///在Silverlight中所有用Emit產生的動態assembly須為SecurityTransparent,不能呼叫override method
        ///不然會有VerificationException:Operation Could Destabilize the runtime
        ///The following are disallowed in SecurityTransparent code: (http://msdn.microsoft.com/zh-tw/magazine/ee336023.aspx)
        //1.Calling SecurityCritical methods 2.Asserting a permission or permission set 3.Unverifiable code 4.Calling unmanaged code 
        //5.Overriding SecurityCritical virtual methods 6.Implementing SecurityCritical interfaces 7.Deriving from any type that is not SecurityTransparent
        /// </summary>        
        public object InvokeSecurityTransparentMethod(MethodInfo methodInfo, params object[] args)
        {
            return InvokeMethod(methodInfo, ref args);      
        }
    }
    public abstract class RealProxyBase<TEntity> : RealProxyBase
    {
        public RealProxyBase(params Type[] interfaces)
        {
            var entityType = typeof(TEntity);
            if (entityType.IsInterface)
            {
                AddInterfaces(entityType);
            }
//#if SILVERLIGHT  2013/09/11 似乎不止在Sivlerlight其Parent Interface沒被實作,目前只實作含往上一層
            //在silverlight中的data binding無法binding到interface的parent interface之property
            AddInterfaces(entityType.GetInterfaces());
//#endif           
            AddInterfaces(interfaces);
            Entity = Create();
        }

        /// <summary>
        /// Dummy object
        /// </summary>
        public TEntity Entity
        {
            get;
            private set;
        }
        public TEntity Create()
        {
            Type dummyEntityType;
            lock (this)
            {
                dummyEntityType = dynamicTypeFactory.CreateType(this.hostInterfaces.ToArray());
            }
            object entity = Activator.CreateInstance(dummyEntityType, this);
            return (TEntity)entity;
        }
        private void AddInterfaces(params Type[] interfaces)
        {
            if (interfaces == null) return;
            if (hostInterfaces == null) hostInterfaces = new List<Type>();

            foreach (Type type in interfaces)
            {
                if (type != null && type.IsInterface && !hostInterfaces.Contains(type))
                {
                    hostInterfaces.Add(type);
                    continue;
                }
                //throw new ArgumentException("contains invalid type", "");
            }
        }
        static DynamicTypeFactory<TEntity> dynamicTypeFactory = new DynamicTypeFactory<TEntity>();

        private IList<Type> hostInterfaces;

    }
}
