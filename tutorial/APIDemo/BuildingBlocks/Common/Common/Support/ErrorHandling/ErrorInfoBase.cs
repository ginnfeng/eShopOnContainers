using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;
using System.Globalization;

namespace Common.Support.ErrorHandling
{
#if !SILVERLIGHT
    [Serializable]
#endif
    //[DataContract]
    public class ErrorInfoBase:IErrorInfo
    {
        public ErrorInfoBase()
        {
            DumpingFieldList = new List<string>();
            Reason = ErrorInfoIndex.Error;
        }

        #region IErrorInfo Members
        
        public object Reason 
        {
            get { return index; }
            set { index = value; }
        }
        
        public object Reference 
        {
            get
            {
                return sourceObject;
            }
            set
            {
                if ((value != null) && DumpingFieldList == null)
                {
                    DumpingFieldList = new List<string>();
                    foreach (PropertyInfo propertyInfo in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        DumpingFieldList.Add(propertyInfo.Name);
                    }
                }
                sourceObject = value;
            }
        }        
        
        public virtual string Message
        {
            get
            {
                StringBuilder referenceInfo = new StringBuilder();
                if (Reference != null)
                {
                    Type type = Reference.GetType();
                    foreach (string propertyName in DumpingFieldList)
                    {
                        PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        FormatFieldMessage(referenceInfo, Reference, propertyInfo);                        
                    }
                    if (referenceInfo.Length == 0)
                        referenceInfo.Append(Reference.ToString());
                }
                StringBuilder detailInfo = new StringBuilder();
                foreach(PropertyInfo propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if(typeof(IErrorInfo).GetProperty(propertyInfo.Name)==null)
                    {
                        FormatFieldMessage(detailInfo, this, propertyInfo);
                    }
                }
                return string.Format(CultureInfo.CurrentCulture
                    , "*[{0}.{1}({2}): Ref(Type={3},{4})] {5}"
                    , GetType().Name
                    , Reason.ToString()
                    ,detailInfo
                    ,(Reference!=null)?Reference.GetType().Name:""
                    , referenceInfo.ToString()
                    ,AppendedMessage
                    );                
            }
        }
        public string AppendedMessage
        {
            get
            {
                return appendedMessage;
            }
            set
            {
                appendedMessage = value;
            }
        }


        public virtual List<string> DumpingFieldList
        {
            get { return dumpingFieldList; }
            set { dumpingFieldList = value; } 
        }

        #endregion

        protected virtual void FormatFieldMessage(StringBuilder detailInfo,object it,PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                try
                {
                    object value = propertyInfo.GetValue(it, null);
                    if (value != null)
                        detailInfo.Append(string.Format(CultureInfo.CurrentCulture, "{0}={1} ", propertyInfo.Name, value));
                }
                catch  { }
            }
        }
        private string appendedMessage = "";
        private object sourceObject;
        private object index;
        private List<string> dumpingFieldList;
    }
}
