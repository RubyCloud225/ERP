namespace ERP.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error");
                // Create a error response
                var errorResponse = new
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                    StackTrace = ex.StackTrace,
                    Detailed = ex.Message
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}