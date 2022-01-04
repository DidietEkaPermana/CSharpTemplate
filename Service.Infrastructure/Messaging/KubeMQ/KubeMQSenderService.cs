using System;
using System.Threading.Tasks;
using KubeMQ.SDK.csharp.Events;
using KubeMQ.SDK.csharp.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Interfaces;

namespace Service.Infrastructure.Messaging.KubeMQ
{
    public class KubeMQSenderService: IMessageSenderService, IDisposable
    {
        private readonly ILogger<KubeMQSenderService> _logger;
        private readonly IConfiguration _configuration;

        public KubeMQSenderService(
            IConfiguration configuration,
            ILogger<KubeMQSenderService> logger
            )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Send(string topic, object message)
        {
            var KubeMQServerAddress = _configuration.GetValue<string>("Messaging:KubeMQ:Servers");
            var ClientID = _configuration.GetValue<string>("Messaging:Group");

            var channel = new Channel(new ChannelParameters
            {
                ChannelName = topic,
                ClientID = ClientID,
                KubeMQAddress = KubeMQServerAddress
            });

            try
            {
                var result = channel.SendEvent(new Event()
                {
                    Body = (byte[])message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send event error");
                throw new Exception($"Failed to send message.", ex);
            }
        }

        public Task SendAsync(string topic, object message)
        {
            throw new NotImplementedException();
        }
    }
}
