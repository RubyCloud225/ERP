using Microsoft.Extensions.Caching.Memory;

namespace ERP.Middleware
{
    public class PropertyCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        public PropertyCachingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // create a unique cache key based on the request path
            var cacheKey = $"{context.Request.Path}";
            // Declare the variable to hold cached properties
            Dictionary<string, string>? cachedProperties = null;
            // Check if the peroperies are already cached
            if (!_cache.TryGetValue(cacheKey, out var cachedPropertiesTemp))
            {
                // If the properties are cached, return the cached properties
                cachedProperties = new Dictionary<string, string>
                {
                    {"User -Agent", context.Request.Headers["User -Agent"].ToString()},
                    {"RequestPath", context.Request.Path.ToString()},
                    {"RequestMethod", context.Request.Method.ToString()},
                    {"RequestQuery", context.Request.QueryString.ToString()},
                    {"Timestamp", DateTime.Now.ToString()}
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, cachedProperties, cacheEntryOptions);
            }
            else
            {
                cachedProperties = cachedPropertiesTemp as Dictionary<string, string>;
                Console.WriteLine("Retrieved from Cache:");
            }
            if (cachedProperties != null)
            {
                foreach (var property in cachedProperties)
                {
                    Console.WriteLine($"{property.Key}: {property.Value}");
                }
            }
            await _next(context);
        }
    }
}