using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERP.Model;
using ERP.Service;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisionMissionReportController : ControllerBase
    {
        private readonly ILlmService _llmService;

        public VisionMissionReportController(ILlmService llmService)
        {
            _llmService = llmService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromQuery] Guid userId, [FromBody] string userAims)
        {
            try
            {
                var report = await _llmService.GenerateVisionMissionAlignmentReportAsync(userId, userAims);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            try
            {
                var report = await _llmService.GetVisionMissionReportByIdAsync(id);
                return Ok(report);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Report with id {id} not found.");
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
                var updatedReport = await _llmService.UpdateVisionMissionReportAsync(id, updatedContent);
                return Ok(updatedReport);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Report with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
