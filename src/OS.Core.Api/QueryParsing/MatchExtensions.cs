using OS.Core.Queries;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OS.Core.QueryParsing
{
    internal static class MatchExtensions
    {
        internal const BindingFlags PropertyLookupFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static PropertyInfo GetProperty<TModel>(this Match match, FilterCriterium criterium, Func<string, bool> allowedPropertyFilter = null, Func<string, bool> nestedPropertyFilter = null)
        {
            var group = match.Groups["PropertyName"];
            var name = group.Value;
            var argEx = new ArgumentException($"Filtering with {name} is not allowed");

            PropertyInfo result = null;

            if (allowedPropertyFilter != null && !allowedPropertyFilter(name))
            {
                throw argEx;
            }

            if (Criterium.IsNestedProperty(name))
            {
                if (nestedPropertyFilter != null && !nestedPropertyFilter(name))
                {
                    throw argEx;
                }

                criterium.PropertyName = Criterium.GetMainPropertyName(name);
                criterium.NestedProperty = Criterium.GetNestedPropertyName(name);

                var mainPropertyInfo = typeof(TModel).GetProperty(criterium.PropertyName, PropertyLookupFlags);
                result = mainPropertyInfo.PropertyType.GetProperty(criterium.NestedProperty, PropertyLookupFlags);
            }
            else
            {
                criterium.PropertyName = name;
                result = typeof(TModel).GetProperty(name, PropertyLookupFlags);
            }

            return result;
        }

        public static void SetCriteriumOperator(this Match match, FilterCriterium criterium)
        {
            var operatorName = match.Groups["Operator"];

            if (Enum.TryParse(operatorName.Value, true, out CriteriumOperator result))
            {
                criterium.Operator = result;
            }
            else
            {
                throw new ArgumentException($"Operator {operatorName.Value} does not exist.");
            }
        }

        public static void SetCriteriumValue(this Match match, FilterCriterium criterium, PropertyInfo property)
        {
            var propertyValue = match.Groups["Value"];
            var textValue = propertyValue.Value;

            if (criterium.Operator == CriteriumOperator.In)
            {
                var textValues = textValue.GetTextValues();
                var values = property.ExtractValues(textValues).ToArray();

                if (values.Length == 0)
                {
                    throw new ArgumentException($"The 'In' operator has an invalid value: {textValue}");
                }

                criterium.Value = property.ToList(values);
            }
            else
            {
                criterium.Value = property.ExtractValue(textValue);
            }
        }
    }
}