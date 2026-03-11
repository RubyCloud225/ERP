using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet("properties")]
        public IActionResult GetCachedProperties()
        {
            var properties = HttpContext.Items
                .Where(kv => kv.Key is string)
                .ToDictionary(kv => kv.Key.ToString()!, kv => kv.Value?.ToString() ?? "");

            return Ok(properties);
        }
    }
}