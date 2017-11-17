using Microsoft.Extensions.Configuration;

namespace OS.Core.Queues
{
    public class QueueSettings
    {
        private readonly IConfiguration configuration;

        public QueueSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ConnectionStringName => configuration["OS:Queues:ConnectionStringName"];
        public string ConnectionString => configuration.GetConnectionString(ConnectionStringName);
    }
}