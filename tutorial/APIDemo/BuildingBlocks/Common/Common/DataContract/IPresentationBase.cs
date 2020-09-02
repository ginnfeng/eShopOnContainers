////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/7/2011 2:48:32 PM 
// Description: IPresentation.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataContract
{
    public interface IPresentationBase
    {
        string Title{get;set;}        
        bool ReadOnly{get;set;}     
        bool Visible{get;set;}        
    }
}
