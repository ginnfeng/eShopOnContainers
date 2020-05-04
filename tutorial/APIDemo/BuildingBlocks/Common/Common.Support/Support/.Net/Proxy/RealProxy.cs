////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 2/25/2011 2:42:30 PM 
// Description: RealProxy.cs  
//          RealProxy<TEntity>在runtime中create產生如下 class TempEntity
//          public class TempEntity:TEntity{
//              public TempEntity(IRealProxy proxy){this._proxy=proxy;}
//              private RealProxyBase _proxy;
//              public void MethodOfTEntity(parameters)
//              {
//                  var thisMethod=(MethodInfo)MethodBase.GetMethodFromHandle(methodHandle);
//                  //在Silverlight中所有用Emit產生的動態assembly須為SecurityTransparent,不能呼叫override method
//                  this._proxy.InvokeSecurityTransparentMethod(thisMethod,parameters);
//              }
//          }

// Revisions  :            		
// **************************************************************************** 
using System;
using System.Reflection;

namespace Support.Net.Proxy
{
    public delegate object InvokeMethodDelegate(MethodInfo methodInfo, ref object[] args);
    public delegate object GetPropertyDelegate(MethodInfo methodInfo, string propertyName);
    public delegate void SetPropertyDelegate(MethodInfo methodInfo, string propertyName, object value);
    

    public class RealProxy<TEntity> : RealProxyBase<TEntity>
    {
        public RealProxy(params Type[] interfaces)
            : base(interfaces)
        {
        }

        public event InvokeMethodDelegate InvokeMethodEvent;
        public event GetPropertyDelegate GetPropertyEvent;
        public event SetPropertyDelegate SetPropertyEvent;

        override public object InvokeMethod(MethodInfo methodInfo, ref object[] args)
        {
            if (methodInfo.IsSpecialName && (GetPropertyEvent != null || SetPropertyEvent!=null))//get&setproperty
            {                
#if SILVERLIGHT
                var methodName = methodInfo.Name.Split(getSetSplitChar);
#else
                //當methed有"_"時會有問題,故加此修正 by Feng
                var methodName = methodInfo.Name.Split(getSetSplitChar,2);
#endif

                var propertyName = methodName[1];
                //當methodName=="set_Item"時,其args.Length==2
                if (args.Length == 1 && SetPropertyEvent != null && methodName[0].Equals("set"))
                {
                    SetPropertyEvent(methodInfo, propertyName, args[0]);
                    return null;
                }
                //當methodName=="get_Item"時,其args.Length==1
                if (args.Length == 0 && GetPropertyEvent != null && methodName[0].Equals("get"))
                {
                    return GetPropertyEvent(methodInfo, propertyName);
                }
                //throw new NotSupportedException(methodInfo.Name);
            }
            if (InvokeMethodEvent != null)
                return InvokeMethodEvent(methodInfo,ref args);
            throw new NotSupportedException(methodInfo.Name);
        }

        static private  readonly char[] getSetSplitChar = new char[] { '_' }; 
    }

    

}
