////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 9/18/2013 5:55:46 PM 
// Description: DisposableProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Support.Net.Proxy
{
    public interface IDisposableAdapter<T> : IDisposable
    {        
        void AssignEntity(T it);
        void RenewEntity();
        bool IsDamaged { get; set; }
        T Entity { get; }
        
        /// <summary>
        /// 通常包含thread Id
        /// </summary>
        string UsingId { get; }
        string ResourceId { get; }
    }
    
    public class DisposableAdapter<T> : IDisposableAdapter<T>
        where T : class
    {
        public DisposableAdapter(T it)
        {
            AssignEntity(it);
            IsDamaged = false;
        }
        public DisposableAdapter(ref T it)
        {
            AssignEntity(it);
            IsDamaged = false;
        }
        
        public event Action<IDisposableAdapter<T>> DisposeEvent;
        public event Action<IDisposableAdapter<T>> RenewEntityEvent;
        public bool IsDamaged { get; set; }
        public static implicit operator T(DisposableAdapter<T> it)
        {
            return it.Entity;
        }
        
        public void RenewEntity()
        {
            if (RenewEntityEvent != null)
                RenewEntityEvent(this);
        }
        public void AssignEntity(T it)
        {
            Entity = it;
        }
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                    if (DisposeEvent != null) DisposeEvent(this);
                    Entity = null;
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        public T Entity { get; private set; }
        public string UsingId { get; set; }
        public string ResourceId { get; set; }
        
    }
}
