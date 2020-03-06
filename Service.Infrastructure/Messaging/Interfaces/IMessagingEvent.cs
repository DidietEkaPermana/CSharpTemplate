namespace Services.Infrastructure.Messaging.Interfaces
{
    public interface IMessagingEvent
    {
        void Process(object message);
    }
}
