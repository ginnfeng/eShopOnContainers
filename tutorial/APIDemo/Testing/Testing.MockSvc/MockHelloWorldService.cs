////########################*Copyright © 2020 Feng 豐########################**	
// Created    : 6/11/2020 9:39:48 AM 
// Description: MockHelloWorldService.cs  
// Revisions  :            		
// ###########################################################################* 
using Common.Contract;
using Service.HelloWorld.Contract.Entity;
using Service.HelloWorld.Contract.Servic;
using Support.Net.Util;
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Reflection;


namespace Testing.MockSvc
{
   
    public class MockHelloWorldService : IHelloWorldService, IHelloQuService
    {
        static T LoadJsonSample<T>(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = ResourceHelper.LoadFromManifestResource(asm, $"DataSamples.{name}");
            var ts = new JsonNetTransfer();
            return ts.Deserialize<T>(stream);            
        }
        
        public MockHelloWorldService()
        {
        }
        public string Hello(string id)
        {
            return $"### Hello {id}";
        }
        public IEnumerable<HelloWeather> DefaultGet()
        {
            var sample = LoadJsonSample<List<HelloWeather>>("HelloWeather_ListData_001.json");
            return sample;
        }

        public HelloWeather Hello(string id1, long id2, DateTime id3, HelloInput inp)
        {
            var sample = LoadJsonSample<List<HelloWeather>>("HelloWeather_ListData_001.json");
            return sample[0];
        }

        public string HelloGet(string id1, string id2)
        {
            return $"### HelloGet('{id1},{id2}')";
        }

        public string HelloPost(string id1, string id2)
        {
            return $"### HelloPost('{id1},{id2}')";
        }
        public void OneWayCall(string id1, HelloInput inp)
        {
            Debug.WriteLine($"### ConsoleMessage (HelloMQDemo {id1}) ");
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
            rlt.Summary = $"### from {nameof(TwoWayCall)}  {rlt.UserName} {rlt.TemperatureC} {rlt.Date}";
            return new QuResult<HelloWeather>(rlt);
        }
    }
}
