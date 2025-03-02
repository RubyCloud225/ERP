using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ERP.Middleware
{
    public class RateLimitingMiddleware
    {
        public static readonly ConcurrentDictionary<string, int> _requestCounts = new ConcurrentDictionary<string, int>();
        private static readonly int _maxRequestsPerMinute = 5; //Limit on prompts

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userIp = context.Connection.RemoteIpAddress.ToString();
            var requestCount = _requestCounts.GetOrUpdate(userIp, 1, (Key, count) => count + 1);
            if (requestCount > _maxRequestsPerMinute)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests");
                return;
            }
            await next(context);
        }
    }
}