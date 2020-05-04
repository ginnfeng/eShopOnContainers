using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
#if SILVERLIGHT
using Support.Net.Util;
#endif
namespace Support.DataTransaction
{

#if !SILVERLIGHT
    [Serializable]
#endif
    public class ObjectTransactionBasic : IDataTransactionBasic, INotifyPropertyChanged
    {
        public ObjectTransactionBasic()
        {
            ownerObject = this;
        }

        public ObjectTransactionBasic(object owner)
        {
            ownerObject = owner;
        }


        public void FirePropertyChangedEvent()
        {
            foreach (PropertyTransaction item in PropertyDataPairList)
            {
                item.OnPropertyChanged();
            }
        }

        public bool TryGetPropertyTransaction(string name, out PropertyTransaction propertyTransaction)
        {
            if (PropertyDataPairList == null)
            {
                propertyTransaction = null;
                return false;
            }
            propertyTransaction = PropertyDataPairList.Find(property => property.Property.Name.Equals(name));
            return propertyTransaction != null;
        }

        private void BuildTransactionPropertys(object owner)
        {
            //this.ownerObject = owner;
            //有實作IDataTransaction但非繼承自ObjectTransaction的不需處理
            IDataTransactionBasic dt;
            if (TryGet(out dt)) return;

            Type type = owner.GetType();
            List<PropertyInfo> propertyInfoList;
            lock (this)
            {
                if (!propertyDataPairMap.TryGetValue(type, out propertyInfoList))
                {
                    propertyInfoList = new List<PropertyInfo>();
                    List<PropertyInfo> allPropertyInfoList = new List<PropertyInfo>();
                    foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {

                        if (!propertyInfo.IsDefined(typeof(NotDataTransactionAttribute), false))
                            allPropertyInfoList.Add(propertyInfo);
                        if (propertyInfo.IsDefined(typeof(DataTransactionAttribute), false))
                        {
                            propertyInfoList.Add(propertyInfo);
                        }
                    }
                    //若整個Owner的property都沒定DataTransactionAttribute則視同全部Pproperty受Transaction控制
                    if (propertyInfoList.Count == 0)
                        propertyInfoList = allPropertyInfoList;
                    propertyDataPairMap[type] = propertyInfoList;
                }
            }
            propertyDataPairList = new List<PropertyTransaction>();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                PropertyTransaction propertyTransaction = new PropertyTransaction(this.ownerObject);
                propertyTransaction.Property = propertyInfo;
                propertyDataPairList.Add(propertyTransaction);
            }
        }

        #region IDataTransactionBasic Members


        virtual public void BeginUpdate(params object[] ids)
        {
            IDataTransactionBasic dataTa;
            if (TryGet(out dataTa))
            {
                dataTa.BeginUpdate(ownerObject as IDataTransactionBasic, ids);
                State = DataTransactionState.Begin;
                return;
            }
            DataTransactionHelper.BeginUpdate(PropertyDataPairList, ownerObject as IDataTransactionBasic, ids);
            State = DataTransactionState.Begin;
        }


        virtual public void CommitUpdate()
        {
            IDataTransactionBasic dataTa;
            if (TryGet(out dataTa))
            {
                dataTa.CommitUpdate();
                State = DataTransactionState.Commit;
                return;
            }
            DataTransactionHelper.CommitUpdate(PropertyDataPairList);
            State = DataTransactionState.Commit;
        }

        virtual public void CancelUpdate()
        {
            IDataTransactionBasic dataTa;
            if (TryGet(out dataTa))
            {
                dataTa.CancelUpdate();
                State = DataTransactionState.Cancel;
                return;
            }
            DataTransactionHelper.CancelUpdate(PropertyDataPairList);
            State = DataTransactionState.Cancel;
        }

        [NotDataTransaction]
        [XmlIgnore]
        public DataTransactionState State
        {
            get { return state; }
            private set
            {
                if (state != value)
                {
                    state = value;
                    if (StateChangedEvent != null) StateChangedEvent(this);
                }
            }
        }

        public event Action<IDataTransactionBasic> StateChangedEvent;

        #endregion

        #region INotifyPropertyChanged Members

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool TryGet<TTransaction>(out TTransaction dt)
            where TTransaction:class,IDataTransactionBasic
        {
            dt = default(TTransaction);
            if (ownerObject is ObjectTransactionBasic) return false;
            dt = ownerObject as TTransaction;
            return (dt != null);

        }

        protected List<PropertyTransaction> PropertyDataPairList
        {
            private set { propertyDataPairList = value; }
            get
            {
                if (propertyDataPairList == null) this.BuildTransactionPropertys(ownerObject);
                return propertyDataPairList;
            }
        }
        private List<PropertyTransaction> propertyDataPairList;
        private static Dictionary<Type, List<PropertyInfo>> propertyDataPairMap = new Dictionary<Type, List<PropertyInfo>>();
        protected object ownerObject;
        private DataTransactionState state = DataTransactionState.None;


        virtual public List<DataDifference> ChangeDetails
        {
            get { throw new NotImplementedException(); }
        }
    }
}
