﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/8/2020 2:49:24 PM 
// Description: Test_Exception.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Support.ErrorHandling;
using Support.Net.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UTDll;
namespace UTool.Test
{
    class Test_Exception : UTest
    {
        public Test_Exception()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        static internal IServiceProvider InitSP()
        {
            var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetFileProvider(new PhysicalFileProvider(basePath))
                //.AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",optional: true);
            var cfg = builder.Build();
            var sc = new ServiceCollection();
            sc.AddLogging(builder => builder.AddDailyFile(opt => { opt.FileName = "MyLog_"; }).AddConsole().AddFilter(level => level >= LogLevel.Debug));
            //sc.AddLogging(builder => builder.AddDailyFile("LoggerSetting",  o=> { } ).AddConsole().AddFilter(level => level >= LogLevel.Debug));
            
            return sc.BuildServiceProvider();
        }
        [UMethod]
        public void T_Retry()
        {// TODO: Add Testing logic here
            var xo = new OutService();
            var ts=new TimeSpan(0, 0, 3);//Retry 間隔時間
            int retryNum = 2;//Retry 之次數限制
            try
            {
                RetryHelper.AutoRetry(() => xo.CallWebServiceFunction(), ts, retryNum);
            }
            catch (Exception e)
            {
                //Here! To Log Error!
                throw e;
            }            
        }
        [UMethod]
        public void T_DailyLoggerProvider()
        {
            var sp = InitSP();
            //var loggerFactory = sp.GetService<ILoggerFactory>();
            //var logger= loggerFactory.CreateLogger<Test_Exception>();
            var logger = sp.GetService<ILogger<Test_Exception>>();
            using (logger.BeginScope("START LOG"))
            {// APIDemo\UTool\bin\Debug\netcoreapp3.1\Log\MyLog_20200717.txt
                logger.LogDebug("DEBUG");
                logger.LogError("ERROR!");
            }
            
        }
    }
    class OutService
    {
        public void CallWebServiceFunction()
        {
            //......呼叫介面，其可能throw Exception;
        }
    }
}
