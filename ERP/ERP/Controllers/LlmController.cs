using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LlmController : ControllerBase
    {
        private readonly ILlmService _llmService;
        public LlmController(ILlmService llmService)
        {
            _llmService = llmService;
        }
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Prompt is required");
            }
            var response = await _llmService.GenerateResponseAsync(prompt);
            return Ok(new { Response = response });
        }
    }
}