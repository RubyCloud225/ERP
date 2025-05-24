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
            if (context.Connection?.RemoteIpAddress != null)
            {
                var userIp = context.Connection.RemoteIpAddress.ToString();
                var requestCount = _requestCounts.AddOrUpdate(userIp, 1, (Key, count) => count + 1);
                if (requestCount > _maxRequestsPerMinute)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Unable to determine IP address");
                return;
            }
            
            await next(context);
        }
    }
}