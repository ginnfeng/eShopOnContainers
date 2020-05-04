using System;
using System.Collections.Generic;
using System.Reflection;
using Support.ErrorHandling;
using System.Xml.Serialization;
#if SILVERLIGHT
using Support.Net.Util;
#endif
namespace Support.DataTransaction
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ObjectTransaction<T> : ObjectTransaction
    {
        public ObjectTransaction(T owner)
            : base(owner)
        {

        }
        public ObjectTransaction(IDataTransaction parent, T owner)
            : base(parent, owner)
        {
        }
        public T Owner { get { return (T)base.ownerObject; } }
    }
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ObjectTransaction : ObjectTransactionBasic, IDataTransaction
    {
        public ObjectTransaction()
            : base()
        {
        }

        public ObjectTransaction(object owner)
            : base(owner)
        {

        }
        public ObjectTransaction(IDataTransaction parent, object owner)
            : base(owner)
        {
            Parent = parent;
        }



        #region IDataTransaction Members

        virtual public void RollbackOriginal()
        {
            DataTransactionHelper.RollbackOriginal(PropertyDataPairList);
        }
        virtual public void SetAsOriginal()
        {
            DataTransactionHelper.SetAsOriginal(PropertyDataPairList);
        }

        [NotDataTransaction]
        [XmlIgnore]
        override public List<DataDifference> ChangeDetails
        {
            get
            {
                return DataTransactionHelper.GetChangeDetails(PropertyDataPairList);
            }
        }

        [NotDataTransaction]
        [XmlIgnore]
        public bool IsOriginal
        {
            get
            {
                return DataTransactionHelper.IsOriginalContent(PropertyDataPairList);
            }
        }

        [NotDataTransaction]
        [XmlIgnore]
        public bool IsUpdating
        {
            get
            {
                IDataTransaction dataTa;
                if (TryGet(out dataTa))
                    return dataTa.IsUpdating;
                return DataTransactionHelper.IsUpdating(PropertyDataPairList);
            }
        }
        override public void BeginUpdate(params object[] ids)
        {
            BeginUpdate(Parent, ids);
        }
        virtual public void BeginUpdate(IDataTransaction parent, params object[] ids)
        {
            Parent = parent;
            if (IsReadOnly)
                return;
            base.BeginUpdate(ids);
        }


        public static List<DataDifference> GetChangeDetailFromCompare<T>(T currentObject, T originalObject)
             where T : ObjectTransaction, new()
        {
            List<DataDifference> result = new List<DataDifference>();

            Type type = originalObject.GetType();// typeof(original);
            foreach (PropertyInfo field in type.GetProperties())
            {
                if (field.IsDefined(typeof(DataTransactionAttribute), false))
                {
                    if (field.CanRead && field.CanWrite)
                    {
                        object newValue = field.GetValue(currentObject, null);
                        object oldValue = field.GetValue(originalObject, null);
                        if (!ObjectHelper.IsContentEquals(newValue, oldValue, true))
                        {
                            PropertyTransaction tr = null;
                            currentObject.TryGetPropertyTransaction(field.Name, out tr);

                            result.Add(new DataDifference { CurrentContent = newValue, OriginalContent = oldValue, Name = tr.Description });
                        }
                    }
                }
            }


            return result;
        }
        virtual public bool Validate(out ValidatingExceptionMessage exceptionMessage)
        {
            exceptionMessage = null;
            if (IsReadOnly) return true;
            IDataTransaction dataTa;
            if (TryGet(out dataTa))
            {
                bool resultValue = dataTa.Validate(out exceptionMessage);
                ValidationStatus = resultValue;
                return resultValue;
            }
            bool resultItem = DataTransactionHelper.Validate(PropertyDataPairList, out exceptionMessage);
            ValidationStatus = resultItem;
            return resultItem;// DataTransactionHelper.Validate(propertyDataPairList, out exceptionMessage);
        }

        [NotDataTransaction]
        [XmlIgnore]
        public bool ValidationStatus
        {
            get
            {
                return validationStatus;
            }
            set
            {
                validationStatus = value;
                FirePropertyChangedEvent();
                OnPropertyChanged("ValidationStatus");
            }
        }


        [NotDataTransaction]
        [XmlIgnore]
        public virtual bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {

                isReadOnly = value;
                DataTransactionHelper.SetReadOnly(PropertyDataPairList, value);
                FirePropertyChangedEvent();
                OnPropertyChanged("IsReadOnly");
            }
        }

        
        [NotDataTransaction]
        [XmlIgnore]
        public IDataTransaction Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public void PropagateParent(IDataTransaction parent)
        {
            Parent = parent;
            IDataTransaction dataTa;
            if (TryGet(out dataTa))
            {
                dataTa.PropagateParent(ownerObject as IDataTransaction);
            }
            DataTransactionHelper.PropagateParent(PropertyDataPairList, ownerObject as IDataTransaction);
        }
    

        #endregion




        private IDataTransaction parent;
        private bool isReadOnly;
        private bool validationStatus;
    }
}
