////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/16/2020 3:51:24 PM 
// Description: BatchingLoggerProvider.cs  
// Revisions  :            		
// **************************************************************************** 
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
    /// <summary>
    /// A <see cref="BatchingLoggerProvider"/> which writes out to a file.
    /// </summary>
    [ProviderAlias("DailyFile")]
    public class DailyLoggerProvider : BatchingLoggerProvider
    {
        private readonly string _path;
        private readonly string _fileName;
        private readonly int? _maxFileSize;
        private readonly int? _maxRetainedFiles;

        /// <summary>
        /// Creates a new instance of <see cref="FileLoggerProvider"/>.
        /// </summary>
        /// <param name="options">The options to use when creating a provider.</param>
        public DailyLoggerProvider(IOptionsMonitor<FileLoggerOptions> options) 
            : base(options)
        {
            var loggerOptions = options.CurrentValue;
            _path = loggerOptions.LogDirectory ?? "./Log";
            _fileName = loggerOptions.FileName ?? "";
            _maxFileSize = loggerOptions.FileSizeLimit;
            _maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
        }

        internal override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
        {
            if(!string.IsNullOrEmpty(_path))Directory.CreateDirectory(_path);

            foreach (var group in messages.GroupBy(msg=>msg.Timestamp))
            {
                //var fullName = GetFullName(group.Key);
                //var fileInfo = new FileInfo(fullName);
                //if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
                //{
                //    return;
                //}                
                
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
        //private string GetFullName((int Year, int Month, int Day) group)
        //{
        //    return Path.Combine(_path, $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
        //}

        //private (int Year, int Month, int Day) GetGrouping(LogMessage message)
        //{
        //    return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
        //}

        
    }
}
