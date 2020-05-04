////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/7/2010 2:09:12 PM 
// Description: SwitchSensor.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.Container
{
    public class SwitchSensor<TSwitchStatus, THost> : SwitchSensor<TSwitchStatus>
        where TSwitchStatus : struct
    {
        private SwitchSensor() { }
        private SwitchSensor(TSwitchStatus status) { }
        public SwitchSensor(THost host, Func<THost, TSwitchStatus> takeStatusMethod, Action<THost, TSwitchStatus> updateStatusMethod)
        {
            Host = host;
            TakeStatusMethod = takeStatusMethod;
            UpdateStatusMethod = updateStatusMethod;
            SetInitStatus();
        }
        public void SetInitStatus()
        {
            base.SetInitStatus(TakeStatusMethod(Host));
            base.IsRepeatDetecting = false;
        }
        public void CommitStatusChange()
        {
            if (IsStatusChanged)
            {
                UpdateStatusMethod(Host, Status);
                SetInitStatus(Status);
            }
        }
        public THost Host { get; private set; }
        private Func<THost, TSwitchStatus> TakeStatusMethod;
        private Action<THost, TSwitchStatus> UpdateStatusMethod;

    }

    public class SwitchSensor<TSwitchStatus> : MatrixSensorBase
        where TSwitchStatus : struct
    {
        public SwitchSensor()
            : this(default(TSwitchStatus))
        {
        }

        public SwitchSensor(TSwitchStatus status)
        {
            SetInitStatus(status);
        }
        public TSwitchStatus Status { get; private set; }
        public TSwitchStatus InitStatus { get; private set; }
        public bool IsStatusChanged { get { return !InitStatus.Equals(Status); } }
        public void SetInitStatus(TSwitchStatus status)
        {
            InitStatus = status;
            Status = status;
        }
        public void SetStatus(TSwitchStatus status)
        {
            if (!status.Equals(Status))
            {
                if (status.Equals(InitStatus))
                {
                    IsRepeatDetecting = true;
                    if (!AllowRepeat) return;
                }
                Status = status;
                this.Triger(null);
            }
        }
        public bool IsRepeatDetecting { get; protected set; }
        public bool AllowRepeat { get; set; }

    }
}
