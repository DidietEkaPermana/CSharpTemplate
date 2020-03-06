using System.Threading.Tasks;

namespace Service.Infrastructure.Interfaces
{
    public interface IMessageSenderService
    {
        void Send(string topic, object message);
        Task SendAsync(string topic, object message);
    }
}
