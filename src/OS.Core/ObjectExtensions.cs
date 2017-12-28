using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OS.Core
{
    /// <summary>
    /// @object.GetPropertyName(x => x.Property)
    /// </summary>
    public static class ObjectExtensions
    {
        public static string GetPropertyName<T, U>(this T @object, Expression<Func<T, U>> propertyExpression)
        {
            var property = @object.GetProperty<T, U>(propertyExpression);

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);

            if (getMethod == null)
            {
                // This shouldn't happen - the expression would reject the property before reaching this far.
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return property.Name;
        }

        public static PropertyInfo GetProperty<T, U>(this T @object, Expression<Func<T, U>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            return memberExpression.Member as PropertyInfo;
        }
    }
}