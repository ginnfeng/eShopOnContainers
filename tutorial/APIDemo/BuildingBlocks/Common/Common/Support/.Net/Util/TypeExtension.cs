////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 8/21/2009 5:51:05 PM 
// Description: TypeExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Support.Net.Util
{
    public static class TypeExtension
    {
        // <param name="genericDefType">ex: typeof(List<>)</param>
        /// <param name="parameterTypes"></param>
        /// <param name="args"></param>
        static public object CreateGenericInstance(this Type genericDefType, Type[] typeArguments, object[] args)
        {
            Type genericType = genericDefType.MakeGenericType(typeArguments);
            return Activator.CreateInstance(genericType, args);
        }

        static public MethodInfo FindGenericMethod(this Type type, string methodName, Type[] typeArguments)
        {
            return type.FindGenericMethod(methodName, typeArguments, BindingFlags.Public | BindingFlags.Instance);
        }

        static public MethodInfo FindGenericMethod(this Type type, string methodName, Type[] typeArguments, BindingFlags bindingFlags)
        {
            //MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);
            return FindGenericMethod(type, methodName, typeArguments, null, bindingFlags);
        }

        static public MethodInfo FindGenericMethod(this Type type, string methodName, Type[] typeArguments, Type[] parameterTypes, BindingFlags bindingFlags)
        {
            //當(parameterTypes == null),若超過一個以上同名method name會產生AmbiguousMatchException
            MethodInfo methodInfo = (parameterTypes == null) ?
                type.GetMethod(methodName, bindingFlags)
                : type.GetMethod(methodName, parameterTypes);
            return methodInfo.MakeGenericMethod(typeArguments);
        }
        static public object DefaultValue(this Type it)
        {
            Type type = typeof(TypeExtension);
            var methodInfo = type.FindGenericMethod("DefaultValue", new Type[] { it }, emptyTypes, BindingFlags.Static | BindingFlags.Public);
            return methodInfo.Invoke(null, null);

        }
        static public T DefaultValue<T>()
        {
            return default(T);
        }

        static public bool HasDefaultConstructor(this Type it)
        {
            return GetDefaultConstructor(it) != null;
        }

        static public ConstructorInfo GetDefaultConstructor(this Type it)
        {
            return it.GetConstructor(Type.EmptyTypes);
        }

        static public bool IsPrimitiveXmlType(this Type type)
        {
            //typeof(DateTime).IsPrimitive=false,IsValueType=true
            //typeof(string).IsPrimitive=false,IsValueType=false
            bool isPrimitive = type.IsPrimitive || type.IsValueType || (type == typeof(string));
            return isPrimitive;
        }
        static public object ConvertFrom<T>(this Type it, T fromValue)
        {
            Type type = typeof(Convert);
            string methodName=Support.CommonExtension.StringFormat("To{0}",it.Name);
            var method=type.GetMethod(methodName, new Type[] { typeof(T) });
            if (method == null)
                throw new InvalidOperationException("TypeExtension.ConvertFrom<T>()");
            return method.Invoke(null, new object[]{fromValue});
           
        }
        static public List<PropertyInfo> GetPropertyInfosOfInterfaces(this Type it, bool includSelf=true, BindingFlags bindingAttr = BindingFlags.Instance|BindingFlags.Public)
        {
            var entityPropertyInfos = new List<PropertyInfo>();
            if (includSelf)
                entityPropertyInfos.AddRange(it.GetProperties(bindingAttr));
            foreach (var entityType in it.GetInterfaces())
            {
                foreach (var property in entityType.GetProperties(bindingAttr))
                {
                    if (entityPropertyInfos.Find(it2 => it2.Name.Equals(property.Name)) == null)
                        entityPropertyInfos.Add(property);
                }
            }
            return entityPropertyInfos;

        }
        static public TInfo FindIncludBaseType<TInfo>(this Type it,Func<Type, TInfo> cond)
            where TInfo:class
        {
            TInfo info = cond(it);
            if (info!=null) return info;
            var baseInterfacs = it.GetInterfaces();
            foreach (var baseInterface in baseInterfacs)
            {
                info=baseInterface.FindIncludBaseType(cond);
                if (info != null) return info;
            }
            return null;
        }
        static public List<TInfo> GetAllIncludBaseType<TInfo>(this Type it, Func<Type, TInfo[]> cond)
            where TInfo : class
        {
            List<TInfo> list = new List<TInfo>();
            list.AddRange(cond(it));
            
            var baseInterfacs = it.GetInterfaces();
            foreach (var baseInterface in baseInterfacs)
            {
                list.AddRange(baseInterface.GetAllIncludBaseType(cond));
            }
            return list;
        }


        static public bool IsXmlString(string it)
        {
            return it.Contains("<?xml");
        }

        private static Type[] emptyTypes = Type.EmptyTypes;
    }

}
