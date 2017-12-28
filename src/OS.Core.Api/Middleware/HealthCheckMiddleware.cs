using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OS.Core.Middleware
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate next;

        public HealthCheckMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "GET" && context.Request.Path == "/probe")
            {
                context.Response.StatusCode = 200;
                return;
            }

            await next.Invoke(context);
        }
    }
}