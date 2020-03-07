using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace Service.Infrastructure.Messaging.Kafka
{
    public class KafkaMessageSenderService : IMessageSenderService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ProducerConfig _producerConfig;
        private readonly int _timeoutMs = 30000;
        private readonly ILogger<KafkaMessageSenderService> _logger;
        private readonly IProducer<Null, object> _producer;

        public KafkaMessageSenderService(IConfiguration configuration, ILogger<KafkaMessageSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            var bootstrapServers = _configuration.GetValue<string>("Messaging:Kafka:Servers");

            _producerConfig = new ProducerConfig
            {
                Acks = Acks.Leader,
                BootstrapServers = bootstrapServers,
                MessageTimeoutMs = _timeoutMs
            };

            _producer = new ProducerBuilder<Null, object>(_producerConfig).Build();
        }

        public void Send(string topic, object message)
        {
            SendAsync(topic, message).GetAwaiter().GetResult();
        }

        public async Task SendAsync(string topic, object message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Sending. Topic: {topic} Message: {message}");
            }

            var msg = new Message<Null, object>
            {
                Value = message
            };

            DeliveryResult<Null, object> deliveryResult;

            try
            {
                deliveryResult = await _producer.ProduceAsync(topic, msg);
            }
            catch (ProduceException<Null, object> e)
            {
                if (e.Error.Code == ErrorCode.Local_MsgTimedOut)
                {
                    throw new TimeoutException($"Error code \"{e.Error.Code}\". Failed to send within {_timeoutMs} milliseconds.", e);
                }
                else
                {
                    throw new Exception($"Error code \"{e.Error.Code}\". Failed to send message.", e);
                }
            }

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"Sent. Topic: {deliveryResult.Topic} Partition: {deliveryResult.Partition.Value} Offset: {deliveryResult.Offset.Value} Message: {deliveryResult.Message.Value}");
            }
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
                    _producer?.Dispose();
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
