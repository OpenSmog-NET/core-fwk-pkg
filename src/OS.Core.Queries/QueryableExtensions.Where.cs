using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OS.Core.Queries
{
    public static partial class QueryableExtensions
    {
        public static IReadOnlyDictionary<CriteriumOperator, Func<FilterCriterium, Expression, Expression, Expression>>
            OperatorExpressions = new Dictionary<CriteriumOperator, Func<FilterCriterium, Expression, Expression, Expression>>()
            {
                { CriteriumOperator.In, ApplyInCriterium },         // IList.Contains?
                { CriteriumOperator.Sw, ApplySwCriterium },         // StartsWith?
                { CriteriumOperator.Lk, ApplyLkCriterium },         // string.Contains?
                { CriteriumOperator.Eq, ApplyCompareCriterium },    // ==
                { CriteriumOperator.Gt, ApplyCompareCriterium },    // >
                { CriteriumOperator.Ge, ApplyCompareCriterium },    // >=
                { CriteriumOperator.Lt, ApplyCompareCriterium },    // <
                { CriteriumOperator.Le, ApplyCompareCriterium }     // <=
            };

        public static IReadOnlyDictionary<CriteriumOperator, Func<Expression, Expression, Expression>>
            CompareOperatorExpressions = new Dictionary<CriteriumOperator, Func<Expression, Expression, Expression>>()
            {
                { CriteriumOperator.Eq, Expression.Equal },
                { CriteriumOperator.Gt, Expression.GreaterThan },
                { CriteriumOperator.Ge, Expression.GreaterThanOrEqual },
                { CriteriumOperator.Lt, Expression.LessThan },
                { CriteriumOperator.Le, Expression.LessThanOrEqual }
            };

        public static IQueryable<T> Where<T>(this IQueryable<T> source, Query query)
        {
            if (query.FilterCriteria.Count == 0)
            {
                return source;
            }

            Expression whereClause = null;
            var @param = Expression.Parameter(typeof(T), "x");

            query.FilterCriteria.ForEach(criterium =>
            {
                var left = criterium.GetProperty(@param);
                var @value = Expression.Constant(criterium.Value);
                var predicate = OperatorExpressions[criterium.Operator](criterium, left, @value);

                whereClause = whereClause == null ? predicate : Expression.AndAlso(whereClause, predicate);
            });

            return source.Where(Expression.Lambda<Func<T, bool>>(whereClause, @param));
        }

        private static Expression ApplyInCriterium(FilterCriterium criterium, Expression left, Expression valueExpression)
        {
            var typedList = (IList)criterium.Value;
            var containsMethod = typeof(Enumerable).GetMethods().Where(x => x.Name == "Contains").Single(x => x.GetParameters().Length == 2).MakeGenericMethod(left.Type); // signature of containsMethod: bool Contains<TSource>(IEnumerable<TSource> source, TSource value)
            return Expression.Call(containsMethod, typedList.AsQueryable().Expression, left);
        }

        private static Expression ApplySwCriterium(FilterCriterium criterium, Expression left, Expression valueExpression)
        {
            return Expression.Call(left, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) }), valueExpression);
        }

        private static Expression ApplyLkCriterium(FilterCriterium criterium, Expression left, Expression valueExpression)
        {
            return Expression.Call(left, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }), valueExpression);
        }

        private static Expression ApplyCompareCriterium(FilterCriterium criterium, Expression left, Expression valueExpression)
        {
            return CompareOperatorExpressions[criterium.Operator](left, valueExpression);
        }
    }
}