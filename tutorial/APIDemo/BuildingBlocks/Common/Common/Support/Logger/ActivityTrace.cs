////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/6/2009 10:40:20 AM 
// Description: ActivityEntity.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Common.Support.Logger
{
    public enum ActivityState
    {
        None,
        Start,
        Suspend,
        Resume,
        Stop
    }

    public class ActivityTrace:IDisposable
    {
        public ActivityTrace(TraceSource[] source)
            :this(source,true)
        {
        }
        public ActivityTrace(TraceSource[] source,bool autoStart)
        {
            activityId = Guid.NewGuid();
            traceSource = new List<TraceSource>(source);
            if (autoStart) Start();
        }
        public int Id
        {
            get { return id; }
            set { id=value; }
        }
        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        public void Start()
        {
            if (State != ActivityState.None) return;            
            if (!Guid.Empty.Equals(oldActivityId))
            {
                traceSource.ForEach(delegate(TraceSource src) { src.TraceTransfer(id, "transfer", activityId); });
            }
            Trace.CorrelationManager.ActivityId = activityId;
            traceSource.ForEach(delegate(TraceSource src) { src.TraceEvent(TraceEventType.Start, 0, Name); });
        }

        public void Stop()
        {
            if (State == ActivityState.Stop) return;

            traceSource.ForEach(delegate(TraceSource src) { src.TraceEvent(TraceEventType.Stop, 0, Name); });
            Trace.CorrelationManager.ActivityId = oldActivityId;
        }

        public void Suspend()
        {
            if (State != ActivityState.Start) return;
            traceSource.ForEach(delegate(TraceSource src) { src.TraceEvent(TraceEventType.Suspend, 0, Name); });
        }

        public void Resume()
        {
            if (State != ActivityState.Suspend) return;
            traceSource.ForEach(delegate(TraceSource src) { src.TraceEvent(TraceEventType.Resume, 0, Name); });
        }
        public ActivityState State 
        {
            get { return state; }
            set { state = value; } 
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
                    Stop();
                }
            }
            //resource = null;
            disposed = true;
        }
        private int id;
        private Guid activityId;
        private Guid oldActivityId = Trace.CorrelationManager.ActivityId;
        private string name="activity";
        private bool disposed; 
        private List<TraceSource> traceSource;
        private ActivityState state=ActivityState.None;        
    }
}
