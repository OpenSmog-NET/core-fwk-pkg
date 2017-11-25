using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System;

namespace OS.Core.WebJobs
{
    public static class WebJobHostConfiguration
    {
        public static JobHost Configure(IConfiguration configuration, IServiceProvider container, bool useServiceBus = false, bool useTimers = false)
        {
            var settings = GetSettings(configuration);
            var jobHostConfiguration = CreateJobHostConfiguration(container, settings);

            if (useTimers)
            {
                jobHostConfiguration.UseTimers();
            }

            return new JobHost(jobHostConfiguration);
        }

        private static WebJobSettings GetSettings(IConfiguration configuration)
        {
            var settings = new WebJobSettings(configuration);
            configuration.GetSection("OS:WebJob").Bind(settings);

            return settings;
        }

        private static JobHostConfiguration CreateJobHostConfiguration(IServiceProvider container, WebJobSettings settings)
        {
            var jobHostConfiguration = new JobHostConfiguration()
            {
                DashboardConnectionString = settings.DashboardConnectionString,
                StorageConnectionString = settings.StorageConnectionString,

                JobActivator = new WebJobActivator(container)
            };

            return jobHostConfiguration;
        }
    }
}