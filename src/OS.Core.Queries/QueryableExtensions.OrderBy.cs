using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OS.Core.Queries
{
    public static partial class QueryableExtensions
    {
        public static IQueryable<T> OrderByAndPage<T>(this IQueryable<T> source, Query query)
        {
            return source.OrderBy(query.SortCriteria)
                .Skip(query.PageSize * (query.PageIndex - 1))
                .Take(query.PageSize);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IList<SortCriterium> criteria)
        {
            var expression = source.Expression;

            if (criteria.Count == 0)
            {
                return source;
            }

            for (var i = 0; i < criteria.Count; i++)
            {
                var @param = Expression.Parameter(typeof(T), "x");
                var @property = criteria[i].GetProperty(@param);

                var method = criteria[i].Ascending ?
                    i == 0 ? nameof(Queryable.OrderBy) : nameof(Queryable.ThenBy) :
                    i == 0 ? nameof(Queryable.OrderByDescending) : nameof(Queryable.ThenByDescending);

                var @typeParams = new[] { source.ElementType, property.Type };
                var @expressionParams = new[] { expression, Expression.Quote(Expression.Lambda(@property, @param)) };

                expression = Expression.Call(typeof(Queryable), method, @typeParams, expressionParams);
            }

            return source.Provider.CreateQuery<T>(expression);
        }
    }
}