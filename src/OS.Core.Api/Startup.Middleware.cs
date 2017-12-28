using Microsoft.AspNetCore.Builder;
using OS.Core.Middleware;
using Serilog;

namespace OS.Core
{
    public static class StartupMiddlewareExtensions
    {
        public static IApplicationBuilder UseOpenSmogMiddlewares(this IApplicationBuilder app)
        {
            return app
                .UseCorrelationIdMiddleware()
                .UseRequestLoggingMiddleware();
        }

        public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder app, ILogger logger = null)
        {
            app.UseMiddleware<RequestLoggingMiddleware>(logger);

            return app;
        }
    }
}