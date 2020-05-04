using System.Collections.Generic;

namespace Support.ErrorHandling
{

    public interface IErrorInfo
    {
        /// <summary>
        /// Reason必須是enum type
        /// </summary>
        object Reason { get; set; }
        object Reference { get; set; }

        /// <summary>
        /// 自訂哪些欄位是References欲顯示在Message的,default是所有欄位
        /// </summary>
        List<string> DumpingFieldList { get; set; }

        string Message { get;}
        string AppendedMessage { get; set; }
        
    }
}
