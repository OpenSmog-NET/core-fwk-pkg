using Shouldly;
using System.Linq;
using Xunit;

namespace OS.Core.Queries.UnitTests
{
    public partial class QueryableExtensionsTests
    {
        public class GivenCompareOperator
        {
            private readonly IQueryable<TestClass> source = (new[]
            {
                new TestClass {Property1 = true,  Property2 = 12},
                new TestClass {Property1 = false, Property2 = 42},
                new TestClass {Property1 = true,  Property2 = 22},
                new TestClass {Property1 = false, Property2 = 11}
            }).AsQueryable();

            [Theory]
            [InlineData("Property1", CriteriumOperator.Eq, true, 2)]
            [InlineData("Property2", CriteriumOperator.Eq, 11, 1)]
            [InlineData("Property2", CriteriumOperator.Gt, 11, 3)]
            [InlineData("Property2", CriteriumOperator.Gt, 20, 2)]
            [InlineData("Property2", CriteriumOperator.Ge, 11, 4)]
            [InlineData("Property2", CriteriumOperator.Lt, 22, 2)]
            [InlineData("Property2", CriteriumOperator.Le, 22, 3)]
            public void WhenApplyingFilterCriterium_CorrectResultCount(string property, CriteriumOperator @operator, object @value, int resultCount)
            {
                // Arrange
                var criterium = new FilterCriterium() { PropertyName = property, Operator = @operator, Value = value };
                var query = new Query();
                query.FilterCriteria.Add(criterium);

                // Act
                var result = source.Where(query);

                // Assert
                result.Count().ShouldBe(resultCount);
            }

            private class TestClass
            {
                public bool Property1 { get; set; }
                public int Property2 { get; set; }
            }
        }
    }
}