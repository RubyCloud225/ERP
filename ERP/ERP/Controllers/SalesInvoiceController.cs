using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateSalesInvoice([FromBody] Model.ApplicationDbContext.GenerateSalesInvoiceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is null.");
            }

            try
            {
                await _salesInvoiceService.GenerateSalesInvoiceAsync(
                    request.Id,
                    request.BlobName,
                    request.InvoiceDate,
                    request.InvoiceNumber,
                    request.CustomerName,
                    request.CustomerAddress,
                    request.TotalAmount,
                    request.SalesTax,
                    request.NetAmount,
                    request.UserId
                );

                return Ok(new { message = "Sales invoice generated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is set up
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSalesInvoice([FromQuery] int id, [FromQuery] int userId)
        {
            try
            {
                await _salesInvoiceService.DeleteSalesInvoiceAsync(id, userId);
                return Ok(new { message = "Sales invoice deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log exception if logging is set up
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSalesInvoice([FromBody] Model.ApplicationDbContext.SalesInvoice updatedSalesInvoice)
        {
            if (updatedSalesInvoice == null)
            {
                return BadRequest("Request body is null.");
            }
            try
            {
                await _salesInvoiceService.UpdateSalesInvoiceAsync(
                    updatedSalesInvoice.Id,
                    updatedSalesInvoice.BlobName,
                    updatedSalesInvoice.InvoiceDate,
                    updatedSalesInvoice.InvoiceNumber,
                    updatedSalesInvoice.CustomerName,
                    updatedSalesInvoice.CustomerAddress,
                    updatedSalesInvoice.TotalAmount,
                    updatedSalesInvoice.SalesTax,
                    updatedSalesInvoice.NetAmount,
                    updatedSalesInvoice.UserId
                );
                return Ok(new { message = "Sales invoice updated successfully." });
            }
            catch (Exception ex)
            {
                // Log exception if logging is set up
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
