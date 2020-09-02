////*************************Copyright © 2010 Feng**************************	
// Created    : 8/16/2011 11:28:42 AM 
// Description: IForeignFieldBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections;

namespace Common.DataCore
{
    public interface IForeignSetBaseField
    {
        void SetForeignRelation(string ns, string tableName=null);
        bool IsLoadedForeignRelation { get; }
         Type ItemType { get;}
        Action<IForeignSetBaseField> PropagateForeignSettingMethod { get; set; }
        bool IsEnablePropagateForeignSetting { get; set; }
    }
    public interface IForeignSetField : IForeignSetBaseField,IEnumerable
    {
        int Count { get; }
    }
}
