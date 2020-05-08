////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 5:08:51 PM 
// Description: IHelloWorldService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Microsoft.AspNetCore.Mvc;
using Service.Ordering.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Servic
{
    [ApiSpec("api/[controller]")]
    public interface IHelloWorldService
    {
        [ApiSpec(HTTP.GET, "hello/{id}")]
        string Hello(string id);

        [ApiSpec(HTTP.GET)]
        IEnumerable<HelloWeather> DefaultGet();
    }
}
