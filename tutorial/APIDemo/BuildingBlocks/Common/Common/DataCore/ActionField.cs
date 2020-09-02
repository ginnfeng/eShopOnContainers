////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/24/2009 5:08:47 PM 
// Description: ActionField.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataCore
{

    public class ActionField<T> : StatementField<T>   
    {
        public void Exec<TParameter>(TParameter api)
            where TParameter : T
        {
            base.Invoke(api);
        }
    }
    //public class ActionField : ActionField<IEntityAccess>
    public class ActionField : ActionField<object>
    {
    }
}
