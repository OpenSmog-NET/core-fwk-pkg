using OS.Core.Queries;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OS.Core.Queries.UnitTests
{
    public partial class QueryableExtensionsTests
    {
        public class GivenInOperator
        {
            private static readonly IReadOnlyDictionary<string, int[]> FilterValues = new Dictionary<string, int[]>
            {
                { "1", new [] { 1, 15, 22 } },
                { "2", new [] { 17 } },
                { "3", new [] { 22, 28, 33, 34 } },
                { "4", new int[] { } },
                { "5", new [] { 0 } }
            };

            //private static readonly int[] filterValue1 = new[] { 1, 5, 15 };

            private readonly IQueryable<TestClass> source = (new[]
                {
                    1, 1, 15, 45, 17, 22, 28, 34
                }).Select(x => new TestClass() { Property = x })
                .AsQueryable();

            [Theory]
            [InlineData(CriteriumOperator.In, "1", 4)]
            [InlineData(CriteriumOperator.In, "2", 1)]
            [InlineData(CriteriumOperator.In, "3", 3)]
            [InlineData(CriteriumOperator.In, "4", 0)]
            [InlineData(CriteriumOperator.In, "5", 0)]
            public void WhenApplyingFilterCriteriumWithIntValues_CorrectResultCount(CriteriumOperator @operator, string key, int resultCount)
            {
                // Arrange
                var criterium = new FilterCriterium() { PropertyName = nameof(TestClass.Property).ToLowerInvariant(), Operator = @operator, Value = FilterValues[key] };
                var query = new Query();
                query.FilterCriteria.Add(criterium);

                // Act
                var result = source.Where(query);

                // Assert
                result.Count().ShouldBe(resultCount);
            }

            private class TestClass
            {
                public int Property { get; set; }
            }
        }
    }
}