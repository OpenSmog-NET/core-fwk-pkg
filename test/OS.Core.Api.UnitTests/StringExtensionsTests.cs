using OS.Core.QueryParsing;
using Shouldly;
using System.Linq;
using Xunit;

namespace OS.Core.UnitTests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("('abc')", 1)]
        [InlineData("('abc', 'def')", 2)]
        [InlineData("('abc', '')", 0)]
        [InlineData("'abc', 'def'", 0)]
        [InlineData("abc, def", 0)]
        public void GivenAnInput_WhenGetTextValueIsCalled_ThenApropriateNumberOfMatchesIsReturned(string input, int expectedMatchesCount)
        {
            var result = input.GetTextValues();

            result.Count().ShouldBe(expectedMatchesCount);
        }
    }
}