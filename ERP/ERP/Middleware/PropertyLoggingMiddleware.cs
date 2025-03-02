using System.Security.Claims;
using ERP.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.UserSecrets;
using static ERP.Model.ApplicationDbContext;

namespace ERP.ERP.Middleware 
{
    public class PropertyLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public PropertyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            int? userId = null;
            
            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                Console.WriteLine("User is not Authenticated");
            };
            var properties = new Dictionary<string, string>
            {
                {"User Agent", context.Request.Headers["User-Agent"].ToString() },
                {"RequestPath", context.Request.Path.ToString() },
                {"RequestMethod", context.Request.Method.ToString() },
                {"RequestQuery", context.Request.QueryString.ToString() },
                {"PropertyType", context.Request.Headers["PropertyType"].ToString() },
                {"UserId", userId ?.ToString() ?? "0" }
            };
            // Log the Properties to the Database
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            foreach (var property in properties)
            {
                var logEntry = new PropertyLog
                {
                    PropertyName= property.Key,
                    PropertyValue= property.Value,
                    PropertyType= property.Key == "UserId" ? "Integer" : "String",
                    LoggedAt = DateTime.Now,
                    UserId = userId
                };
                await dbContext.PropertyLogs.AddAsync(logEntry);
            };
            await dbContext.SaveChangesAsync();
            await _next(context);
        }
    }
}
