////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/17/2020 10:06:47 AM 
// Description: BatchingLoggerProvider.cs  
// Revisions  :            		
// **************************************************************************** 

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Net.Logger;

namespace Support.Net.Logger
{
    /// <summary>
    /// A provider of <see cref="BatchingLogger"/> instances.
    /// </summary>
    public abstract class BatchingLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
        private BatchingLoggerOptions _options;
       
        private readonly IDisposable _optionsChangeToken;

        private int _messagesDropped;

        private BlockingCollection<LogMessage> _messageQueue;
        private Task _outputTask;
        private CancellationTokenSource _cancellationTokenSource;

        //private bool _includeScopes;
        private IExternalScopeProvider _scopeProvider;

        internal IExternalScopeProvider ScopeProvider => _options.IncludeScopes ? _scopeProvider : null;

        internal BatchingLoggerProvider(IOptionsMonitor<BatchingLoggerOptions> options)
        {
            // NOTE: Only IsEnabled is monitored
            _optionsChangeToken = options.OnChange(UpdateOptions);
            UpdateOptions(options.CurrentValue);
        }

        /// <summary>
        /// Checks if the queue is enabled.
        /// </summary>
        public bool IsEnabled => _options?.IsEnabled??false;
        
        virtual protected void UpdateOptions(BatchingLoggerOptions options)
        {
            if (options.BatchSize <= 0)
            {
                if (_options==null) throw new ArgumentOutOfRangeException(nameof(options.BatchSize), $"{nameof(options.BatchSize)} must be a positive number.");
                return;
            }
            if (options.FlushPeriod <= TimeSpan.Zero)
            {
                if (_options == null) throw new ArgumentOutOfRangeException(nameof(options.FlushPeriod), $"{nameof(options.FlushPeriod)} must be longer than zero.");
                return;
            }
            var oldIsEnabled = IsEnabled;
            _options = options;
            if (oldIsEnabled == IsEnabled) return;
            if (IsEnabled) Start();
            else Stop();
        }

        internal abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

        private async Task ProcessLogQueue()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _options.BatchSize ?? int.MaxValue;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                var messagesDropped = Interlocked.Exchange(ref _messagesDropped, 0);
                if (messagesDropped != 0)
                {
                    _currentBatch.Add(new LogMessage(DateTimeOffset.Now, $"{messagesDropped} message(s) dropped because of queue size limit. Increase the queue size or decrease logging verbosity to avoid this.{Environment.NewLine}"));
                }

                if (_currentBatch.Count > 0)
                {
                    try
                    {
                        await WriteMessagesAsync(_currentBatch, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        // ignored
                    }

                    _currentBatch.Clear();
                }
                else
                {
                    await IntervalAsync(_options.FlushPeriod, _cancellationTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// Wait for the given <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="interval">The amount of time to wait.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the delay.</param>
        /// <returns>A <see cref="Task"/> which completes when the <paramref name="interval"/> has passed or the <paramref name="cancellationToken"/> has been canceled.</returns>
        protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        internal void AddMessage(DateTimeOffset timestamp, string message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    if (!_messageQueue.TryAdd(new LogMessage(timestamp, message), millisecondsTimeout: 0, cancellationToken: _cancellationTokenSource.Token))
                    {
                        Interlocked.Increment(ref _messagesDropped);
                    }
                }
                catch
                {
                    //cancellation token canceled or CompleteAdding called
                }
            }
        }

        private void Start()
        {
            _messageQueue = _options.BackgroundQueueSize == null ?
                new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>()) :
                new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), _options.BackgroundQueueSize.Value);

            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Run(ProcessLogQueue);
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(_options.FlushPeriod);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _optionsChangeToken?.Dispose();
            if (IsEnabled)
            {
                Stop();
            }
        }

        /// <summary>
        /// Creates a <see cref="BatchingLogger"/> with the given <paramref name="categoryName"/>.
        /// </summary>
        /// <param name="categoryName">The name of the category to create this logger with.</param>
        /// <returns>The <see cref="BatchingLogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new BatchingLogger(this, categoryName);
        }

        /// <summary>
        /// Sets the scope on this provider.
        /// </summary>
        /// <param name="scopeProvider">Provides the scope.</param>
        void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }
    }
}