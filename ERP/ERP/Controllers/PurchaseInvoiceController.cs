using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        public PurchaseInvoiceController(IPurchaseInvoiceService purchaseInvoiceService)
        {
            _purchaseInvoiceService = purchaseInvoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseInvoice([FromBody] ApplicationDbContext.CreatePurchaseInvoiceDto request)
        {
            try
            {
                var result = await _purchaseInvoiceService.ProcessPurchaseInvoiceAsync(request);
                return CreatedAtAction(nameof(GetPurchaseInvoiceById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AmendPurchaseInvoice(Guid id, [FromBody] ApplicationDbContext.CreatePurchaseInvoiceDto request)
        {
            try
            {
                var result = await _purchaseInvoiceService.AmendPurchaseInvoiceAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase invoice with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseInvoice(Guid id)
        {
            try
            {
                await _purchaseInvoiceService.DeletePurchaseInvoiceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase invoice with id {id} not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseInvoiceById(Guid id)
        {
            try
            {
                var result = await _purchaseInvoiceService.GetPurchaseInvoiceByIdAsync(id);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound($"Purchase invoice with id {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPurchaseInvoiceByUserId(Guid userId)
        {
            try
            {
                var result = await _purchaseInvoiceService.GetPurchaseInvoiceByUserIdAsync(userId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound($"Purchase invoice for user with id {userId} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPurchaseInvoices()
        {
            try
            {
                var result = await _purchaseInvoiceService.GetAllPurchaseInvoicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
