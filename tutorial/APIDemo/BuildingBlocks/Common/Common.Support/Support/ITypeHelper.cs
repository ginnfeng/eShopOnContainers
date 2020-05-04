using System;
using System.Reflection;

namespace Support
{
    [Serializable]
    public abstract class TypeHelpBase
    {
        abstract public Assembly GetCurrentAssembly();
        public object CreateSidObject(string typeFullName)
        {
            
            Type t = GetCurrentAssembly().GetType(typeFullName);
            return Activator.CreateInstance(t);
        }

        public Type GetType(string typeFullName)
        {
            return GetCurrentAssembly().GetType(typeFullName);
        }

    }

    [Serializable]
    public class TypeHelper: TypeHelpBase
    {
        public TypeHelper(Type type)
        {
            this.type = type;
        }
        public override Assembly GetCurrentAssembly()
        {
            return type.Assembly;
        }
        Type type;
    }

    [Serializable]
    public class TypeHelper<T> : TypeHelper
        where T:new()
    {
        public TypeHelper()
            :base(typeof(T))
        {
        }
    }

}
