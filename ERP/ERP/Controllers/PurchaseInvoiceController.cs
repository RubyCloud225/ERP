using Microsoft.AspNetCore.Mvc;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPurchaseInvoice(IFormFile document, int userId)
        {
            await _purchaseInvoiceService.UploadPurchaseInvoiceAsync(document, userId);
            return Ok(document);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeletePurchaseInvoice(ApplicationDbContext.PurchaseInvoice deletedInvoice, int userId)
        {
            await _purchaseInvoiceService.DeletePurchaseInvoiceAsync(deletedInvoice, userId);
            return Ok();
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPurchaseInvoice(ApplicationDbContext.PurchaseInvoice purchaseInvoiceId, int userId)
        {
            var result = await _purchaseInvoiceService.GetPurchaseInvoiceAsync(purchaseInvoiceId, userId);
            return Ok(result);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdatePurchaseInvoice(ApplicationDbContext.PurchaseInvoice updatedInvoice, int userId)
        {
            await _purchaseInvoiceService.AmendPurchaseInvoiceAsync(updatedInvoice, userId);
            return Ok(updatedInvoice);
        }
    }
}