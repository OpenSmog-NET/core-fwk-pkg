using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OS.Core.Middleware;
using Shouldly;

namespace OS.Core.UnitTests
{
    public class CorrelationIdMiddlewareFixture : IDisposable
    {
        private readonly TestServer server;

        public HttpClient CreateClient()
        {
            return server?.CreateClient();
        }

        public CorrelationIdMiddlewareFixture()
        {
            server = new TestServer(BuildServer());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private IWebHostBuilder BuildServer()
        {
            return new WebHostBuilder().UseStartup<Startup>();
        }

        private class Startup
        {
            public Startup(IHostingEnvironment env)
            {
                var builder = new ConfigurationBuilder();
                //.AddInMemoryCollection()
                //.AddEnvironmentVariables();
                Configuration = builder.Build();
            }

            public IConfigurationRoot Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                // Add framework services.
                services.AddMvc();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.UseMiddleware<CorrelationIdMiddleware>();
                app.Run(async (ctx) =>
                {
                    ctx.Request.Headers.ShouldContainKey(Constants.RequestCorrelation.RequestHeaderName);
                    ctx.Response.StatusCode = 200;

                    await Task.FromResult(0);
                });
            }
        }
    }
}
