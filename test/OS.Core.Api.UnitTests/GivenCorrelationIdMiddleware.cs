using System.Net;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace OS.Core.UnitTests
{
    public class GivenCorrelationIdMiddleware : IClassFixture<CorrelationIdMiddlewareFixture>
    {
        private readonly CorrelationIdMiddlewareFixture fixture;
        public GivenCorrelationIdMiddleware(CorrelationIdMiddlewareFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public async Task WhenMakingAnyRequest_ShouldContainCorrelationIdRequestHeader()
        {
            // Arrange
            var client = this.fixture.CreateClient();

            // Act
            var response = await client
                .GetAsync("/anyUrl")
                .ConfigureAwait(false);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
