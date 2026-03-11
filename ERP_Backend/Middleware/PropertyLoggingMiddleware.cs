using System.Security.Claims;
using ERP.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Middleware
{
    public class PropertyLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PropertyLoggingMiddleware> _logger;

        public PropertyLoggingMiddleware(RequestDelegate next, ILogger<PropertyLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("PropertyLoggingMiddleware invoked for {Path}", context.Request.Path);

                Guid? userId = null;

                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (Guid.TryParse(userIdString, out Guid parsedUserId))
                    {
                        userId = parsedUserId;
                        _logger.LogInformation("Authenticated user ID: {UserId}", userId);
                    }
                    else
                    {
                        _logger.LogWarning("User ID claim is not a valid GUID: {UserIdString}", userIdString);
                    }
                }
                else
                {
                    _logger.LogInformation("User is not authenticated.");
                }

                var properties = new Dictionary<string, string>
                {
                    { "User Agent", context.Request.Headers["User-Agent"].ToString() },
                    { "RequestPath", context.Request.Path.ToString() },
                    { "RequestMethod", context.Request.Method.ToString() },
                    { "RequestQuery", context.Request.QueryString.ToString() },
                    { "PropertyType", context.Request.Headers["PropertyType"].ToString() },
                    { "UserId", userId?.ToString() ?? "0" }
                };

                var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

                foreach (var property in properties)
                {
                    _logger.LogDebug("Logging property: {Key} = {Value}", property.Key, property.Value);

                    var logEntry = new ApplicationDbContext.PropertyLog
                    {
                        PropertyName = property.Key,
                        PropertyValue = property.Value,
                        PropertyType = property.Key == "UserId" ? "Integer" : "String",
                        LoggedAt = DateTime.Now,
                        UserId = userId
                    };
                    await dbContext.PropertyLogs.AddAsync(logEntry);
                }

                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Properties logged to database successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PropertyLoggingMiddleware.");
                // Optionally rethrow or handle error gracefully
            }

            await _next(context);
        }
    }
}