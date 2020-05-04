////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/7/2008 11:05:04 AM 
// Description: MouseCursor.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Support.Net.WPF
{
    
    public class MouseCursor : IDisposable
    {
        #region IDisposable Members
        public MouseCursor()
        {

            SetCursor(Cursors.Wait);
        }

        /// <param name="cursor">ex: Cursors.AppStarting</param>
        public MouseCursor(Cursor cursor)
        {
            SetCursor(cursor);
        }
        public void SetCursor(Cursor cursor)
        {
            Mouse.SetCursor(cursor);
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
                    Mouse.SetCursor(Cursors.No);
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        

        #endregion
    }
}

#endif
