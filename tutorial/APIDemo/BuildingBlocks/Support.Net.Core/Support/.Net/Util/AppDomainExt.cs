////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 9/18/2013 3:37:47 PM 
// Description: AppDomainProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.Util
{
    static public class AppDomainEx
    {
       
        /// <summary>
        /// T:MarshalByRefObject  確保T object被其他Domain呼叫,但還是執行在原本Domain中.(跨Domain通訊)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="it"></param>
        /// <returns></returns>
        static public T CreateInstance<T>(this AppDomain it)
            where T : MarshalByRefObject,new()
        {            
            //http://www.west-wind.com/weblog/posts/2012/Jan/13/Unable-to-cast-transparent-proxy-to-type-type
            Type t = typeof(T);
            //DailyLogger.Instance.Write(it.SetupInformation.ApplicationBase);
             
            //it.AssemblyResolve+=OnAssemblyResolve; 
             
            //it.Load(t.Assembly.FullName); 
           
            //var obj = it.CreateInstance(t.Assembly.FullName, t.FullName).Unwrap(); 
            return (T)it.CreateInstanceFrom(t.Assembly.Location, t.FullName).Unwrap();            
        }

    }
}
