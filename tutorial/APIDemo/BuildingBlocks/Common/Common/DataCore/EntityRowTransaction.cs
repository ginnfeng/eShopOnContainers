////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/3/2011 3:40:38 PM 
// Description: EntityRowTransaction.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.Support.DataTransaction;
using System.Xml.Serialization;
using Common.DataContract;

namespace Common.DataCore
{
    public class EntityRowTransaction : IDataTransactionBasic
    {
        public EntityRowTransaction(EntityRow row)
        {
            Row = row;
        }
        public EntityRow Row { get; private set; }
        public EntityRow OrignalRow 
        {
            get { return orignalRow; }
            private set { orignalRow = value; } 
        }
        #region IDataTransactionBasic Members


        [NotDataTransaction]
        [XmlIgnore]
        public DataTransactionState State
        {
            get { return state; }
            private set
            {
                if (state == value) return;
                state = value;
                if (StateChangedEvent != null) StateChangedEvent(this);
            }
        }


        public void BeginUpdate(params object[] ids)
        {
            if (!IsUpdating)
            {
                Reset();
                Backup(Row, ref  oldRow);
                State = DataTransactionState.Begin;
            }
        }



        public void CommitUpdate()
        {
            if (IsUpdating)
            {
                State = DataTransactionState.Commit;
                Reset();
            }

        }

        public void CancelUpdate()
        {
            if (IsUpdating)
            {
                State = DataTransactionState.Cancel;
                Restore(oldRow);
                Reset();
            }
        }

        public void RollbackOriginal()
        {
            CancelUpdate();
            if (!canRollbackOriginal)
                return;
            Restore(orignalRow);
        }

        public void SetAsOriginal()
        {
            Backup(Row, ref orignalRow);
            canRollbackOriginal = true;
        }

        public bool IsOriginal
        {
            get
            {
                if (!canRollbackOriginal) return false;
                for (int i = 0; i < orignalRow.ItemArray.Length; i++)
                {
                    if (Row[i] != orignalRow[i]) return false;
                }
                return true;
            }
        }


        public event Action<IDataTransactionBasic> StateChangedEvent;

        #endregion
        private void Backup(EntityRow src, ref EntityRow target)
        {
            target = src.Clone();
        }
        private void Restore(EntityRow srcRow)
        {
            if (srcRow.ItemArray.Length != Row.ItemArray.Length)
                throw new Exception("EntityRowTransaction.Restore()");
            Row.Attributes = srcRow.Attributes;
            srcRow.CopyItermArrayTo(Row);
        }

        private bool IsUpdating { get { return State == DataTransactionState.Begin; } }

        private void Reset()
        {
            oldRow = null;
        }

        private DataTransactionState state;

        private EntityRow oldRow;
        private EntityRow orignalRow;
        private bool canRollbackOriginal;


        public List<DataDifference> ChangeDetails
        {
            get 
            {   
                var attris = Row.GetAttributesEntity();
                if (attris.Status != OPStatus.Steady)
                {
                    var details = new List<DataDifference>();
                    details.Add(new DataDifference() { CurrentContent = Row, OriginalContent = OrignalRow, ContentType = typeof(EntityRow) });
                    return details;
                }                
                return null;
            }
        }
    }
}
