using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OS.Core.Queries
{
    public static class CriteriumExtensions
    {
        public static Expression GetProperty(this Criterium criterium, ParameterExpression parameter, Type typeConstraint = null)
        {
            Expression left;
            if (criterium.HasNestedProperty)
            {
                left = GetPropertyExpression(parameter, criterium.PropertyName);
                left = GetPropertyExpression(left, criterium.NestedProperty, typeConstraint);
            }
            else
            {
                left = GetPropertyExpression(parameter, criterium.PropertyName, typeConstraint);
            }
            return left;
        }

        public static Expression GetPropertyExpression(Expression parameter, string propertyName, Type typeConstraint = null)
        {
            var property = parameter.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException($"{propertyName} property is not defined.");

            if (typeConstraint != null && property.PropertyType != typeConstraint)
            {
                throw new ArgumentException($"{propertyName} property should be defined as {typeConstraint.Name} type.");
            }

            return Expression.PropertyOrField(parameter, property.Name);
        }
    }
}