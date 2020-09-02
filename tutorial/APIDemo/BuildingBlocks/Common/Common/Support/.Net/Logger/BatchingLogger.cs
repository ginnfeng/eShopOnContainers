﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/17/2020 10:19:00 AM 
// Description: BatchingLogger.cs  
// Revisions  :            		
// **************************************************************************** 

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Common.Support.Net.Logger
{
    internal class BatchingLogger : ILogger
    {
        private readonly BatchingLoggerProvider _provider;
        private readonly string _category;

        public BatchingLogger(BatchingLoggerProvider loggerProvider, string categoryName)
        {
            _provider = loggerProvider;
            _category = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled;
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var builder = new StringBuilder();
            //builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
            builder.Append(timestamp.ToString("HH:mm:ss.fff"));
            builder.Append(" [");
            builder.Append(logLevel.ToString());
            builder.Append("] ");
            builder.Append(_category);

            var scopeProvider = _provider.ScopeProvider;
            if (scopeProvider != null)
            {
                scopeProvider.ForEachScope((scope, stringBuilder) =>
                {
                    stringBuilder.Append(" => ").Append(scope);
                }, builder);

                builder.AppendLine(":");
            }
            else
            {
                builder.Append(": ");
            }

            builder.AppendLine(formatter(state, exception));

            if (exception != null)
            {
                builder.AppendLine(exception.ToString());
            }

            _provider.AddMessage(timestamp, builder.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.Now, logLevel, eventId, state, exception, formatter);
        }
    }
}

