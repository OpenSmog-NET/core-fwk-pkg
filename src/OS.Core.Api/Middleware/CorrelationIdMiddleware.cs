using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace OS.Core.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<CorrelationIdMiddleware>();
            
            if (!context.Request.Headers.ContainsKey(Constants.RequestCorrelation.RequestHeaderName))
            {
                var correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(Constants.RequestCorrelation.RequestHeaderName, correlationId);
                logger.LogInformation($"{context.Request.Method} {context.Request.Path} : new request chain {correlationId}");
            }
            else
            {
                logger.LogDebug($"{context.Request.Method} {context.Request.GetDisplayUrl()} : already in request chain");
            }

            using (
                LogContext.PushProperty(Constants.RequestCorrelation.LogPropertyName, 
                context.Request.Headers[Constants.RequestCorrelation.RequestHeaderName]))
            {
                await next.Invoke(context);
            }
        }
    }
}
