using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.Messaging.Extension;
using STAN.Client;
using System;
using System.Threading.Tasks;

namespace Service.Infrastructure.Messaging.Stan
{
    public class StanMessageSenderService : IMessageSenderService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StanMessageSenderService> _logger;
        private IStanConnection _connection;

        StanOptions cOpts = StanOptions.GetDefaultOptions();

        public StanMessageSenderService(
            IConfiguration configuration, 
            ILogger<StanMessageSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            cOpts.NatsURL = _configuration.GetValue<string>("Messaging:Stan:Servers");
            string clusterID = _configuration.GetValue<string>("Messaging:Stan:ClusterId");
            string clientID = _configuration.GetValue<string>("Messaging:Group");
            _connection = new StanConnectionFactory().CreateConnection(clusterID, clientID, cOpts);
        }

        public void Send(string topic, object message)
        {
            SendAsync(topic, message).GetAwaiter().GetResult();
        }

        public async Task SendAsync(string topic, object message)
        {
            await _connection.PublishAsync(topic, message.SerializeToByteArray());
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
                    _connection?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KafkaMessageSenderService() {
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
