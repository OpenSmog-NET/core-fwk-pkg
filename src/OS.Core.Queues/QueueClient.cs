using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using OS.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OS.Core.Queues
{
    public class QueueClient : IQueueClient, IDisposable
    {
        private bool isDisposed = false;
        private readonly CloudQueueClient client;
        private readonly ILogger<QueueClient> logger;

        private readonly ConcurrentDictionary<string, CloudQueue> queues = new ConcurrentDictionary<string, CloudQueue>();

        private readonly Func<QueueSettings, CloudQueueClient> clientFactory = (settings) =>
        {
            var account = CloudStorageAccount.Parse(settings.ConnectionString);
            return account.CreateCloudQueueClient();
        };

        public QueueClient(QueueSettings settings, ILogger<QueueClient> logger)
        {
            client = clientFactory(settings);
            this.logger = logger;
        }

        public async Task SendAsync<T>(T message, string queueName)
            where T : class, IDomainEvent
        {
            if (isDisposed) throw new ObjectDisposedException(this.GetType().Name);

            var queue = await GetQueueAsync(queueName);
            var msg = GetMessage(message);
            await queue.AddMessageAsync(msg);

            logger.LogInformation("{@message} {@bytes}[bytes] ==(queue)==> {@queue}", message, msg.AsBytes.Length, queueName);
        }

        private async Task<CloudQueue> GetQueueAsync(string key)
        {
            if (queues.ContainsKey(key))
            {
                return queues[key];
            }

            queues[key] = client.GetQueueReference(key);
            await queues[key].CreateIfNotExistsAsync();

            return queues[key];
        }

        private CloudQueueMessage GetMessage<T>(T message) where T : class
        {
            return new CloudQueueMessage(Serializer.Serialize(message));
        }

        public void Dispose()
        {
            if (isDisposed) return;

            logger.LogDebug("Disposing");

            queues.Clear();

            isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}