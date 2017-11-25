using Microsoft.Extensions.Configuration;

namespace OS.Core.WebJobs
{
    public class WebJobSettings
    {
        private readonly IConfiguration configuration;

        public WebJobSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string DashboardConnectionStringName { get; set; }
        public string StorageConnectionStringName { get; set; }

        public string ServiceBusConnectionStringName { get; set; }

        public string DashboardConnectionString => configuration.GetConnectionString(DashboardConnectionStringName);

        public string StorageConnectionString => configuration.GetConnectionString(StorageConnectionStringName);

        public string ServiceBusConectionString => configuration.GetConnectionString(ServiceBusConnectionStringName);
    }
}