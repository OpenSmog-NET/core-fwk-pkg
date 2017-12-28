using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using OS.Core.Middleware;
using Shouldly;
using System;

namespace OS.Core.Api.IntegrationTests
{
    public abstract class ApiTestFixture<TStartup>
        where TStartup : class
    {
        protected readonly TestServer Server = new TestServer(new WebHostBuilder()
                .UseStartup<TStartup>());

        protected static void AssertCorrelationId(HttpContext ctx)
        {
            ctx.Request.Headers.Keys.ShouldContain(Constants.RequestCorrelation.RequestHeaderName);
            var id = ctx.Request.Headers[Constants.RequestCorrelation.RequestHeaderName];

            Guid correlationIdGuid;
            Guid.TryParse(id, out correlationIdGuid).ShouldBe(true);
        }
    }
}