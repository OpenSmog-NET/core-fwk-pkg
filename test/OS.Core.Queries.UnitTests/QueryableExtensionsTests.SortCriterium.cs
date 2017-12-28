using OS.Core.Queries;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OS.Core.Queries.UnitTests
{
    public partial class QueryableExtensionsTests
    {
        public class GivenSortCriterium
        {
            private readonly IQueryable<TestClass> source = (new[]
            {
                new TestClass {Property1 = 4, Property2 = 1},
                new TestClass {Property1 = 3, Property2 = 2},
                new TestClass {Property1 = 1, Property2 = 2},
                new TestClass {Property1 = 1, Property2 = 1}
            }).AsQueryable();

            [Fact]
            public void WhenCriteriumContainsNonExistingProperty_ThenArgumentException()
            {
                // Arrange
                const string propertyName = "NonExistingProperty";
                var criteriums = new[]
                {
                    new SortCriterium() {PropertyName = propertyName, Ascending = true}
                };

                // Act & Assert
                var ex = Assert.Throws<ArgumentException>(() => source.OrderBy(criteriums).ToArray());
                ex.Message.ShouldContain(propertyName);
            }

            [Fact]
            public void WhenAscendingOrderBySortingCriteriumIsPassed_ThenResultShouldBeSorted()
            {
                // Arrange
                var criteriums = new[]
                {
                    new SortCriterium() {PropertyName = "Property1", Ascending = true}
                };

                // Act
                var result = source.OrderBy(criteriums).ToArray();

                // Assert
                result.IsAscending(x => x.Property1).ShouldBe(true);
            }

            [Fact]
            public void WhenDescendingOrderBySortingCriteriumIsPassed_ThenResultShouldBeSorted()
            {
                // Arrange
                var criteriums = new[]
                {
                    new SortCriterium() {PropertyName = "Property1", Ascending = false}
                };

                // Act
                var result = source.OrderBy(criteriums).ToArray();

                // Assert
                result.IsAscending(x => x.Property1).ShouldBe(false);
            }

            [Fact]
            public void WhenAscendingThenBySortingCriteriumIsPassed_ThenResultShouldBeSorted()
            {
                // Arrange
                var criteriums = new[]
                {
                    new SortCriterium() {PropertyName = "Property1", Ascending = true},
                    new SortCriterium() {PropertyName = "Property2", Ascending = true}
                };

                // Act
                var result = source.OrderBy(criteriums).ToArray();

                // Assert
                result.IsAscending(x => x.Property1, x => x.Property2).ShouldBe(true);
            }

            [Fact]
            public void WhenDescendingThenBySortingCriteriumIsPassed_ThenResultShouldBeSorted()
            {
                // Arrange
                var criteriums = new[]
                {
                    new SortCriterium() {PropertyName = "Property1", Ascending = false},
                    new SortCriterium() {PropertyName = "Property2", Ascending = false}
                };

                // Act
                var result = source.OrderBy(criteriums).ToArray();

                // Assert
                result.IsAscending(x => x.Property1, x => x.Property2).ShouldBe(false);
            }

            private class TestClass
            {
                public int Property1 { get; set; }
                public int Property2 { get; set; }
            }
        }
    }

    internal static class TestExtensions
    {
        public static bool IsAscending<T, U>(this IList<T> data, Func<T, U> orderBySelector)
            where U : IComparable<U>
        {
            return !data.Where((t, i) => i > 0 && orderBySelector(t).CompareTo(orderBySelector(data[i - 1])) < 0).Any();
        }

        public static bool IsAscending<T, U, V>(this IList<T> data, Func<T, U> orderBySelector, Func<T, V> thenBySelector)
            where U : IComparable<U>
            where V : IComparable<V>
        {
            var result = true;

            for (var i = 0; i < data.Count; i++)
            {
                if (i <= 0) continue;

                if (orderBySelector(data[i]).CompareTo(orderBySelector(data[i - 1])) < 0)
                {
                    result = false;
                    break;
                }

                if (orderBySelector(data[i]).CompareTo(orderBySelector(data[i - 1])) != 0) continue;

                result = thenBySelector(data[i]).CompareTo(thenBySelector(data[i - 1])) > 0;
                if (result == false)
                {
                    break;
                }
            }

            return result;
        }
    }
}