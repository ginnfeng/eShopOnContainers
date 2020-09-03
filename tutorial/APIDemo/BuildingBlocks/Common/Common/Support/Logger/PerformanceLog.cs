using System;

namespace Common.Support.Logger
{
    public class PerformanceLog : IDisposable
    {
        #region IDisposable Members
        public PerformanceLog()
        {
            StartTime = DateTime.Now;
            Title = "";
            // SetCursor(Cursors.Wait);
        }
        public PerformanceLog(string title)
        {
            StartTime = DateTime.Now;
            Title = title;
            // SetCursor(Cursors.Wait);
        }
        DateTime StartTime;//= DateTime.Now;

        DateTime EndTime;// = DateTime.Now;


        /// <param name="cursor">ex: Cursors.AppStarting</param>
        public string Title { get; set; }

        public void Dispose()
        {
            Dispose(true);
            EndTime = DateTime.Now;
            TimeSpan ts = EndTime.Subtract(StartTime);
            Console.WriteLine(Title+" Cost Time:" + ts.ToString());
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
                    //Mouse.SetCursor(Cursors.No);

                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;


        #endregion

    }

}
