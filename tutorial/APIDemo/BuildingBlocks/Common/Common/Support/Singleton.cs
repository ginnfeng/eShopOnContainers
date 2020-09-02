using System;

namespace Common.Support
{
    public static class Singleton<T>
        where T : class//, new()
    {
        private static T instance;
        static public T Create(Func<T> createFunc)
        {
            lock (typeof(T))
            {
                instance = createFunc();
            }
            return instance;
        }

        static public T Instance
        {
            get
            {
                lock (typeof(T))
                {
                    instance ??= Activator.CreateInstance<T>();
                }
                return instance;
            }
        }
       
    }
    public static class Singleton0<T>
        where T : class, new()
    {
        private static T instance;
        static public void Init(params object[] parameters)
        {
            lock (typeof(T))
            {
                instance = (T)Activator.CreateInstance(typeof(T), parameters);
            }
        }

        static public T Instance
        {
            get
            {
                lock (typeof(T))
                {
                    if (instance == null) Init();
                }
                return instance;
            }
        }
        static public void TryNew(ref T it)
        {
            if (it == null) it = new T();            
        }
    }

}
