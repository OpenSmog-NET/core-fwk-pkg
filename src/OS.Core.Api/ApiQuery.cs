using Microsoft.AspNetCore.Mvc;
using OS.Core.Queries;
using OS.Core.QueryParsing;

namespace OS.Core
{
    public abstract class ApiQuery
    {
        public const int DefaultPageSize = 10;
        public const int DefaultPageIndex = 1;

        [FromQuery]
        public int PageIndex { get; set; }

        [FromQuery]
        public int PageSize { get; set; }

        [FromQuery]
        public string Sort { get; set; }

        [FromQuery]
        public string Filter { get; set; }
        public abstract TQuery GetQuery<TQuery>() where TQuery : Query, new();
        protected TQuery GetQuery<TQuery, TModel>()
            where TQuery : Query, new()
        {
            var query = new TQuery()
            {
                PageIndex = PageIndex > 0 ? PageIndex : DefaultPageIndex,
                PageSize = PageSize > 0 ? PageSize : DefaultPageSize,
                SortCriteria = Sort.ParseSortCriteria(),
                FilterCriteria = Filter.ParseFilterCriteria<TModel>()
            };

            return query;
        }
    }
}