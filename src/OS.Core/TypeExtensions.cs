using System;
using System.Linq;
using System.Reflection;

namespace OS.Core
{
    public static class TypeExtensions
    {
        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            var modelProperties = type.GetProperties();
            var propertyInfo = modelProperties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property {propertyName} does not exist.");
            }

            return propertyInfo;
        }
    }
}