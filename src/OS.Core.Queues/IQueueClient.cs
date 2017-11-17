using OS.Events;
using System.Threading.Tasks;

namespace OS.Core.Queues
{
    public interface IQueueClient
    {
        Task SendAsync<T>(T message, string queueName) where T : class, IDomainEvent;
    }
}