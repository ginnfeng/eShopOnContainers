////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/13/2009 4:31:18 PM 
// Description: IDomainAccess.cs  
// Revisions  :            		
// **************************************************************************** 


namespace Common.DataCore
{
    public interface IDomainAction //: IEntityAccess
    {
        IPolicyAccess PolicyAccess { get; set; }
    }
    public class DomainActionBase : IDomainAction//: EntityAccessBase
    {
        //object this[string propertyKey] { get; set; }
        #region IDomainAction Members

        public IPolicyAccess PolicyAccess { get; set; }

        #endregion
    }
}
