using OS.Core.Queries;
using Shouldly;
using System.Linq;
using Xunit;

namespace OS.Core.Queries.UnitTests
{
    public partial class QueryableExtensionsTests
    {
        public class GivenSwOperator
        {
            private readonly IQueryable<TestClass> source = (new[]
            {
                "caterpillar", "whale", "bicycle", "automobile", "mobilephone", "bidirectional", "bipolar"
            }).Select(x => new TestClass() { Property = x })
                .AsQueryable();

            [Theory]
            [InlineData(CriteriumOperator.Sw, "cat", 1)]
            [InlineData(CriteriumOperator.Sw, "wha", 1)]
            [InlineData(CriteriumOperator.Sw, "bi", 3)]
            [InlineData(CriteriumOperator.Sw, "bid", 1)]
            public void WhenApplyingFilterCriterium_CorrectResultCount(CriteriumOperator @operator, object @value, int resultCount)
            {
                // Arrange
                var criterium = new FilterCriterium() { PropertyName = nameof(TestClass.Property).ToLowerInvariant(), Operator = @operator, Value = value };
                var query = new Query();
                query.FilterCriteria.Add(criterium);

                // Act
                var result = source.Where(query);

                // Assert
                result.Count().ShouldBe(resultCount);
            }

            private class TestClass
            {
                public string Property { get; set; }
            }
        }
    }
}