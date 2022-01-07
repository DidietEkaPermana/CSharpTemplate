using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Models;
using Services.Infrastructure.Messaging.Interfaces;
using STAN.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.Messaging.Stan
{
    public class StanMessageReceiverService : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StanMessageReceiverService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IStanConnection _connection;

        MessagingEvent _messagingEvent;
        private Task[] _tasks;

        private CancellationTokenSource _cancellationTokenSource;

        StanSubscriptionOptions sOpts = StanSubscriptionOptions.GetDefaultOptions();
        string qGroup;

        public StanMessageReceiverService(
            IConfiguration configuration,
            ILogger<StanMessageReceiverService> logger,
            MessagingEvent messagingEvent,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messagingEvent = messagingEvent;

            var opts = StanOptions.GetDefaultOptions();
            opts.NatsURL = _configuration.GetValue<string>("Messaging:Stan:Servers");
            string clusterID = _configuration.GetValue<string>("Messaging:Stan:ClusterId");
            qGroup = _configuration.GetValue<string>("Messaging:Group");
            try
            {

                _connection = new StanConnectionFactory().CreateConnection(clusterID, qGroup, opts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error messaging connection");
                //throw ex;
            }

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Starting hosted service \"{nameof(StanMessageReceiverService)}\".");
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

                        var theInstance = (IMessagingEvent)Activator.CreateInstance(_messagingEvent.subscriptions[topic], _serviceProvider);

                        EventHandler<StanMsgHandlerArgs> msgHandler = (sender, args) =>
                        {
                            theInstance.Process(args.Message.Data);
                        };

                        //while (!cancellationToken.IsCancellationRequested)
                        //{
                            IStanSubscription stanSubscription = _connection.Subscribe(topic, qGroup, sOpts, msgHandler);
                        //}

                    }, cancellationToken);

                    tasks.Add(t);
                }

                _tasks = tasks.ToArray();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Started hosted service \"{nameof(StanMessageReceiverService)}\".");
                }
            });

            return task.IsCompleted ? Task.CompletedTask : task;
        }

        private bool stoppedValue = false;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (!stoppedValue)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Stopping hosted service \"{nameof(StanMessageReceiverService)}\".");
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

                    _connection?.Close();
                    _connection?.Dispose();


                    stoppedValue = true;

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Stopped hosted service \"{nameof(StanMessageReceiverService)}\".");
                    }
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Stopping hosted service \"{nameof(StanMessageReceiverService)}\" throws exception. {e.Message}");
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
                        _logger.LogCritical($"Hosted service \"{nameof(StanMessageReceiverService)}\" is being disposed while running.");
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
