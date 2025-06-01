using System.Collections.Concurrent;

namespace ERP.Middleware
{
    public class PropertyCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (Dictionary<string, string> Properties, DateTime Expiration)> _cachedProperties = new();
        public PropertyCachingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // create a unique cache key based on the request path
            var cacheKey = $"PropertiesCache_{context.Request.Path}";
            // Declare the variable to hold cached properties
            Dictionary<string, string>? propertiesToCache = null;
            // Try to get the cached properties
            if (_cachedProperties.TryGetValue(cacheKey, out var cacheEntry))
            {
                if (cacheEntry.Expiration > DateTime.Now)
                {
                    Console.WriteLine("Using cached properties");
                    propertiesToCache = cacheEntry.Properties;
                }
                else
                {
                    _cachedProperties.TryRemove(cacheKey, out _);
                }
            }
            if (propertiesToCache == null)
            {
                propertiesToCache = new Dictionary<string, string>
                {
                    { "User-Agent", context.Request.Headers["User-Agent"].ToString() },
                    { "Accept-Language", context.Request.Headers["Accept-Language"].ToString() },
                    { "Host", context.Request.Headers["Host"].ToString() },
                    { "Content-Type", context.Request.Headers["Content-Type"].ToString() },
                    { "Request-Method", context.Request.Method },
                    { "Request-Scheme", context.Request.Scheme },
                    { "Request-Host", context.Request.Host.ToString() },
                    { "Request-PathBase", context.Request.PathBase.ToString() },
                    { "Request-Path", context.Request.Path },
                    { "Request-Query", context.Request.QueryString.ToString() },
                    { "Request-Protocol", context.Request.Protocol },
                    { "Request-ContentLength", context.Request.ContentLength?.ToString() ?? "0" },
                    { "Timestamp", DateTime.UtcNow.ToString("o") }
                };
                var expirationTime = DateTime.UtcNow.AddSeconds(10);
                // Add the properties to the cache with an expiration time
                _cachedProperties[cacheKey] = (propertiesToCache, expirationTime);
            }
            foreach (var property in propertiesToCache)
            {
                Console.WriteLine($"Caching property: {property.Key} = {property.Value}");
                context.Items[property.Key] = property.Value;
            }
            // Call the next middleware in the pipeline
            await _next(context);
        }
        public static Dictionary<string, string> GetCache()

        {

            return _cachedProperties
                .SelectMany(entry => entry.Value.Properties.Select(property => new KeyValuePair<string, string>($"{entry.Key}_{property.Key}", property.Value)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        }
    }
}