using System;

namespace Support
{
    public static class Singleton<T>
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
