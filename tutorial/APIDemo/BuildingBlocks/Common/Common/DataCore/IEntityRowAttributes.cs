////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/7/2011 11:03:16 AM 
// Description: IEntityRowAttributes.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;

namespace Common.DataCore
{
    public interface IEntityRowAttributes
    {
        OPStatus Status { get; set; }
    }
    static public class OPStatusExtension
    {
        /// <summary>
        /// 當屬於Table的OP操作, it>OPStatus.Add的動作只傳遞Schem不傳table內容
        /// </summary>     
        static public bool IsPassSchema(this OPStatus it)
        {
            return it > OPStatus.Add;
        }
    }
}
