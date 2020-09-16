////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/11/2020 9:46:24 AM 
// Description: Test_UnitTest.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.DependencyInjection;
using Service.HelloWorld.ApiImp;
using Service.HelloWorld.Contract.Servic;
using System;
using System.Linq;
using Testing.MockSvc;
using UTDll;
using Common.Open.Serializer;
using System.Collections.Generic;
using Sid.Bss.Spec;
namespace UTool.Test
{
    enum TestEnv
    {
        Real,
        Mock
    }
    class Test_UnitTest : UTest
    {
        public Test_UnitTest()
        {
            //
            // TODO: Add constructor logic here
            //      
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, TestEnv.Mock);
            //ConfigureServices(serviceCollection, TestEnv.Real);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
        private IServiceProvider ServiceProvider { get; }
        private void ConfigureServices(ServiceCollection sc, TestEnv env)
        {
            sc.AddTransient<IMyMvcModel, MyMvcModel>();
            switch (env)
            {
                case TestEnv.Real:
                    sc.AddTransient<IHelloWorldService, HelloWorldService>();
                    break;
                case TestEnv.Mock:
                    sc.AddTransient<IHelloWorldService, MockHelloWorldService>();
                    break;
                
            }
        }
        [UMethod]
        public void T_MyMvcModel_GetCustomerInfo()
        {// TODO: Add Testing logic here
            var model = ServiceProvider.GetService<IMyMvcModel>();
            print(model.GetCustomerInfo());
            //....
        }
        [UMethod]
        public void T_MyMvcModel_QueryWeather()
        {// TODO: Add Testing logic here
            var model = ServiceProvider.GetService<IMyMvcModel>();
            var weathers=model.QueryWeather();
            
            var rlt = weathers.First();
            print($"{rlt.UserName} {rlt.TemperatureC} {rlt.Date}");
            assert(weathers.Count() >100);
            //....
        }
         
        [UMethod]
        public void T_SaveTestData()
        {// TODO: Add Testing logic here
            var ts = new JsonNetTransfer();
            var data = new SvcInstance();
            data.SpecId = "SPIP";
            data.Characteristics.Add(new SvcCharacteristic("id_1", "case_001"));
            data.Characteristics.Add(new SvcCharacteristic("id_2", "受理"));
            data.Characteristics.Add(new SvcCharacteristic("id_3", "訂單"));
            ts.Save(data,@"d:\a.json");
        }

    }
}
