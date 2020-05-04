////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/7/2011 3:02:24 PM 
// Description: EntityRowProxy.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Support.Net.Proxy;
using System.Reflection;
using Common.DataContract;
using Support;
using System.ComponentModel;
using Support.DataTransaction;
using Support.Helper;
using System.Text.RegularExpressions;

namespace Common.DataCore
{
    public interface IEntityRowProxy
    {
        EntityRow Row { get; }
        void OnConverterFieldContentUpdatedEvent(IConverterField field, string newValue);
    }

    public class EntityRowProxy : IEntityRowProxy,INotifyPropertyChanged,IDisposable
    {
        public EntityRowProxy(EntityRow row)
            : this(row, null)
        {
        }
        public EntityRowProxy(EntityRow row, IEntityTableSource sourceProvider)
        {            
            Row = row;
            SourceProvider = sourceProvider;
            Row.PropertyChanged += new PropertyChangedEventHandler(OnFieldValueChanged);
        }

        private void OnFieldValueChanged(object sender, PropertyChangedEventArgs e)
        {
            IConverterField converterField;
            if (fieldMap != null && fieldMap.TryGetValue(e.PropertyName, out converterField))
            {
                var value = Row[e.PropertyName];
                converterField.Content = (value == null) ? null : value.ToString();
                return;
            }
            this.OnPropertyChanged(e.PropertyName);
        }
        protected object InvokeMethod(MethodInfo methodInfo, ref object[] args)
        {
            //目前只處理Special Interface 的get_Item與set_Item的特殊case
            if (args.Length == 1)
            {
                object retValue;                
                TryGetSpecialInterfacePropertyValue(methodInfo, null, out retValue, args);
                return retValue;
            }
            TrySetSpecialInterfacePropertyValue(methodInfo,null,null,args);
            return null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected void SetPropertyValue(MethodInfo methodInfo, string propertyName, object arg)
        {      
            EntityPropertyAttribute attri;
            if (methodInfo != null && AttributeHelper.TryGetCustomAttribute(methodInfo.DeclaringType.GetProperty(propertyName), out attri))
            {
                if (!string.IsNullOrEmpty(attri.ValidationExp))
                {
                    string value = (arg == null) ? "" : arg.ToString();
                    Regex regex = new Regex(attri.ValidationExp);
                    if (!regex.Match(value).Success)
                    {
                        throw new ArgumentException(string.IsNullOrEmpty(attri.ValidationErrorMsg) ? "欄位驗證錯誤" : attri.ValidationErrorMsg, propertyName);
                    }
                }
            }           
            Row[propertyName] = arg;
            //OnPropertyChanged(propertyName);
        }

        protected object GetPropertyValue(MethodInfo methodInfo, string propertyName)
        {
            object retValue;
            if (TryGetSpecialInterfacePropertyValue(methodInfo, propertyName, out retValue))
            {
                return retValue;
            }
            return DoGetPropertyValue(methodInfo, propertyName);

        }
        protected object DoGetPropertyValue(MethodInfo methodInfo, string propertyName)
        {
            var retValue = Row[propertyName];
            if (methodInfo == null) return retValue;
            var propertyType = methodInfo.ReturnType;

            if (typeof(IConverterField).IsAssignableFrom(propertyType))
            {  //當Column.Expression!="" ,才會於此進入此code做Field value轉換 
                if (fieldMap == null) fieldMap = new Dictionary<string, IConverterField>();
                IConverterField converterField;
                if (!fieldMap.TryGetValue(propertyName, out converterField))
                {
                    converterField = (IConverterField)Activator.CreateInstance(propertyType);
                    converterField.SourceProvider = SourceProvider;

                    converterField.Column = Row.Table.Columns[propertyName];
                    converterField.Row = Row;
                    converterField.Content = (retValue == null) ? null : retValue.ToString();
                    converterField.Initializing();
                    converterField.ContentUpdatedEvent += OnConverterFieldContentUpdatedEvent;
                    fieldMap[propertyName] = converterField;
                }
                return converterField;
            }
            if (retValue == null || retValue is DBNull)
            {
                retValue = (propertyType.IsValueType) ? Activator.CreateInstance(propertyType) : null;
            }
            if (retValue == null || propertyType.IsAssignableFrom(retValue.GetType()))
            {
                return retValue;
            }
            return CommonExtension.ToObject(retValue.ToString(), propertyType);
        }

        public void OnConverterFieldContentUpdatedEvent(IConverterField field, string newValue)
        {
            if (field.Column == null) return;
            SetPropertyValue(null, field.Column.ColumnName, newValue);
        }
        private bool TrySetSpecialInterfacePropertyValue(MethodInfo methodInfo, string propertyName, object value, object[] args = null)
        {
            if (typeof(IDictionaryAccess).Equals(methodInfo.DeclaringType))
            {                
                if (methodInfo.Name.Equals("set_Item"))
                {
                    SetPropertyValue(null, args[0].ToString(), args[1]);
                    return true;
                }
            }
            return false;
        }
        private bool TryGetSpecialInterfacePropertyValue(MethodInfo methodInfo, string propertyName, out object value, object[] args = null)
        {
            if (typeof(IEntityProxyInfo).Equals(methodInfo.DeclaringType))
            {            
                if (methodInfo.Name.Equals("get_Proxy"))
                {
                    value = this;
                    return true;
                }
                throw new NotImplementedException(methodInfo.Name);
            }
            if (typeof(IDictionaryAccess).Equals(methodInfo.DeclaringType))
            {
                if (methodInfo.Name.Equals("get_Keys"))
                {
                    var keys = new List<string>();
                    Row.Table.Columns.ForEach(column => keys.Add(column.ColumnName));
                    value = keys;
                    return true;
                }
                if (methodInfo.Name.Equals("get_Item"))
                {
                    value = DoGetPropertyValue(null, args[0].ToString());
                    return true;
                }               
                throw new NotImplementedException(methodInfo.Name);
            }
            if (typeof(INotifyPropertyChanged).Equals(methodInfo.DeclaringType))
            {
                var param=methodInfo.GetParameters();
                //Delegate handler = Delegate.CreateDelegate(param[0].ParameterType, args[0]);
                var handler= args[0] as PropertyChangedEventHandler;
                PropertyChanged += handler;
                value = null;
                return true;
            }
            value = null;
            return false;
        }
        public IEntityTableSource SourceProvider { get; set; }

        [DataTransaction]
        public EntityRow Row { get; private set; }


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
                    Row.PropertyChanged -= OnFieldValueChanged;
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        private Dictionary<string, IConverterField> fieldMap;

    }

    public class EntityRowProxy<TEntity> : EntityRowProxy
    {
        public EntityRowProxy(EntityRow row)
            : this(row, null)
        {
        }
        public EntityRowProxy(EntityRow row, IEntityTableSource sourceProvider)
            : base(row, sourceProvider)
        {
            realProxy.GetPropertyEvent += new GetPropertyDelegate(GetPropertyValue);
            realProxy.SetPropertyEvent += new SetPropertyDelegate(SetPropertyValue);
            realProxy.InvokeMethodEvent += new InvokeMethodDelegate(InvokeMethod);
        }

        public TEntity Entity
        {
            get { return realProxy.Entity; }
        }

        private RealProxy<TEntity> realProxy = new RealProxy<TEntity>(typeof(IEntityProxyInfo), typeof(INotifyPropertyChanged));


    }
}
