using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Middleware
{
    public class PropertyCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<PropertyCachingMiddleware> _logger;

        private static readonly ConcurrentDictionary<string, (Dictionary<string, string> Properties, DateTime Expiration)> _cachedProperties = new();

        public PropertyCachingMiddleware(RequestDelegate next, IHostEnvironment hostEnvironment, ILogger<PropertyCachingMiddleware> logger)
        {
            _next = next;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Incoming request headers:");
            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation("{Key}: {Value}", header.Key, header.Value.ToString());
            }

            _logger.LogInformation("Current environment: {Env}", _hostEnvironment.EnvironmentName);

            _logger.LogInformation("PropertyCachingMiddleware invoked for {Path}", context.Request.Path);

            if (!_hostEnvironment.IsEnvironment("Testing"))
            {
                if (!context.Request.Headers.ContainsKey("User-Agent"))
                {
                    _logger.LogWarning("Missing User-Agent header in request to {Path}", context.Request.Path);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Missing required header: User-Agent");
                    return;
                }
                else
                {
                    _logger.LogInformation("User-Agent header: {UserAgent}", context.Request.Headers["User-Agent"].ToString());
                }
            }
            else
            {
                _logger.LogInformation("Skipping User-Agent header check in Testing environment");
            }

            var cacheKey = $"PropertiesCache_{context.Request.Path}";
            _logger.LogInformation("Cache key for request: {CacheKey}", cacheKey);

            Dictionary<string, string>? propertiesToCache = null;

            if (_cachedProperties.TryGetValue(cacheKey, out var cacheEntry))
            {
                if (cacheEntry.Expiration > DateTime.Now)
                {
                    _logger.LogInformation("Using cached properties for key {CacheKey}", cacheKey);
                    propertiesToCache = cacheEntry.Properties;
                }
                else
                {
                    _logger.LogInformation("Cache expired for key {CacheKey}, removing", cacheKey);
                    _cachedProperties.TryRemove(cacheKey, out _);
                }
            }

            if (propertiesToCache == null)
            {
                _logger.LogInformation("No valid cache found for key {CacheKey}, building properties", cacheKey);

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
                _cachedProperties[cacheKey] = (propertiesToCache, expirationTime);

                _logger.LogInformation("Cached properties for key {CacheKey} with expiration {Expiration}", cacheKey, expirationTime);
            }

            foreach (var property in propertiesToCache)
            {
                _logger.LogDebug("Caching property: {Key} = {Value}", property.Key, property.Value);
                context.Items[property.Key] = property.Value;
            }

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