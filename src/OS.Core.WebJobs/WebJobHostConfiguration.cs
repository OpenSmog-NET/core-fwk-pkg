using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System;

namespace OS.Core.WebJobs
{
    internal static class WebJobHostConfiguration
    {
        public static JobHost Configure(IConfiguration configuration, IServiceProvider container)
        {
            var settings = GetSettings(configuration);
            var jobHostConfiguration = CreateJobHostConfiguration(container, settings);

            if (settings.UseTimers)
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
            if (string.IsNullOrEmpty(settings.StorageConnectionString)) throw new ArgumentException($"Connection string : {settings.StorageConnectionStringName} is missing or emtpy");
            if (string.IsNullOrEmpty(settings.DashboardConnectionString)) throw new ArgumentException($"Connection string : {settings.DashboardConnectionStringName} is missing or emtpy");

            Environment.SetEnvironmentVariable("AzureWebJobsStorage", settings.StorageConnectionString);
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", settings.DashboardConnectionString);

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