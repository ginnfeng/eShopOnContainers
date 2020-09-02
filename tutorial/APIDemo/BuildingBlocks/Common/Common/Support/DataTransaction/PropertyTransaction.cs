using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Common.Support.ErrorHandling;
using System.Xml.Serialization;

namespace Common.Support.DataTransaction
{
#if !SILVERLIGHT
    [Serializable]
#endif

    public class PropertyTransaction : IDataTransaction
    {
        public PropertyTransaction(object propertyOwner)
        {
            PropertyOwner = propertyOwner;           
        }
        
        public object PropertyOwner
        {
            get { return propertyOwner; }
            private set { propertyOwner = value; }
        }
        public PropertyInfo Property 
        {
            get { return property; }
            set { property = value; }
        }

        public  DataTransactionAttribute CustomAttribute 
        {
            get
            {
                if(customAttribute==null)
                {
                    object[] atributes=Property.GetCustomAttributes(typeof(DataTransactionAttribute), false);
                    customAttribute = (atributes.Length == 0) ? new DataTransactionAttribute() : (DataTransactionAttribute)atributes[0];
                }
                return customAttribute;
            }
        }
      
        public object CurrentData
        {
            get{return Property.GetValue(PropertyOwner, null);}            
            set
            {
                if (Property.CanWrite)
                {
                    Property.SetValue(PropertyOwner, value, null);
                    OnPropertyChanged();                     
                }
            }
        }

        public IDataTransaction Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public void PropagateParent(IDataTransaction parent)
        {
            Parent = parent;
            object value = CurrentData;
            if (DataTransactionHelper.IsDataTransactionObject(value))
            {
                DataTransactionHelper.PropagateParent(value,parent);                
            }
        }
        public object OriginalData { get; private set;}


        public void OnPropertyChanged()
        {
            ObjectTransaction it = PropertyOwner as ObjectTransaction; 
            if (it!=null)
                it.OnPropertyChanged(Property.Name);
        }

        #region IDataTransaction Members

        public bool IsUpdating
        {
            get
            {
                object value = CurrentData;
                if (DataTransactionHelper.IsDataTransactionObject(value))
                    return DataTransactionHelper.IsUpdating(value);
                return isUpdating;
            }
        }
        public void BeginUpdate(params object[] ids)
        {
            BeginUpdate(Parent, ids);
        }
        public void BeginUpdate(IDataTransaction parent,params object[] ids)
        {
            Parent = parent;
            object value = CurrentData;
            
            if (DataTransactionHelper.IsDataTransactionObject(value))
            {
                DataTransactionHelper.BeginUpdate(value, parent, ids);
                if(value is IList)
                    Backup(value);
                State = DataTransactionState.Begin;
                return;
            }
            Backup(value);
            
            isUpdating = true;
            State = DataTransactionState.Begin;
        }

        public void CommitUpdate()
        {
            object value = CurrentData;
         
            if (DataTransactionHelper.IsDataTransactionObject(value))
            {
                DataTransactionHelper.CommitUpdate(value);
                State = DataTransactionState.Commit;
                return;
            }
            if (isUpdating)
            {  
                oldPropertyValue = null;
                isUpdating = false;                
                OnPropertyChanged();
                State = DataTransactionState.Commit;
            }
        }

        public void CancelUpdate()
        {
            object value = CurrentData;            
            
            if (DataTransactionHelper.IsDataTransactionObject(value))
            {
                DataTransactionHelper.CancelUpdate(value);
                if (value is IList)
                    Restore(oldPropertyValue);
                State = DataTransactionState.Cancel;
                return;
            }
            if (isUpdating)
            {
                Restore(oldPropertyValue);
                isUpdating = false;
                State = DataTransactionState.Cancel;
            }
        }
        
        public bool IsReadOnly 
        {
            get{ return isReadOnly; }
            set 
            { 
                isReadOnly = value;                
                DataTransactionHelper.SetReadOnly(CurrentData, value);             
            }
        }

        public void RollbackOriginal()
        {
            CancelUpdate();
            if (!canRollbackOriginal)
                return;
            object data = CurrentData;
            
            if (DataTransactionHelper.IsDataTransactionObject(data))
            {
                DataTransactionHelper.RollbackOriginal(data);
                return;
            }            
            CurrentData = OriginalData;
        }

        public void SetAsOriginal()
        {
            canRollbackOriginal = true;
            object data=CurrentData;
            
            if (DataTransactionHelper.IsDataTransactionObject(data))
            {
                DataTransactionHelper.SetAsOriginal(data);
                return;
            }            
            OriginalData = Clone(data);
        }

        public bool IsOriginal
        {
            get
            {
                object data = CurrentData;
                if (DataTransactionHelper.IsDataTransactionObject(data))
                {
                    return DataTransactionHelper.IsOriginalContent(data);
                }
                return ObjectHelper.IsContentEquals(data, OriginalData, true);
            }            
        }

        public List<DataDifference> ChangeDetails
        {
            get
            {               
                object data = CurrentData;
                if (DataTransactionHelper.IsDataTransactionObject(data))
                {
                    return DataTransactionHelper.GetChangeDetails(data);
                }
                if (ObjectHelper.IsContentEquals(data, OriginalData, true))
                    return new List<DataDifference>();
                 IDataOwner dataOwner = PropertyOwner as IDataOwner;
                return new List<DataDifference>()
                 {
                     new DataDifference()
                     {
                         Transaction=this,
                         Mode=DataDifference.ChangeMode.Update,
                         Name=Property.Name,
                         ContentType=Property.PropertyType,
                         Description=Description,　
                         CurrentContent=CurrentData,
                         OriginalContent=OriginalData
                     }
                 };
               
            }
        }

        public bool Validate(out ValidatingExceptionMessage exceptionMessage)
        {          
            
            return DoValidate(CurrentData,out exceptionMessage);
        }
        public string Description 
        {
            get
            {
                IDataOwner dataOwner = PropertyOwner as IDataOwner;
                if (dataOwner != null)
                    return dataOwner.Description;
                if (customAttribute != null)
                    if (!string.IsNullOrEmpty(customAttribute.Comment))
                        return customAttribute.Comment;
                return Property.Name;
            }
        }
        private bool DoValidate(object value,out ValidatingExceptionMessage exceptionMessage)
        {            
            exceptionMessage = null;            

            if (DataTransactionHelper.IsDataTransactionObject(value))
            {
                return DataTransactionHelper.Validate(value,out exceptionMessage);                
            }
            IDataOwner dataOwner = PropertyOwner as IDataOwner;
            bool isPass =
                (dataOwner != null)
                ? dataOwner.Validate(value)
                : (CustomAttribute.IsRequired)
                    ? Validate(value)
                    : true;
            if (!isPass)
            {
                exceptionMessage = new ValidatingExceptionMessage() ;
                exceptionMessage.Reference = PropertyOwner;
                exceptionMessage.ReferenceProperty = this;
            }
            return isPass;
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

        private void Backup(object currentValue)
        {
            oldPropertyValue = Clone(currentValue);
        }
        private void Restore(object oldValue)
        {
            IList list = CurrentData as IList;
            if (list == null || oldValue==null)
            {
                CurrentData = oldValue;
                return;
            }
            if (list.IsReadOnly)
                return;
            list.Clear();
            foreach (var item in oldValue as IList)
            {
                list.Add(item);
            }
        }

        static public bool Validate(object value)
        {
            if ((value == null) || (string.IsNullOrEmpty(value.ToString())))
            {                
                return false;
            }
            
            if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue)
                {                    
                    return false;
                }
            }
            return true;
        }

        static public bool IsEnumerableProperty(object value)
        {
            return DataTransactionHelper.IsEnumerableProperty(value);
        }
        static internal object Clone(object it)
        {
            if ((it == null) || (it.GetType().IsValueType))
                return it;
            IList list = it as IList;
            if (list != null)
            {
                List<object>  cloneList = new List<object>();
                foreach (var item in list)
                {
                    cloneList.Add(item);
                }
                return cloneList;
            }
            if (it.GetType() == typeof(string))
                return it.ToString();
            
            return Support.ObjectHelper.Clone(it);
        }

        private bool isUpdating;//false is default        
        private object oldPropertyValue;
        
        private DataTransactionAttribute customAttribute;
        private IDataTransaction  parent;
        private object propertyOwner;
        private PropertyInfo property;
        private bool isReadOnly;
        private bool canRollbackOriginal;
        private DataTransactionState state = DataTransactionState.None;
    }
}
