////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 5:10:26 PM 
// Description: HelloWorldService.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Servic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Ordering.ApiImp
{
    public class HelloWorldService : IHelloWorldService
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
            return $"Hello {id}";           
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
    }
}
