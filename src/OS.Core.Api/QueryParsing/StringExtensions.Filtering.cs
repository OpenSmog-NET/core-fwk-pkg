using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OS.Core.QueryParsing
{
    internal static partial class StringExtensions
    {
        public static MatchCollection ParseFilter(this string filter)
        {
            const string filterRegex =
                @"(?<Filter>
                                   (?<PropertyName>.+?)\s+
                                   (?<Operator>eq|gt|ge|lt|le|sw|lk|in)\s+
                                   '?(?<Value>.+?)'?
                              )
                              (?:
                                  \s*$
                                 |\s+(?:and)\s+
                              )";

            var regex = new Regex(filterRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            var matches = regex.Matches(filter);
            return matches;
        }

        public static IEnumerable<string> GetTextValues(this string value)
        {
            var captures = new Regex(@"\( (?:'?(?<Value>[^,']+)'? ,? \s* )+ \)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled)
                .Match(value)
                .Groups["Value"]
                .Captures;

            foreach (Capture capture in captures)
            {
                if (!string.IsNullOrWhiteSpace(capture.Value))
                    yield return capture.Value;
            }
        }
    }
}