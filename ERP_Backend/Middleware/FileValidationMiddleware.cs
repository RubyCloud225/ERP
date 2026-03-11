namespace ERP.Middleware
{
    public class FileValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private const long MaxFileSize = 1024 * 1024 * 10; // 10MB
        private readonly string[] _allowedExtensions = new[] { ".txt", ".pdf", ".docx", ".xlsx", ".jpg", ".png", ".csv" };

        public FileValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // check if the request is for a file
            if (context.Request.HasFormContentType && context.Request.Form.Files.Count > 0)
            {
                var file = context.Request.Form.Files[0];
                // Validate file size
                if (file.Length > MaxFileSize)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("File size exceeds the maximum allowed size.");
                    return;
                }
                //Validate file type
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("File type is not allowed.");
                    return;
                }
            }
            await _next(context);
        }
    }
}