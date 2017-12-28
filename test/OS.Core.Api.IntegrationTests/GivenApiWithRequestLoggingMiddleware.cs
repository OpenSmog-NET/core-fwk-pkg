using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OS.Core.Api.IntegrationTests
{
    public class GivenApiWithRequestLoggingMiddleware : ApiTestFixture<GivenApiWithRequestLoggingMiddleware.Startup>
    {
        public static IDictionary<HttpStatusCode, LogEventLevel> ExpectedLogResponses = new Dictionary<HttpStatusCode, LogEventLevel>()
        {
            { HttpStatusCode.OK, LogEventLevel.Information },
            { HttpStatusCode.NotFound, LogEventLevel.Information },
            { HttpStatusCode.InternalServerError, LogEventLevel.Error }
        };

        [Fact]
        public async Task WhenHandlingOkResponse_ThenInformationShouldBeLogged()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenHandlingNotFoundResponse_ThenInformationShouldBeLogged()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/notFound");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenHandlingInternalServerErrorResponse_ThenErrorShouldBeLogged()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/error");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            { }

            public void Configure(IApplicationBuilder app)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.Observers(events => events
                    .Do(evt =>
                    {
                        var status = (HttpStatusCode)(Enum.Parse(typeof(HttpStatusCode), evt.Properties["StatusCode"].ToString()));
                        evt.Level.ShouldBe(ExpectedLogResponses[status]);
                    })
                    .Subscribe())
                .CreateLogger();
                app.UseRequestLoggingMiddleware(logger);

                app.Map("/notFound", appl =>
                {
                    appl.Run(async (ctx) =>
                    {
                        ctx.Response.StatusCode = 404;

                        await Task.FromResult(0);
                    });
                });

                app.Map("/error", appl =>
                {
                    appl.Run(async (ctx) =>
                    {
                        ctx.Response.StatusCode = 500;

                        await Task.FromResult(0);
                    });
                });

                app.Run(async (ctx) =>
                {
                    ctx.Response.StatusCode = 200;

                    await Task.FromResult(0);
                });
            }
        }
    }
}