using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesInvoiceController : ControllerBase
    {
        private readonly ISalesInvoiceService _salesInvoiceService;

        public SalesInvoiceController(ISalesInvoiceService salesInvoiceService)
        {
            _salesInvoiceService = salesInvoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] ApplicationDbContext.GenerateSalesInvoiceDto request, [FromQuery] string blobName, [FromQuery] Guid? userId)
        {
            try
            {
                var result = await _salesInvoiceService.GenerateSalesInvoiceAsync(request, blobName, userId);
                return CreatedAtAction(nameof(GetSalesInvoiceById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AmendSalesInvoice(Guid id, [FromBody] ApplicationDbContext.GenerateSalesInvoiceDto request, [FromQuery] string blobName, [FromQuery] Guid? userId)
        {
            try
            {
                var result = await _salesInvoiceService.AmendSalesInvoiceAsync(id, request, blobName, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Sales invoice with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesInvoice(Guid id)
        {
            try
            {
                await _salesInvoiceService.DeleteSalesInvoiceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Sales invoice with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesInvoiceById(Guid id)
        {
            try
            {
                var salesInvoice = await _salesInvoiceService.GetSalesInvoiceByIdAsync(id);
                if (salesInvoice == null)
                {
                    return NotFound($"Sales invoice with id {id} not found.");
                }
                return Ok(salesInvoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSalesInvoiceByUserId(Guid userId)
        {
            try
            {
                var salesInvoice = await _salesInvoiceService.GetSalesInvoiceByUserIdAsync(userId);
                if (salesInvoice == null)
                {
                    return NotFound($"Sales invoice for user with id {userId} not found.");
                }
                return Ok(salesInvoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
