////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 9/15/2020 11:56:06 AM 
// Description: DynamicClassFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Common.Support.Net.Util;

namespace Common.Support.Net.Proxy
{
    public class DynamicClassDescription
    {
        public DynamicClassDescription(string nsName, string clsName)
        {
            this.nsName = nsName;
            this.ClassName = clsName;
            ClassKey = $"DynamicClass_{nsName}_{clsName}";
        }
        public List<MethodInfo> MethodInfos { get; set; }
        public List<PropertyInfo> PropertyInfos { get; set; }
        public List<Type> InterfaceTypes { get; set; }
        public string ClassKey { get;} 
        public string ClassName { get; }
        private string nsName;
    }
    public class DynamicClassDescription<Entity>: DynamicClassDescription
    {
        public DynamicClassDescription()
            :base(typeof(Entity).Assembly.FullName, typeof(Entity).Name)
        {
            var type = typeof(Entity);
            MethodInfos = type.GetAllIncludBaseType<MethodInfo>(t => t.GetMethods());
            PropertyInfos= type.GetAllIncludBaseType<PropertyInfo>(t => t.GetProperties());
            InterfaceTypes= type.GetAllIncludBaseType<Type>(t => t.GetInterfaces());
        }
    }
    public class DynamicClassFactory
    {
        static public DynamicClassFactory Instance=> Singleton<DynamicClassFactory>.Instance;
        
        public DynamicClassFactory()//(DynamicClassDescription classDescription)
        {
            //this.clsDescription = classDescription;
        }
        //private DynamicClassDescription clsDescription;
        
        private Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        public List<MethodInfo> GetProxiableMethods(DynamicClassDescription clsDescription)
        {
            List<MethodInfo> proxiableMethods = clsDescription.MethodInfos??new List<MethodInfo>();
            if (clsDescription.InterfaceTypes == null)
                return proxiableMethods;
            foreach (Type parentType in clsDescription.InterfaceTypes)
            {
                GetProxiableMethods(ref proxiableMethods, parentType);
            }
            return proxiableMethods;
        }
        static public void GetProxiableMethods(ref List<MethodInfo> proxiableMethods, Type type)
        {
            foreach (var parentType in type.GetInterfaces())
            {
                GetProxiableMethods(ref proxiableMethods, parentType);
            }            
            foreach (var methodInfo in type.GetMethods())
            {
                MethodInfo baseMethod = methodInfo.GetBaseDefinition();
                if (!proxiableMethods.Contains(baseMethod))
                {
                    proxiableMethods.Add(baseMethod);
                }
            }
        }
        public List<PropertyInfo> GetProxiablePropertyMap(DynamicClassDescription clsDescription)
        {
            var list = clsDescription.PropertyInfos??new List<PropertyInfo>();
            if (clsDescription.InterfaceTypes == null)
                return list;
            foreach (var type in clsDescription.InterfaceTypes)
            {
                foreach (var propertyInfo in  type.GetProperties())
                {
                    if (!list.Contains(propertyInfo))
                        list.Add(propertyInfo);
                }
            }            
            return list;
        }
        //static public Dictionary<string,PropertyInfo> GetProxiablePropertyMap(Type classType, Type[] hostInterfaces)
        //{
        //    var map = new Dictionary<string,PropertyInfo>();
        //    var addNew =new Action<PropertyInfo>( info=>{ if(info.CanRead)map["get_"+info.Name] = info; if(info.CanWrite)map["set_"+info.Name] = info;});
        //    classType.GetProperties().ForEach(addNew);
        //    hostInterfaces.ForEach(type=>type.GetProperties().ForEach(addNew));
        //    return map;
        //}
        //void AddPropertyBuilder(ref Dictionary<string, PropertyBuilder> map, TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        //{
        //    PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
        //            propertyInfo.Name,
        //            PropertyAttributes.None,
        //            propertyInfo.PropertyType,
        //            Type.EmptyTypes);
        //    propertyBuilder;
        //}
        static private void GenProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder proxyFieldBuilder, MethodInfo getMethodInfo, MethodInfo setMethodInfo)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                    propertyInfo.Name,
                    PropertyAttributes.None,
                    propertyInfo.PropertyType,
                    Type.EmptyTypes);
            if (getMethodInfo != null)
            {
                GenMethod(typeBuilder, getMethodInfo, proxyFieldBuilder, methodBuilder => propertyBuilder.SetGetMethod(methodBuilder));
            }
            if (setMethodInfo != null)
            {
                GenMethod(typeBuilder, setMethodInfo, proxyFieldBuilder, methodBuilder => propertyBuilder.SetSetMethod(methodBuilder));
            }
           
        }
        static private void GenMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder proxyFieldBuilder, Action<MethodBuilder> genProperty = null)
        {
            List<Type> parameterTypes = new List<Type>();
            methodInfo.GetParameters().ForEach(it => parameterTypes.Add(it.ParameterType));
            var methodBuilder = typeBuilder.DefineMethod(
                methodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                methodInfo.CallingConvention,
                methodInfo.ReturnType,
                parameterTypes.ToArray());

            if (genProperty != null) genProperty(methodBuilder);

            if (methodInfo.DeclaringType.IsInterface)
            {
                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }

            foreach (ParameterInfo p in methodInfo.GetParameters())
            {
                methodBuilder.DefineParameter(p.Position + 1, p.Attributes, p.Name);
            }

            ILGenerator generator = methodBuilder.GetILGenerator();


            //OpCodes.Ldtoken instruction pushes a RuntimeHandle for the specified metadata token. A RuntimeHandle can be a fieldref/fielddef, a methodref/methoddef, or a typeref/typedef.
            //The passed token is converted to a RuntimeHandle and pushed onto the stack
            generator.Emit(OpCodes.Ldtoken, methodInfo);
            generator.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) }));
            generator.Emit(OpCodes.Castclass, typeof(MethodInfo));
            LocalBuilder thisMethod = generator.DeclareLocal(typeof(MethodInfo));
            generator.Emit(OpCodes.Stloc, thisMethod);

            // get the delegate that handles this method and store it in
            // a local "mid"

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, proxyFieldBuilder);
            generator.Emit(OpCodes.Ldloc, thisMethod);
            PackParametersToObjects(methodInfo, generator);

            generator.Emit(OpCodes.Call, typeof(RealProxyBase).GetMethod("InvokeSecurityTransparentMethod"));


            if (methodInfo.ReturnType == typeof(void))
            {
                // if this method is declared to return void, clear the
                // object reference returned by the delegate/method from
                // the evaluation stack

                generator.Emit(OpCodes.Pop);
            }
            else if (methodInfo.ReturnType.IsValueType && methodInfo.ReturnType != typeof(void))
            {
                // if the return type is a value type, we must unbox the
                // value the delegate/method returned to us

                generator.Emit(OpCodes.Unbox, methodInfo.ReturnType);
                generator.Emit(OpCodes.Ldobj, methodInfo.ReturnType);
            }
            else
            {
                // otherwise just assert the type of the reference
                generator.Emit(OpCodes.Castclass, methodInfo.ReturnType);
            }
            generator.Emit(OpCodes.Ret);
        }
        // Uses Reflection Emit to create the proxy type.
        public Type CreateType(DynamicClassDescription clsDescription)
        {
            Type type;
            if (typeMap.TryGetValue(clsDescription.ClassKey, out type))
                return type;
            
            
            AssemblyName name = new AssemblyName(clsDescription.ClassKey);
            //AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            //ConstructorInfo securityTransparentCtor = typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes);
            //CustomAttributeBuilder securityTransparentAttriBuilder = new CustomAttributeBuilder(securityTransparentCtor, new object[] { });
            //assembly.SetCustomAttribute(securityTransparentAttriBuilder);

            ModuleBuilder module = assembly.DefineDynamicModule($"{clsDescription.ClassKey}.dll");

            // define a proxy type in the main module
            TypeBuilder typeBuilder = module.DefineType(clsDescription.ClassName, TypeAttributes.Public);
            if(clsDescription.InterfaceTypes!=null)
                clsDescription.InterfaceTypes.ForEach(it => typeBuilder.AddInterfaceImplementation(it));

            // create a field to hold the proxy 
            FieldBuilder proxyFieldBuilder = typeBuilder.DefineField("_proxy", typeof(RealProxyBase), FieldAttributes.InitOnly | FieldAttributes.Private);

            // Define constructors

            ILGenerator generator;

            // define a  constructor       

            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard
                , new Type[] { typeof(RealProxyBase) }
                );

            generator = ctorBuilder.GetILGenerator();

            // invoke the object constructor

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            // create a DynamicProxyHelper for this proxy

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, proxyFieldBuilder);

            generator.Emit(OpCodes.Ret);           
           

            // Define methods            
            var proxiableMethods = GetProxiableMethods(clsDescription);            
            var proxiableProps=GetProxiablePropertyMap(clsDescription);
            foreach (var propertyInfo in proxiableProps)
            {
                string getMethodName="get_" + propertyInfo.Name;
                string setMethodName="set_" + propertyInfo.Name;
                MethodInfo getMethod = (propertyInfo.CanRead) ? proxiableMethods.Find(it => getMethodName.Equals(it.Name)) : null;
                MethodInfo setMethod=(propertyInfo.CanWrite) ? proxiableMethods.Find(it => setMethodName.Equals(it.Name)) : null;
                GenProperty(typeBuilder, propertyInfo, proxyFieldBuilder, getMethod, setMethod);
                if (getMethod != null) proxiableMethods.Remove(getMethod);
                if (setMethod != null) proxiableMethods.Remove(setMethod);
            }

            foreach (MethodInfo methodInfo in proxiableMethods)
            {
                GenMethod(typeBuilder,methodInfo,proxyFieldBuilder);
            }
            type= typeBuilder.CreateType();
            typeMap[clsDescription.ClassKey] = type;
            return type;
        }
        // Emits code to the ILGenerator g to pack the non-this parameters of
        // the method m into a one-dimensional array of objects. The resultant
        // array will be on the top of the evaluation stack when the code this
        // method emits finishes. The values of the parameters are loaded from
        // args, that is, g should be emitting the body of the method m. If m
        // has no non-this parameters, the emitted code will create a zero-
        // length array.
        private static void PackParametersToObjects(MethodInfo methodInfo, ILGenerator generator)
        {
            // create a new object array to accomodate all of the parameters

            generator.Emit(OpCodes.Ldc_I4, methodInfo.GetParameters().Length);
            generator.Emit(OpCodes.Newarr, typeof(object));

            foreach (ParameterInfo p in methodInfo.GetParameters())
            {
                // duplicate the array reference
                generator.Emit(OpCodes.Dup);

                // load the index to insert the parameter into
                generator.Emit(OpCodes.Ldc_I4, p.Position);

                // load the parameter
                generator.Emit(OpCodes.Ldarg, p.Position + 1);

                // if the parameter is a value type, it must be boxed before it
                // can be stored in an array of objects (reference types)
                if (p.ParameterType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, p.ParameterType);
                }

                // store the parameter
                generator.Emit(OpCodes.Stelem_Ref);
            }
        }
    }
}
