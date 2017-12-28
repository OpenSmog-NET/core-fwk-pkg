using System.Linq;

namespace OS.Core.Queries
{
    public abstract class Criterium
    {
        public bool HasNestedProperty => !string.IsNullOrEmpty(NestedProperty);
        public string NestedProperty { get; set; }
        public string PropertyName { get; set; }

        public static string GetMainPropertyName(string property)
        {
            return property.Split('.').First();
        }

        public static string GetNestedPropertyName(string property)
        {
            return property.Split('.').Last();
        }

        public static bool IsNestedProperty(string property)
        {
            return property.Contains(".");
        }
    }
}