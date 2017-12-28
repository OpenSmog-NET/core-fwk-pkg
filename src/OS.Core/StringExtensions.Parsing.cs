using System;
using System.Globalization;
using System.Reflection;

namespace OS.Core
{
    public static class StringExtensions
    {
        public static bool ParseBool(this string value, PropertyInfo property)
        {
            if (bool.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            throw new ArgumentException($"{property.Name} requires boolean, {value} is not a boolean.");
        }

        public static int ParseInt(this string value, PropertyInfo property)
        {
            if (int.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            throw new ArgumentException($"{property.Name} requires ints, {value} is not an int value.");
        }

        public static long ParseLong(this string value, PropertyInfo property)
        {
            if (long.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            throw new ArgumentException($"{property.Name} requires ints, {value} is not a valid long.");
        }

        public static decimal ParseDecimal(this string value, PropertyInfo property)
        {
            if (decimal.TryParse(value, out var parsedValue))
            {
                return parsedValue;
            }

            throw new ArgumentException($"{property.Name} requires decimals, {value} is not a valid decimal.");
        }

        public static DateTime ParseDateTime(this string value, PropertyInfo property)
        {
            if (DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out var parsedValue))
            {
                return parsedValue;
            }

            throw new ArgumentException($"{property.Name} requires ISO8601 date format, {value} is not a valid date.");
        }

        public static object ParseEnum(this string value, PropertyInfo property)
        {
            return Enum.Parse(property.PropertyType, value, true);
        }

        public static object ParseNullableEnum(this string value, PropertyInfo property)
        {
            return Enum.Parse(Nullable.GetUnderlyingType(property.PropertyType), value, true);
        }
    }
}