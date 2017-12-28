using Microsoft.AspNetCore.Http;
using OS.Core.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OS.Core
{
    public class ApiResult
    {
        public ApiResult(HttpContext context)
        {
            CorrelationId = Guid.Parse(context.Request.Headers[Constants.RequestCorrelation.RequestHeaderName]);
        }

        public Guid CorrelationId { get; }

        public List<ApiError> Errors { get; } = new List<ApiError>();

        public bool HasError => Errors.Any();
    }

    public class ApiResult<T> : ApiResult
    {
        public ApiResult(HttpContext context)
            : base(context)
        {
        }

        public T Value { get; set; }
    }
}