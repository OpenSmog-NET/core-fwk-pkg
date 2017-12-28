using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OS.Core.Middleware;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace OS.Core.Api.IntegrationTests
{
    public class GivenApiWithHealthCheckMiddleware : ApiTestFixture<GivenApiWithHealthCheckMiddleware.Startup>
    {
        [Fact]
        public async Task WhenCallingProbeEndpoint_HttpGet_OkResponse()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/probe");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenCallingProbeEndpoint_NotHttpGet_ResponseFromSuccessorPipelineElement()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.PostAsync("/probe", null);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.UseMiddleware<HealthCheckMiddleware>();
                app.Run(async (ctx) =>
                {
                    ctx.Response.StatusCode = 500;

                    await Task.FromResult(0);
                });
            }

            public void ConfigureServices(IServiceCollection services)
            {
            }
        }
    }
}