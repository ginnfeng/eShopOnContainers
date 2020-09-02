////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/8/2020 2:49:24 PM 
// Description: Test_Exception.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Common.Support.ErrorHandling;
using Common.Support.Net.Logger;
using System;

using System.IO;

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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true);
            var cfg = builder.Build();
            var sc = new ServiceCollection(); 
            
            sc.Configure<FileLoggerOptions>(cfg.GetSection("Logging:DailyLogger"));
            //sc.Configure<FileLoggerOptions>(options => cfg.GetSection("Logging:DailyFile").Bind(options));//此IOptionsMonitor無法偵測appsettings.json更動

            //sc.AddLogging(builder => builder.AddDailyFile(opt => { opt.FileName = "MyLog_"; }).AddDebug().AddFilter(level => level >= LogLevel.Debug));
            sc.AddLogging(builder => builder.AddDailyFile().AddDebug().AddFilter(level => level >= LogLevel.Debug));
            

            //https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/logging/?tabs=aspnetcore2x&view=aspnetcore-3.1
            //Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.
            //sc.AddLogging(builder => builder.AddFilter((provider, category, logLevel) =>
            //{
            //    if (provider.Contains("DailyLogger")// LoggerProvider or alias 
            //        && category.Contains("Test_Exception") 
            //        && logLevel >= LogLevel.Warning)
            //    {
            //        return true;
            //    }
            //    return false;
            //}));
            return sc.BuildServiceProvider();
        }
        [UMethod]
        public void T_Retry()
        {// TODO: Add Testing logic here
            var xo = new OutService();
            var ts = new TimeSpan(0, 0, 3);//Retry 間隔時間
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
        static IServiceProvider sp;
        [UMethod]
        public void T_DailyLoggerProvider()
        {
            sp ??= InitSP();
            //var loggerFactory = sp.GetService<ILoggerFactory>();
            //var logger= loggerFactory.CreateLogger<Test_Exception>();
            var logger = sp.GetService<ILogger<Test_Exception>>();
            using (logger.BeginScope("START LOG"))
            {// APIDemo\UTool\bin\Debug\netcoreapp3.1\Log\MyLog_20200717.txt
                logger.LogTrace("TRACE");
                logger.LogDebug("DEBUG");
                logger.LogInformation("INFOMATION");
                logger.LogWarning("WARNING");
                logger.LogError("ERROR!");
            }
        }
        [UMethod]
        public void T_Options()
        {
            //ChangeToken
            sp ??= InitSP();
            //此範例config path為..APIDemo\UTool\bin\Debug\netcoreapp3.1\appsettings.json
            var opt1 = sp.GetRequiredService<IOptions<FileLoggerOptions>>(); //option values不會因config更動而改變
            var optM = sp.GetRequiredService<IOptionsMonitor<FileLoggerOptions>>();//option values不會因cconfig更動立即改變，為Singleton 
            var optS = sp.GetRequiredService<IOptionsSnapshot<FileLoggerOptions>>();//option values不會因cconfig更動立即改變，但在一scoped內其值不變
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
