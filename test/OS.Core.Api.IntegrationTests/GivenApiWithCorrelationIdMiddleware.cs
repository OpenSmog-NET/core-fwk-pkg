using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace OS.Core.Api.IntegrationTests
{
    public class GivenApiWithCorrelationIdMiddleware : ApiTestFixture<GivenApiWithCorrelationIdMiddleware.Startup>
    {
        [Fact]
        public async Task WhenCallingTheApi_FailureResponse_CorrelationIdShouldBeGenerated()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/error");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task WhenCallingTheApi_SuccessResponse_CorrelationIdShouldBeGenerated()
        {
            // Arrange
            var client = Server.CreateClient();

            // Act
            var result = await client.GetAsync("/");

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.UseCorrelationIdMiddleware();
                app.Map("/error", appl =>
                {
                    appl.Run(async (ctx) =>
                    {
                        AssertCorrelationId(ctx);

                        ctx.Response.StatusCode = 500;

                        await Task.FromResult(0);
                    });
                });
                app.Run(async (ctx) =>
                {
                    AssertCorrelationId(ctx);

                    ctx.Response.StatusCode = 200;

                    await Task.FromResult(0);
                });
            }

            public void ConfigureServices(IServiceCollection services)
            {
            }
        }
    }
}