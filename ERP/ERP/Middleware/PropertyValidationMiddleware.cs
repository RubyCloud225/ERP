using ERP.Attributes;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP.ERP.Middleware
{
    public class PropertyValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public PropertyValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Validate required headers
            if (!context.Request.Headers.ContainsKey("User -Agent"))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                await context.Response.WriteAsync("Missing required header: User-Agent");
                return;
            }

            // Validate required query parameters
            if (!context.Request.Query.ContainsKey("requiredParam"))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                await context.Response.WriteAsync("Missing required query parameter: requiredParam");
                return;
            }

            // Validate content type
            if (context.Request.ContentType != null && !context.Request.ContentType.Contains("application/json"))
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType; // Unsupported Media Type
                await context.Response.WriteAsync("Unsupported content type. Only application/json is allowed.");
                return;
            }

            // Validate request method
            if (context.Request.Method != HttpMethods.Post)
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed; // Method Not Allowed
                await context.Response.WriteAsync("Method not allowed. Only POST requests are accepted.");
                return;
            }

            // Validate required body properties (for JSON requests)
            if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                context.Request.EnableBuffering(); // Allow reading the request body multiple times
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Reset the stream position for the next middleware

                    // Deserialize the JSON body
                    var jsonDoc = JsonDocument.Parse(body);
                    var root = jsonDoc.RootElement;

                    // Check for required properties
                    if (!root.TryGetProperty("requiredProperty", out _))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                        await context.Response.WriteAsync("Missing required property: requiredProperty");
                        return;
                    }

                    // Add more property checks as needed
                    //Check if request has a file
                    if (context.Request.Form.Files.Count > 0)
                    {
                        var file = context.Request.Form.Files[0];
                        var erpAttribute = context.GetEndpoint()?.Metadata.GetMetadata<ERPAttribute>();
                        if (erpAttribute != null)
                        {
                            if (file.Length > erpAttribute.MaxFileSize)
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                                await context.Response.WriteAsync("File size exceeds the maximum allowed size.");
                                return;
                            }
                            if (!erpAttribute.AllowedFileTypes.Split(',').Contains(Path.GetExtension(file.FileName)))
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request
                                await context.Response.WriteAsync("File type is not allowed.");
                                return;
                            }
                        }
                    }
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}