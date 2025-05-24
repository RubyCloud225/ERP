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
                    request.NetAmount
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
        public async Task<IActionResult> DeleteSalesInvoice([FromBody] Model.ApplicationDbContext.SalesInvoice deletedSalesInvoice)
        {
            await _salesInvoiceService.DeleteSalesInvoiceAsync(deletedSalesInvoice.Id);
            return Ok(new { message = "Sales invoice deleted successfully." });
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateSalesInvoice(int Id, string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount)
        {
            await _salesInvoiceService.UpdateSalesInvoiceAsync(Id, blobName, invoiceDate, invoiceNumber, customerName, customerAddress, totalAmount, salesTax, netAmount);
            return Ok(new { message = "Sales invoice updated successfully." });
        }
    }
}
