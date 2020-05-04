////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 8/21/2009 3:46:53 PM 
// Description: GenericTypeHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Reflection;

namespace Support.Net.Util
{
    public class GenericTypeHelper
    {
        public GenericTypeHelper(params Type[] helperTypes)
        {
            converter.RegisterTypeHelper(helperTypes);
        }
        
        /// <param name="genericDefType">ex: typeof(List<>)</param>
        /// <param name="parameterTypes"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CreateInstance(Type genericDefType, string[] typeArguments, object[] parameters)
        {
            converter.RegisterTypeHelper(genericDefType);
            return CreateInstance(genericDefType, converter.GetTypes(typeArguments), parameters);
        }

      
        static public object Invoke(object target, string methodName, object[] parameters)
        {
            Type type=target.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName);
            return methodInfo.Invoke(target, parameters);
        }
        public object Invoke(object target, string methodName, string[] typeArguments, object[] parameters)
        {
            return Invoke(target, methodName, converter.GetTypes(typeArguments), parameters);
        }
        
        public MethodInfo FindMethod(Type type, string methodName, string[] typeArguments)
        {
            return FindMethod(type, methodName, typeArguments, BindingFlags.Public | BindingFlags.Instance);
        }

        public MethodInfo FindMethod(Type type, string methodName, string[] typeArguments, BindingFlags bindingFlags)
        {
            converter.RegisterTypeHelper(type);
            return FindMethod(type, methodName, converter.GetTypes(typeArguments), bindingFlags);
        }

        /// <param name="genericDefType">ex: typeof(List<>)</param>
        /// <param name="parameterTypes"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public object CreateInstance(Type genericDefType, Type[] typeArguments, object[] parameters)
        {
            return genericDefType.CreateGenericInstance(typeArguments, parameters);
        }

        static public MethodInfo FindMethod(Type type, string methodName, Type[] typeArguments)
        {
            return FindMethod(type, methodName, typeArguments, BindingFlags.Public | BindingFlags.Instance);
        }       

        static public object Invoke(object target, string methodName, Type[] typeArguments, object[] parameters)
        {
            Type[] parameterTypes = (parameters == null) ? null : new Type[parameters.Length];
            if (parameterTypes != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterTypes.SetValue((parameters[i] == null) ? null : parameters[i].GetType(), i);
                }
            }
            MethodInfo methodInfo = FindMethod(target.GetType(), methodName, typeArguments, parameterTypes, BindingFlags.Public | BindingFlags.Instance);

            return methodInfo.Invoke(target, parameters);
        }

        static public MethodInfo FindMethod(Type type, string methodName, Type[] typeArguments, BindingFlags bindingFlags)
        {
            return type.FindGenericMethod(methodName, typeArguments, bindingFlags);
        }
        static public MethodInfo FindMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes, BindingFlags bindingFlags)
        {
            return type.FindGenericMethod(methodName, typeArguments, parameterTypes, bindingFlags);
        }

        public void RegisterTypeHelper<T>() where T : new()
        {
            converter.RegisterTypeHelper<T>();
        }
        private SidConverter converter = new SidConverter();
    }
}
