using OS.Core.QueryParsing;
using Shouldly;
using System.Linq;
using Xunit;

namespace OS.Core.UnitTests
{
    public partial class QueryParsingExtensions
    {
        public class GivenSingleSortCriteria
        {
            [Theory]
            [InlineData("intProperty", nameof(TestClass.IntProperty), true)]
            [InlineData("-longProperty", nameof(TestClass.LongProperty), false)]
            public void WhenParsingValidSortString_ThenCriteriumWithPropertiesIsReturned(string sort, string expectedPropertyName, bool expectedAscendance)
            {
                // Act
                var criterium = sort.ParseSortCriteria().First();

                // Assert
                criterium.PropertyName.ShouldBe(expectedPropertyName, StringCompareShould.IgnoreCase);
                criterium.Ascending.ShouldBe(expectedAscendance);
            }

            public class TestClass
            {
                public int IntProperty { get; set; }
                public long LongProperty { get; set; }
            }
        }

        public class GivenMultipleSortCriteria
        {
            [Theory]
            [InlineData("intProperty,longProperty", 2, true)]
            [InlineData("intProperty,longProperty,decimalProperty", 3, true)]
            [InlineData("-intProperty,-longProperty,-decimalProperty", 3, false)]
            public void WhenParsingValidSortString_ThenCriteriumWithPropertiesIsReturned(string sort, int expectedCriteriaCount, bool expectedAscendance)
            {
                // Act
                var criteria = sort.ParseSortCriteria();

                // Assert
                criteria.Count.ShouldBe(expectedCriteriaCount);
                criteria.All(x => x.Ascending == expectedAscendance).ShouldBe(true);
            }

            public class TestClass
            {
                public int IntProperty { get; set; }
                public long LongProperty { get; set; }
            }
        }
    }
}