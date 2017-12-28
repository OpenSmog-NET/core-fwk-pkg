using OS.Core.Queries;
using OS.Core.QueryParsing;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OS.Core.UnitTests
{
    public partial class QueryParsingExtensions
    {
        public class GivenASingleFilter
        {
            [Theory]
            [InlineData("intProperty eq 1", "intProperty", CriteriumOperator.Eq, 1)]
            [InlineData("intProperty ge 4", "intProperty", CriteriumOperator.Ge, 4)]
            [InlineData("intProperty gt 5", "intProperty", CriteriumOperator.Gt, 5)]
            [InlineData("intProperty le 3", "intProperty", CriteriumOperator.Le, 3)]
            [InlineData("intProperty lt 2", "intProperty", CriteriumOperator.Lt, 2)]
            public void WhenParsingValidNonNestedFilter_CriteriumWithPropertiesIsReturned(string filter, string expectedProperty, CriteriumOperator expectedOperator, object expectedValue)
            {
                // Act
                var criterium = filter.ParseFilterCriteria<TestClass>().First();

                // Assert
                criterium.PropertyName.ShouldBe(expectedProperty);
                criterium.NestedProperty.ShouldBeNullOrEmpty();
                criterium.Operator.ShouldBe(expectedOperator);
                criterium.Value.ShouldBe(expectedValue);
            }

            [Theory]
            [InlineData("nested.intProperty eq 1", "nested", "intProperty", CriteriumOperator.Eq, 1)]
            [InlineData("nested.intProperty ge 4", "nested", "intProperty", CriteriumOperator.Ge, 4)]
            [InlineData("nested.intProperty gt 5", "nested", "intProperty", CriteriumOperator.Gt, 5)]
            [InlineData("nested.intProperty le 3", "nested", "intProperty", CriteriumOperator.Le, 3)]
            [InlineData("nested.intProperty lt 2", "nested", "intProperty", CriteriumOperator.Lt, 2)]
            public void WhenParsingValidNestedFilter_CriteriumWithPropertiesIsReturned(string filter, string expectedProperty, string expectedNestedProperty, CriteriumOperator expectedOperator, object expectedValue)
            {
                // Act
                var criterium = filter.ParseFilterCriteria<TestClass>().First();

                // Assert
                criterium.PropertyName.ShouldBe(expectedProperty);
                criterium.NestedProperty.ShouldBe(expectedNestedProperty);
                criterium.Operator.ShouldBe(expectedOperator);
                criterium.Value.ShouldBe(expectedValue);
            }

            [Theory]
            [InlineData("intProperty in ('1')", "intProperty", CriteriumOperator.In, typeof(List<int>), 1)]
            [InlineData("intProperty in ('1', '2')", "intProperty", CriteriumOperator.In, typeof(List<int>), 2)]
            [InlineData("intProperty in ('1', '2', '3')", "intProperty", CriteriumOperator.In, typeof(List<int>), 3)]
            public void WhenParsingValidFilterWithInOperator_CriteriumsWithPropertiesIsReturned(string filter, string expectedProperty, CriteriumOperator expectedOperator, Type expectedValueType, int expectedValueLength)
            {
                // Act
                var criterium = filter.ParseFilterCriteria<TestClass>().First();

                // Assert
                criterium.PropertyName.ShouldBe(expectedProperty);
                criterium.Operator.ShouldBe(expectedOperator);
                criterium.Value.GetType().ShouldBe(expectedValueType);
                ((IList)criterium.Value).Count.ShouldBe(expectedValueLength);
            }

            [Theory]
            [InlineData("intProperty in ()")]
            [InlineData("intProperty in ('')")]
            [InlineData("intProperty in ('1', '', '3')")]
            public void WhenParsingInvalidFilterWithInOperator_ArgumentException(string filter)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    // Act & Assert
                    var criterium = filter.ParseFilterCriteria<TestClass>().First();
                });
            }

            [Theory]
            [InlineData("intProperty in ('ab1')")]
            [InlineData("intProperty in ('1', 'x', '3')")]
            [InlineData("intProperty in ('100.0f')")]
            [InlineData("intProperty in ('100.0')")]
            public void WhenParsingInvalidFilterValuesWithInOperator_ArgumentException(string filter)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    // Act & Assert
                    var criterium = filter.ParseFilterCriteria<TestClass>().First();
                });
            }

            public class TestClass
            {
                public int IntProperty { get; set; }
                public TestNestedClass Nested { get; set; }
            }

            public class TestNestedClass
            {
                public int IntProperty { get; set; }
            }
        }
    }
}