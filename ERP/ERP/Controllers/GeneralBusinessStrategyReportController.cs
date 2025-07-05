using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneralBusinessStrategyReportController : ControllerBase
    {
        private readonly ILlmService _llmService;

        public GeneralBusinessStrategyReportController(ILlmService llmService)
        {
            _llmService = llmService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromQuery] Guid userId, [FromBody] string userInput)
        {
            try
            {
                var report = await _llmService.GenerateGeneralBusinessStrategyReportAsync(userId, userInput);
                return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            try
            {
                var report = await _llmService.GetGeneralBusinessStrategyReportByIdAsync(id);
                if (report == null)
                {
                    return NotFound($"General business strategy report with id {id} not found.");
                }
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] string updatedContent)
        {
            try
            {
                var report = await _llmService.UpdateGeneralBusinessStrategyReportAsync(id, updatedContent);
                return Ok(report);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"General business strategy report with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("pitchdeck")]
        public async Task<IActionResult> GeneratePitchDeck([FromQuery] Guid userId, [FromBody] string userInput)
        {
            try
            {
                var pitchDeckContent = await _llmService.GeneratePitchDeckAsync(userId, userInput);
                return Ok(new { PitchDeckContent = pitchDeckContent });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
