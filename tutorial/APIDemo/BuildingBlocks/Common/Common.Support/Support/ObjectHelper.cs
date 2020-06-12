using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using Support.Serializer;

#if !SILVERLIGHT
#endif


namespace Support
{
    public delegate TResult TFunc<TResult>();
    public delegate TResult TFunc<T1,TResult>(T1 t1);
    public delegate TResult TFunc<T1,T2, TResult>(T1 t1,T2 t2);
    public delegate TResult TFunc<T1, T2, T3,TResult>(T1 t1, T2 t2,T3 t3);
    
    public delegate void TAction<T1>(T1 t1);
    public delegate void TAction<T1, T2>(T1 t1, T2 t2);
    public delegate void TAction<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

    public class ObjectHelper
    {
        static public readonly TimeSpan retryTimeSpan = TimeSpan.FromSeconds(2);
        static public T AutoRetryFunc<T>(Func<T> func, int retryCount = 1)
        {
            return AutoRetryFunc<T>(func, retryTimeSpan, retryCount);
        }
        static public T AutoRetryFunc<T>(Func<T> func, TimeSpan ts, int retryCount = 1)
        {
            try
            {
                return func();
            }
            catch (Exception err)
            {
                if (retryCount<=0)
                    throw err;
                System.Threading.Thread.Sleep(ts);
                return AutoRetryFunc<T>(func, ts, retryCount-1);
            }            
        }
        /// <summary>
        /// Copies the specified source to target by properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        /// <returns></returns>
        public static void Copy<T>(T source, T target)
        {
            CopyProperties(source, target);
        }

        public static void CopyProperties(object source, object target)
        {
            CopyProperties(source, target, false);
        }
        public static void CloneCollection<T>(IList<T> source, IList<T> target)
            where T : new()
        {
            foreach (var item in source)
            {
                T clone = Clone(item);
                target.Add(clone);
            }
        }
        public static void CopyProperty(object source, object target, List<string> propertyNames, bool isClone)
        {
            bool isBypassNull=false;
            CopyProperty(source,target, propertyNames, isBypassNull,  isClone);
        }
        public static void CopyProperty(object source, object target, List<string> propertyNames,bool isBypassNull, bool isClone)
        {
            if (source == null || target == null)
                throw new NullReferenceException();
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            foreach (var propertyName in propertyNames)
            {
                PropertyInfo sourcePropertyInfo = sourceType.GetProperty(propertyName);
                PropertyInfo targetPropertyInfo = targetType.GetProperty(propertyName);
                if (sourcePropertyInfo == null || targetPropertyInfo == null)
                    continue;
                object value = sourcePropertyInfo.GetValue(source, null);
                if (value == null && isBypassNull)
                    continue;
                targetPropertyInfo.SetValue(target, (isClone) ? Clone(value) : value, null);
            }
        }
        public static void CopyProperties(object source, object target, bool isClone)
        {
            CopyProperties(source, target,null, isClone);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="propertyFilter">ex:  delegate(PropertyInfo it){ return it.Name!="IsReadOnly"; }</param>
        /// <param name="isClone"></param>
        public static void CopyProperties(object source, object target,TFunc<PropertyInfo,bool> propertyFilter, bool isClone)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();
            if (sourceType == targetType)
            {
                foreach (PropertyInfo field in sourceType.GetProperties())
                {
                    if (propertyFilter != null && !propertyFilter(field))
                        continue;
#if !SILVERLIGHT
                    if (!field.IsDefined(typeof(NonSerializedAttribute), false))
#endif
                    {
                        if (field.CanRead && field.CanWrite)
                        {
                            object value = field.GetValue(source, null);
                            if (value == null)
                            {
                                field.SetValue(target, null, null);
                            }
                            else {
                                field.SetValue(target, (isClone) ? Clone(value) : value, null);
                            }
                            
                        }
                    }
                }
                return;
            }
            PropertyInfo[] sourceFields = sourceType.GetProperties();
            foreach (PropertyInfo sourceField in sourceFields)
            {
                //991125 by sherlock 不同type也應該要支援filter
                if (propertyFilter != null && !propertyFilter(sourceField))
                    continue;
#if !SILVERLIGHT
                if (!sourceField.IsDefined(typeof(NonSerializedAttribute), false))
#endif
                {
                    PropertyInfo targetField = targetType.GetProperty(sourceField.Name);
                  
                        if (targetField == null) 
                            continue;
                        if (!targetField.PropertyType.IsAssignableFrom(sourceField.PropertyType))
                            continue;
                    
                    if (targetField != null)
                    {
                        if (sourceField.CanRead && targetField.CanWrite)
                        {
                            object value = sourceField.GetValue(source, null);
                            if (value == null)
                            {
                                targetField.SetValue(target, (isClone) ? Clone(value) : value, null);
                            }
                            else
                            {
                                if (targetField.PropertyType != value.GetType()
                                    && (IsNullableType(targetField.PropertyType) && GetNonNullableType(targetField.PropertyType) != value.GetType()))
                                {
                                    continue;
                                }
                                targetField.SetValue(target, (isClone) ? Clone(value) : value, null);
                                
                            }
                        }
                    }

                }

            }

        }

        static public T Clone<T>(T source)
        {
            return (T)Clone((object)source);
        }

        static public void Reset<T>(T target)
            where T:class,new()
        {
            CopyProperties(Singleton0<T>.Instance, target, true);  
        }
        static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        static public T[] CloneMany<T>(T sample, int count)
        {
#if !SILVERLIGHT
            return CloneMany<T, BinaryTransfer>(sample, count);
#else
            return CloneMany<T, JsonTransfer>(sample, count);
#endif
        }
        static public T[] CloneMany<T, TTransfer>(T sample, int count)
            where TTransfer : BaseTransfer,new()
        {
            TTransfer transfer = new TTransfer();
            
            using (MemoryStream ms = new MemoryStream())
            {
                transfer.Serialize(sample, ms);                
                T[] objs = new T[count];
                for (int i = 0; i < count; i++)
                {
                    ms.Position = 0;
                    objs.SetValue(transfer.Deserialize<T>(ms), i);
                }
                return objs;
            }
        }        
        static public object Clone(object source)
        {            
            if (source == null) return null;
            return CloneMany(source, 1)[0];
        }        


        /// <summary>
        ///暴力比較法,解object沒有自訂Equals的問題
        /// </summary>
        /// <param name="withBinaryFormatter">true: Object2BinaryString false:Object2XmlString</param>
        /// <returns></returns>        
        static public bool IsContentEquals(object source, object target, bool withBinaryFormatter)
        {
            if (source == target)
                return true;
            if (source == null || target == null)
                return false;
            if (source.Equals(target))
                return true;
            string sourceString = BuildObjectString(source, withBinaryFormatter);
            string targetString = BuildObjectString(target, withBinaryFormatter);
            return sourceString.Equals(targetString);
        }

        static private string BuildObjectString(object source,bool withBinaryFormatter)
        {
#if !SILVERLIGHT
            BaseTransfer transfer = withBinaryFormatter ? new BinaryTransfer() : (BaseTransfer)new XmlTransfer();
            return Encoding.UTF8.GetString(transfer.ToBytes(source));
#else
            BaseTransfer transfer = new JsonTransfer();
            var chars = transfer.ToBytes(source);
            return Encoding.UTF8.GetString(chars, 0, chars.Length);
#endif
        }
    }
}
