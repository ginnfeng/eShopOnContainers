using System;

namespace Support.ErrorHandling
{
    public class TypeConvertExceptionMessage : ErrorInfoBase
    {
        public TypeConvertExceptionMessage() { }
        public Type TargetType 
        {
            get { return targetType; }
            set { targetType = value; }
        }
        public string TargetTypeName 
        {
            get { return targetTypeName; }
            set { targetTypeName = value; }
        }

        private Type targetType;
        private string targetTypeName;

    }
}
