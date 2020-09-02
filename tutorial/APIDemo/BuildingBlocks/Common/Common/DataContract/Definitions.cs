////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/9/2011 9:53:13 AM 
// Description: Definitions.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataContract
{
    public enum EntityFieldType
    {
        Var,
        Foreign,
        NSGrp,
        Function,
        Action,
        Dictionary,
        List
    }

    public enum OPStatus
    {
        Steady, Add, Update, Delete, Copy
    }
}
