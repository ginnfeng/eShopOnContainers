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
    [ApiSpec(typeof(IHelloWorldService), RouteTemplate.API_VER_SVC)]
    public interface IHelloWorldService
    {
        [ApiSpec("hello/{id1}")]
        HelloWeather Hello(string id1, int id2, DateTime id3, HelloInput inp);

        [ApiSpec("HelloGet")]
        string HelloGet(string id1, string id2);

        [ApiSpec("HelloPost")]
        string HelloPost(string id1, string id2);

        [ApiSpec("HelloMQDemo/{id1}", AsQueueName =true)]
        void HelloMQDemo(string id1,HelloInput inp);

        [ApiSpec("")]
        IEnumerable<HelloWeather> DefaultGet();
    }
}
