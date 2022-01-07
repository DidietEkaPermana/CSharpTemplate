using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.Models
{
    public class MessagingEvent
    {
        public Dictionary<string, Type> subscriptions { get; set; }
    }
}
