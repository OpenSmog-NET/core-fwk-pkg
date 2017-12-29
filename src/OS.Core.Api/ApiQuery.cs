using Microsoft.AspNetCore.Mvc;
using OS.Core.Queries;
using OS.Core.QueryParsing;
using System;

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
        public abstract Query GetQuery();
        protected Query GetQuery<TModel>(Func<string, bool> allowedPropertyFilter = null, Func<string, bool> nestedPropertyFilter = null)
        {
            var query = new Query()
            {
                PageIndex = PageIndex > 0 ? PageIndex : DefaultPageIndex,
                PageSize = PageSize > 0 ? PageSize : DefaultPageSize,
                SortCriteria = Sort.ParseSortCriteria(),
                FilterCriteria = Filter.ParseFilterCriteria<TModel>(allowedPropertyFilter, nestedPropertyFilter)
            };

            return query;
        }
    }
}