using System.Security.Claims;

namespace ERP.Middleware
{
    public class UserDataFilterMiddleware
    {
        private readonly RequestDelegate _next;
        public UserDataFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    context.Items["UserId"] = userId;
                    Console.WriteLine($"User Id: {userId}");
                }
            }
            else
            {
                Console.WriteLine("User is not authenticated.");
            }
            await _next(context);
        }
    }
}