using OS.Core.Queries;
using System;
using System.Collections.Generic;

namespace OS.Core.QueryParsing
{
    internal static class QueryParsingExtensions
    {
        public static IList<SortCriterium> ParseSortCriteria(this string source)
        {
            var criteria = new List<SortCriterium>();

            if (string.IsNullOrEmpty(source))
            {
                return criteria;
            };

            var tokens = source.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            tokens.ForEach(token =>
            {
                token.Trim(' ');

                var @descending = token.StartsWith("-");
                var property = @descending ? token.TrimStart('-') : token;

                var criterium = Criterium.IsNestedProperty(property)
                    ? new SortCriterium() { PropertyName = Criterium.GetMainPropertyName(property), NestedProperty = Criterium.GetNestedPropertyName(property) }
                    : new SortCriterium() { PropertyName = property };

                criterium.Ascending = !@descending;
                criteria.Add(criterium);
            });

            return criteria;
        }

        public static IList<FilterCriterium> ParseFilterCriteria<TModel>(this string source, Func<string, bool> allowedPropertyFilter = null, Func<string, bool> nestedPropertyFilter = null)
        {
            var criteria = new List<FilterCriterium>();

            if (string.IsNullOrEmpty(source))
            {
                return criteria;
            }

            var matches = source.ParseFilter();
            matches.ForEach(match =>
            {
                var criterium = new FilterCriterium();
                var property = match.GetProperty<TModel>(criterium, allowedPropertyFilter, nestedPropertyFilter);

                match.SetCriteriumOperator(criterium);
                match.SetCriteriumValue(criterium, property);

                criteria.Add(criterium);
            });

            return criteria;
        }
    }
}