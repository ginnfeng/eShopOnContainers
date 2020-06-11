////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/11/2020 9:46:24 AM 
// Description: Test_UnitTest.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.DependencyInjection;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Servic;
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Testing.MockSvc;
using UTDll;
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
    }
}
