using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Infrastructure.Messaging.Interfaces;
using System;
using System.Text;

namespace Service.Core.Services.MessagingEvent
{
    public class TestEvent : IMessagingEvent
    {
        private readonly ILogger<TestEvent> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TestEvent(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<TestEvent>>();
            }
        }

        public void Process(object message)
        {
            _logger.LogInformation("received event with {0}", message);
        }
    }
}
