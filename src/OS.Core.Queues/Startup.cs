using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OS.Core.Queues
{
    public static class Startup
    {
        public static IServiceCollection AddQueues(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<QueueSettings>();
            services.AddSingleton<IQueueClient, QueueClient>();

            return services;
        }
    }
}