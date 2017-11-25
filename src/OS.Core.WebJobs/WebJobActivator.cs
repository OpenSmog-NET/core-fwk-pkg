using Microsoft.Azure.WebJobs.Host;
using System;

namespace OS.Core.WebJobs
{
    public class WebJobActivator : IJobActivator
    {
        private readonly IServiceProvider container;

        public WebJobActivator(IServiceProvider container)
        {
            this.container = container;
        }

        public T CreateInstance<T>()
        {
            return (T)container.GetService(typeof(T));
        }
    }
}