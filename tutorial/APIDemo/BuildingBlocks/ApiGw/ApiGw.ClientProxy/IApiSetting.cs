////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/1/2020 4:22:22 PM 
// Description: IApiSetting.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGw.ClientProxy
{
    public interface IApiSetting
    {
        string Endpoint { get; set; }
    }
}
