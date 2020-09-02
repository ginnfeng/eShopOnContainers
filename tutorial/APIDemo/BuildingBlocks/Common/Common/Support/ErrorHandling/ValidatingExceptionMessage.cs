using System;
using Common.Support.DataTransaction;
using System.Globalization;

namespace Common.Support.ErrorHandling
{
    public class ValidatingExceptionMessage:ErrorInfoBase
    {
        public ValidatingExceptionMessage()
        {
            
        }        

        public PropertyTransaction ReferenceProperty
        {
            get { return referenceProperty; }
            set { referenceProperty = value; }
        }
        public string ErrorMessage()
        {
            return ErrorMessage(CultureInfo.CurrentCulture);
        }
        public string ErrorMessage(IFormatProvider provider)
        {
            if (ReferenceProperty==null)
            {
                return this.AppendedMessage;
            }
            bool aliasName=(ReferenceProperty.CustomAttribute != null  && !string.IsNullOrEmpty(ReferenceProperty.CustomAttribute.Comment));
            return string.Format(provider, "欄位：{0} 未輸入,請檢查！", aliasName ? ReferenceProperty.CustomAttribute.Comment : ReferenceProperty.PropertyOwner.ToString());
        }

        private PropertyTransaction referenceProperty;


    }
}
