using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LlmController : ControllerBase
    {
        private readonly IDocumentProcessor _documentProcessor;
        public LlmController(IDocumentProcessor documentProcessor)
        {
            _documentProcessor = documentProcessor;
        }
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Prompt is required");
            }
            var response = await _documentProcessor.GenerateResponseAsync(prompt);
            return Ok(new { Response = response });
        }
    }
}
