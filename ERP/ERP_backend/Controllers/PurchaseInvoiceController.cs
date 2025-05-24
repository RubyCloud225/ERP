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
        public async Task<IActionResult> UploadPurchaseInvoice(IFormFile document)
        {
            await _purchaseInvoiceService.UploadPurchaseInvoiceAsync(document);
            return Ok(document);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeletePurchaseInvoice(ApplicationDbContext.PurchaseInvoice deletedInvoice)
        {
            await _purchaseInvoiceService.DeletePurchaseInvoiceAsync(deletedInvoice);
            return Ok();
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPurchaseInvoice(ApplicationDbContext.PurchaseInvoice purchaseInvoiceId)
        {
            await _purchaseInvoiceService.GetPurchaseInvoiceAsync(purchaseInvoiceId);
            return Ok();
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdatePurchaseInvoice(ApplicationDbContext.PurchaseInvoice updatedInvoice)
        {
            await _purchaseInvoiceService.AmendPurchaseInvoiceAsync(updatedInvoice);
            return Ok(updatedInvoice);
        }
    }
}