////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/20/2009 10:40:11 AM 
// Description: ExpressionDelegate.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;
using CodeExpression = System.Linq.Dynamic.DynamicExpression;

namespace Support.Net.LINQ
{
    public partial class ObjectDelegate
    {
        static public object GetPropertyValue(object it, string expression)
        {
            string[] memberItems = SplitExpressionItem(expression, MethodKind.GetProperty);            
            return InvokeMethod(it, memberItems);
        }
        static public void SetPropertyValue(object it, string expression, object value)
        {
            string[] memberItems = SplitExpressionItem(expression, MethodKind.SetProperty);           
            InvokeMethod(it, memberItems, value);
        }
        static public object InvokeMethod(object it, string expression, params object[] parameters)
        {
            string[] memberItems = SplitExpressionItem(expression, MethodKind.Method) ;
            return InvokeMethod(it, memberItems, parameters);
        }
        /// <summary>        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="expression">ex: "prop.ChildProperty.MyMethod"</param>
        /// <returns></returns>
        static public MethodDelegate CompileMethod<T>(Type type, string expression)
        {
            return CompileMethod(typeof(T),expression);
        }
        static public MethodDelegate CompileMethod(Type type, string expression)
        {
            return new MethodDelegate(DoCompileMethod(type, expression, MethodKind.Method), expression);
        }
        static public PropertyDelegate CompileGetProperty<T>(Type type, string expression)
        {
            return CompileGetProperty(typeof(T), expression);
        }
        static public PropertyDelegate CompileGetProperty(Type type, string expression)
        {
            return new PropertyDelegate(DoCompileMethod(type, expression, MethodKind.GetProperty), expression);
        }
        static public PropertyDelegate CompileSetProperty<T>(Type type, string expression)
        {
            return CompileSetProperty(typeof(T), expression);
        }
        static public PropertyDelegate CompileSetProperty(Type type, string expression)
        {
            return new PropertyDelegate(DoCompileMethod(type, expression, MethodKind.SetProperty), expression);
        }
        /// <summary>
        /// example:
        /// class MyClass{ bool Method1(string s1);};
        /// MyClass myObject=new MyClass();
        /// Delegate method1 = EntityObjectAccess.CreateMethodDelegate(myObject,"Method1"); 
        /// bool rlt=(bool)method1.DynamicInvoke(myObject,"parameter");
        ///<!--Func<MyClass,bool> func=(Func<MyClass,bool>) method; -->
        /// </summary>       
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        static public Delegate CreateMethodDelegate(Type objectType, string methodName)
        {
            MethodInfo methodInfo = objectType.GetMethod(methodName);
            if (methodInfo == null) throw new MissingMethodException("Not found method " + methodName);
            return CreateMethodDelegate(methodInfo);
        }
        static public Delegate CreateMethodDelegate<TObject>(string methodName)
        {
            return CreateMethodDelegate(typeof(TObject), methodName);
        }
        static public Delegate CreateMethodDelegate(MethodInfo methodInfo)
        {
            Type objectType = methodInfo.ReflectedType;
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            bool isAction = methodInfo.ReturnParameter.ParameterType.Equals(typeof(void));
            int typeArgumentSize = isAction ? parameterInfos.Length + 1 : parameterInfos.Length + 2;
            Type[] typeArguments = new Type[typeArgumentSize];
            typeArguments.SetValue(objectType, 0);
            if (typeArgumentSize > 1)
                typeArguments.SetValue(methodInfo.ReturnParameter.ParameterType, typeArgumentSize - 1);
            for (int i = 1; i <= parameterInfos.Length; i++)
            {
                typeArguments.SetValue(parameterInfos[i - 1].ParameterType, i);
            }
            Type delgateType = (isAction) ? actionTypes[typeArguments.Length - 1] : funcTypes[typeArguments.Length - 1];
            Type genericType = delgateType.MakeGenericType(typeArguments);
            return Delegate.CreateDelegate(genericType, null, methodInfo);
        }
    }

    public partial class ObjectDelegate
    {
        private enum MethodKind
        {
            Method,GetProperty,SetProperty
        }
        
        static private Delegate DoCompileMethod(Type type, string expression, MethodKind methodKind)
        {
            string[] memberItems = SplitExpressionItem(expression, methodKind);
            const string objectId = "api";
            StringBuilder expressionExe = new StringBuilder(objectId);
            for (int i = 0; i < memberItems.Length; i++)
            {
                expressionExe.Append('.').Append(memberItems[i]);
                if (i == memberItems.Length - 1)
                {
                    expressionExe.Append("(");
                    MethodInfo methodInfo = type.GetMethod(memberItems[i]);
                    if (methodInfo == null) throw new MissingMethodException(type.FullName + " not found method=>" + memberItems[i]);
                    
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>();
                    parameterExpressions.Add(Expression.Parameter(type, objectId));
                    StringBuilder paramStr = new StringBuilder();
                    foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                    {
                        if (paramStr.Length != 0) paramStr.Append(',');
                        paramStr.Append(parameterInfo.Name);
                        parameterExpressions.Add(Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name));
                    }
                    expressionExe.Append(paramStr).Append(")");
                    LambdaExpression exp = CodeExpression.ParseLambda(
                        parameterExpressions.ToArray(), (methodInfo.ReturnType == typeof(void) ? null : methodInfo.ReturnType), expressionExe.ToString());
                    return exp.Compile();
                }
                PropertyInfo propertyInfo = type.GetProperty(memberItems[i]);
                if (propertyInfo == null) throw new MissingMemberException(type.FullName + " not found property=>" + memberItems[i]);
                type = propertyInfo.PropertyType;
                
            }
            throw new Exception();
        }
        static private object InvokeMethod(object it, string[] memberItem, params object[] parameters)
        {
            var propertyValue = it;
            for (int i = 0; i < memberItem.Length; i++)
            {
                Type type = propertyValue.GetType();
                if (i == memberItem.Length - 1)
                {
                    MethodInfo methodInfo = type.GetMethod(memberItem[i]);
                    if (methodInfo == null) throw new MissingMethodException(type.FullName + " not found method=>" + memberItem[i]);
                    return methodInfo.Invoke(propertyValue, parameters);
                }
                PropertyInfo propertyInfo = type.GetProperty(memberItem[i]);
                if (propertyInfo == null) throw new MissingMemberException(type.FullName + " not found property=>" + memberItem[i]);
                propertyValue = propertyInfo.GetValue(propertyValue, null);
                if (propertyValue == null) throw new NullReferenceException(type.FullName + " null reference property=>" + memberItem[i]);
            }
            throw new Exception();
        }   
        static private string[] SplitExpressionItem(string expression, MethodKind methodKind)
        {
            string[] memberItems = expression.Split('.');
            if (methodKind == MethodKind.Method) return memberItems;
            int lastIndex = memberItems.Length - 1;
            string propertyMethodPreName = (methodKind == MethodKind.GetProperty) ? "get_" : "set_";
            memberItems.SetValue(propertyMethodPreName + memberItems[lastIndex], lastIndex);            
            return memberItems;
        }
        
        static private readonly Regex entityExpressionRegex = new Regex(@"([^\.]{1,})[\.]{0,1}(.{0,})");
        static private readonly Regex propertyMethodExpressionRegex = new Regex(@"([^\(\)]{1,})(.{0,})");
        static private readonly Type[] funcTypes = new Type[] { 
            typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>) };
        static private readonly Type[] actionTypes = new Type[] { 
            typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>)};

    }
}
