////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 5:10:26 PM 
// Description: HelloWorldService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Servic;
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Service.Ordering.ApiImp
{
    public class HelloWorldService : IHelloWorldService, IHelloQuService
    {
        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public HelloWorldService()
        {           
        }
        public string Hello(string id)
        {
            return $"*** Hello {id}";           
        }
        public IEnumerable<HelloWeather> DefaultGet()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new HelloWeather
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        public HelloWeather Hello(string id1, int id2, DateTime id3, HelloInput inp)
        {
            var rng = new Random();
            return new HelloWeather()
            {
                Date = inp.Date,
                UserName = inp.UserName,
                TemperatureC = rng.Next(-20, 55),
                Summary = $"*** id1={id1} id2={id2}  id3={id3} "
            };
        }

        public string HelloGet(string id1, string id2)
        {
            return $"*** HelloGet('{id1},{id2}')";
        }

        public string HelloPost(string id1, string id2)
        {
            return $"*** HelloPost('{id1},{id2}')";
        }
        public void OneWayCall(string id1, HelloInput inp)
        {
            Debug.WriteLine($"*** ConsoleMessage (HelloMQDemo {id1}) ");
            //var ts = new JsonNetTransfer();
            //ts.Save(inp, @"d:\temp\a.json");
        }

        public QuResult<HelloWeather> TwoWayCall(string id1)
        {
            var rng = new Random();
            var rlt = new HelloWeather()
            {
                Date = DateTime.Now,
                UserName = id1,
                TemperatureC = rng.Next(-20, 55)
            };
            rlt.Summary = $"*** from {nameof(TwoWayCall)}  {rlt.UserName} {rlt.TemperatureC} {rlt.Date}";
            return new QuResult<HelloWeather> (rlt);
        }
    }
}
