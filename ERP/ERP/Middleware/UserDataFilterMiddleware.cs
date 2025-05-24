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
            // Get the user data from the database
            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    //Store the userId in the HttpContext.Items
                    context.Items["UserId"] = userId;
                    //Optionally, you can log the user Id for debugging
                    Console.WriteLine($"User  Id: {userId}");
                }
            }
            else
            {
                Console.WriteLine("User  is not autenticated.");
            }

            await _next(context);
        }
    }
}