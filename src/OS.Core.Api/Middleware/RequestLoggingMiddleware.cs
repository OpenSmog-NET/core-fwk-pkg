using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OS.Core.Middleware
{
    public class RequestLoggingMiddleware
    {
        private const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private readonly ILogger logger;

        private readonly RequestDelegate next;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger logger = null)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? Log.ForContext<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var sw = Stopwatch.StartNew();
            try
            {
                await next(httpContext);

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : this.logger;
                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode,
                    sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex) when (LogException(httpContext, sw, ex))
            {
            }
            finally
            {
                sw.Stop();
            }
        }

        private bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        private ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = logger
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }
    }
}