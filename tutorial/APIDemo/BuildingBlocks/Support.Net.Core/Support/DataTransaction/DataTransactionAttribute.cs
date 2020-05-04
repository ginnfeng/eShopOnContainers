using System;

namespace Support.DataTransaction
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public  class DataTransactionAttribute : System.Attribute
    {
        public DataTransactionAttribute()
        {
            Comment = "";
        }
        public bool IsRequired 
        {
            get { return isRequired; }
            set { isRequired = value; }
        }
        public object EmptyValue
        {
            get { return emptyValue; }
            set { emptyValue = value; }
        }
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private bool isRequired ;
        private object emptyValue;
        private string comment;

    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class NotDataTransactionAttribute : System.Attribute
    {
        public NotDataTransactionAttribute()
        {
        }
    }    
}
