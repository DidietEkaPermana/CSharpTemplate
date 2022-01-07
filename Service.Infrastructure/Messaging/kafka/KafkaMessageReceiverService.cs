using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Models;
using Services.Infrastructure.Messaging.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.Messaging.Kafka
{
    public class KafkaMessageReceiverService : IHostedService, IDisposable
    {
        public IConfiguration _configuration { get; }
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaMessageReceiverService> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private Task[] _tasks;
        public ConsumerConfig _consumerConfig { get; }


        MessagingEvent _messagingEvent;

        public KafkaMessageReceiverService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<KafkaMessageReceiverService> logger,
            MessagingEvent messagingEvent
        )
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;

            var bootstrapServers = _configuration.GetValue<string>("Messaging:Kafka:Servers");
            var groupId = _configuration.GetValue<string>("Messaging:Group"); ;

            _consumerConfig = new ConsumerConfig
            {
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = bootstrapServers,
                EnableAutoCommit = false,
                GroupId = groupId
            };

            _messagingEvent = messagingEvent;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Starting hosted service \"{nameof(KafkaMessageReceiverService)}\".");
            }

            var task = Task.Run(() =>
            {
                List<Task> tasks = new List<Task>();
                Task t;

                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                foreach (var subscription in _messagingEvent.subscriptions)
                {
                    t = Task.Run(() =>
                    {
                        var topic = subscription.Key;
                        do
                        {
                            try
                            {
                                ConsumeAsync(topic, cancellationToken);
                            }
                            catch (Exception e)
                            {
                                _logger.LogCritical(e, $"Error when consuming topic \"{topic}\".");
                            }
                        } while (!cancellationToken.IsCancellationRequested);
                    });

                    tasks.Add(t);
                }

                _tasks = tasks.ToArray();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Started hosted service \"{nameof(KafkaMessageReceiverService)}\".");
                }
            });

            return task.IsCompleted ? Task.CompletedTask : task;
        }

        private void ConsumeAsync(string topic, CancellationToken cancellationToken)
        {
            ConsumeAsync(topic, true, cancellationToken);
        }

        private void ConsumeAsync(string topic, bool commitOnError, CancellationToken cancellationToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<Ignore, string> result = null;

                        try
                        {
                            result = consumer.Consume(cancellationToken);
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError(e, e.Message);
                        }

                        if (result == null)
                        {
                            continue;
                        }

                        try
                        {
                            var theInstance = (IMessagingEvent)Activator.CreateInstance(_messagingEvent.subscriptions[topic], _serviceProvider);

                            theInstance.Process(result.Value);
                        }
                        catch (OperationCanceledException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            if (!commitOnError)
                            {
                                throw e;
                            }
                        }
                        consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogWarning(e, $"Stopped consuming topic \"{topic}\".");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        private bool stoppedValue = false;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (!stoppedValue)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Stopping hosted service \"{nameof(KafkaMessageReceiverService)}\".");
                }

                try
                {
                    _cancellationTokenSource?.Cancel();
                    if (_tasks != null)
                    {
                        try
                        {
                            Task.WaitAll(_tasks, 5000);
                        }
                        catch (AggregateException)
                        {
                        }
                    }

                    stoppedValue = true;

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Stopped hosted service \"{nameof(KafkaMessageReceiverService)}\".");
                    }
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Stopping hosted service \"{nameof(KafkaMessageReceiverService)}\" throws exception. {e.Message}");
                    }
                    throw e;
                }
            }

            return Task.CompletedTask;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (!stoppedValue)
                    {
                        _logger.LogCritical($"Hosted service \"{nameof(KafkaMessageReceiverService)}\" is being disposed while running.");
                    }
                    _cancellationTokenSource?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KafkaMessageReceiverService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
