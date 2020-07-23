////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/16/2020 3:51:24 PM 
// Description: BatchingLoggerProvider.cs  
// Revisions  :            		
// **************************************************************************** 
#region ****sample****
    //static internal IServiceProvider InitSP()
    //{
    //    var basePath = Directory.GetCurrentDirectory();
    //    var builder = new ConfigurationBuilder()
    //        .SetFileProvider(new PhysicalFileProvider(basePath))
    //        //.AddEnvironmentVariables()
    //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true);
    //    var cfg = builder.Build();
    //    var sc = new ServiceCollection();
    //    sc.Configure<FileLoggerOptions>(cfg.GetSection("Logging:DailyFile"));
    //    //sc.Configure<FileLoggerOptions>(options => cfg.GetSection("Logging:DailyFile").Bind(options));//此IOptionsMonitor無法偵測appsettings.json更動
    //    //sc.AddLogging(builder => builder.AddDailyFile(opt => { opt.FileName = "MyLog_"; }).AddConsole().AddFilter(level => level >= LogLevel.Debug));
    //    sc.AddLogging(builder => builder.AddDailyFile().AddConsole().AddFilter(level => level >= LogLevel.Debug));
    //    return sc.BuildServiceProvider();
    //}
    //static IServiceProvider sp;
    //[UMethod]
    //public void T_DailyLoggerProvider()
    //{
    //    sp ??= InitSP();
    //    //var loggerFactory = sp.GetService<ILoggerFactory>();
    //    //var logger= loggerFactory.CreateLogger<Test_Exception>();
    //    var logger = sp.GetService<ILogger<Test_Exception>>();
    //    using (logger.BeginScope("START LOG"))
    //    {// APIDemo\UTool\bin\Debug\netcoreapp3.1\Log\MyLog_20200717.txt
    //        logger.LogDebug("DEBUG");
    //        logger.LogError("ERROR!");
    //    }
    //}
    //[UMethod]
    //public void T_Options()
    //{
    //    //ChangeToken
    //    sp ??= InitSP();
    //////此範例config path為..APIDemo\UTool\bin\Debug\netcoreapp3.1\appsettings.json
    //    var opt1 = sp.GetRequiredService<IOptions<FileLoggerOptions>>(); //option values不會因config更動而改變
    //    var optM = sp.GetRequiredService<IOptionsMonitor<FileLoggerOptions>>();//option values不會因config更動立即改變，為Singleton 
    //    var optS = sp.GetRequiredService<IOptionsSnapshot<FileLoggerOptions>>();//option values不會因config更動立即改變，但在一scoped內其值不變 
    //}
#endregion
// https://andrewlock.net/creating-a-rolling-file-logging-provider-for-asp-net-core-2-0/
// namespace Microsoft.Extensions.Logging.AzureAppServices

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Support.Net.Logger

{
   
    [ProviderAlias("DailyFile")]
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?tabs=aspnetcore2x&view=aspnetcore-3.1#log-filtering
    //https://www.dotnetcurry.com/aspnet/1402/aspnet-core-2-new-features
    public class DailyLoggerProvider : BatchingLoggerProvider
    {
        private string _path;
        private string _fileName;
        /// <summary>
        /// Creates a new instance of <see cref="FileLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to use when creating a provider.</param>
        public DailyLoggerProvider(IOptionsMonitor<FileLoggerOptions> options) 
            : base(options)
        {                     
        }
        override protected void UpdateOptions(BatchingLoggerOptions options)
        {
            base.UpdateOptions(options);
            var newOptions= options as FileLoggerOptions;
            if (newOptions != null)
            {
                _path = newOptions.LogDirectory ?? "./Log";
                _fileName = newOptions.FileName ?? "";
            }
        }
        internal override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
        {
            if(!string.IsNullOrEmpty(_path))Directory.CreateDirectory(_path);
            foreach (var group in messages.GroupBy(msg=>msg.Timestamp))
            {                         
                
                using (var stream = CreateLogFile(group.Key))
                using (var streamWriter = new StreamWriter(stream))
                {
                    foreach (var item in group)
                    {
                        await streamWriter.WriteAsync(item.Message);
                    }
                }
            }            
        }
        private FileStream CreateLogFile(DateTimeOffset tm)
        {
            //delete last month log
            File.Delete(GenLogPath(tm.AddMonths(-1)));
            string path = GenLogPath(tm);
            bool isExist = File.Exists(path);
            return !isExist ? File.Open(path, FileMode.CreateNew) : File.Open(path, FileMode.Append);
        }
        private string GenLogPath(DateTimeOffset tm)
        {   
            return Path.Combine(_path, $"{_fileName}{tm.Year:0000}{tm.Month:00}{tm.Day:00}.txt");
        }
        
    }
}
