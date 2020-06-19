////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 5:08:51 PM 
// Description: IHelloWorldService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Service.HelloWorld.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.HelloWorld.Contract.Servic
{
    public interface IHelloService
    {
        [QuSpec]
        [ApiSpec("OneWayCall")]
        void OneWayCall(string id1, HelloInput inp);
    }
    public interface IHelloQuService: IHelloService
    {
        [QuSpec]
        [ApiSpec("TwoWayCall")]        
        QuResult<HelloWeather> TwoWayCall(string id1);
    }
    [ApiSpec(typeof(IHelloWorldService), RouteTemplate.API_VER_SVC)]
    public interface IHelloWorldService: IHelloService
    {
        [ApiSpec("hello/{id1}")]//在Queue的解法參數若傳int會有前端總會傳long的bug，
        HelloWeather Hello(string id1, long id2, DateTime id3, HelloInput inp);

        [ApiSpec("HelloGet")]
        string HelloGet(string id1, string id2);

        [QuSpec]
        [ApiSpec("HelloPost")]
        string HelloPost(string id1, string id2);

        [QuSpec]
        [ApiSpec("")]
        IEnumerable<HelloWeather> DefaultGet();
    }
}
