using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace OS.Core
{
    public static class PropertyInfoExtensions
    {
        public static bool Is<T>(this PropertyInfo property)
            where T : struct
        {
            return property.PropertyType == typeof(T) || property.PropertyType == typeof(T?);
        }

        public static bool IsEnum(this PropertyInfo property)
        {
            return property.PropertyType.IsEnum;
        }

        public static bool IsNullableEnum(this PropertyInfo property)
        {
            return Nullable.GetUnderlyingType(property.PropertyType)?.IsEnum == true;
        }

        public static object ExtractValue(this PropertyInfo property, string value)
        {
            if (IsEnum(property))
            {
                return value.ParseEnum(property);
            }

            if (IsNullableEnum(property))
            {
                return value.ParseNullableEnum(property);
            }

            if (Is<bool>(property))
            {
                return value.ParseBool(property);
            }

            if (Is<int>(property))
            {
                return value.ParseInt(property);
            }

            if (Is<long>(property))
            {
                return value.ParseLong(property);
            }

            if (Is<DateTime>(property))
            {
                return value.ParseDateTime(property);
            }

            if (Is<decimal>(property))
            {
                return value.ParseDecimal(property);
            }

            throw new ArgumentException($"The type of property {property.Name} is not supported");
        }

        public static IEnumerable<object> ExtractValues(this PropertyInfo property, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                yield return property.ExtractValue(value);
            }
        }

        public static IList ToList(this PropertyInfo property, IEnumerable<object> values)
        {
            var listType = typeof(List<>).MakeGenericType(property.PropertyType);
            var result = (IList)Activator.CreateInstance(listType);

            foreach (var value in values)
            {
                result.Add(value);
            }

            return result;
        }
    }
}