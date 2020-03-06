using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.Messaging.Models
{
    public class MessagingEvent
    {
        public Dictionary<string, Type> subscriptions { get; set; }
    }
}
