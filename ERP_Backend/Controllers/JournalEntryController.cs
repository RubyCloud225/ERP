using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService;

        public JournalEntryController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryDto request, [FromHeader] Guid? userId)
        {
            try
            {
                var result = await _journalEntryService.CreateJournalEntryAsync(request, userId);
                return CreatedAtAction(nameof(GetJournalEntryById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AmendJournalEntry(Guid id, [FromBody] CreateJournalEntryDto request, [FromHeader] Guid? userId)
        {
            try
            {
                var result = await _journalEntryService.AmendJournalEntryAsync(id, request, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJournalEntry(Guid id)
        {
            try
            {
                await _journalEntryService.DeleteJournalEntryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJournalEntryById(Guid id)
        {
            try
            {
                var result = await _journalEntryService.GetJournalEntryByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetJournalEntryByUserId(Guid userId)
        {
            try
            {
                var result = await _journalEntryService.GetJournalEntryByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
