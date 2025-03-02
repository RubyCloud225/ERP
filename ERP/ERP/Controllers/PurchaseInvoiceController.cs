using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _dbContext;

        public PurchaseInvoiceController(ILlmService llmService, ApplicationDbContext dbContext)
        {
            _llmService = llmService;
            _dbContext = dbContext;
        }
        [HttpPost("upload-purchase-invoice")]
        public async Task<IActionResult> UploadPurchaseInvoice([FromForm]IFormFile Document)
        {
            if (Document == null || Document.Length == 0)
            {
                return BadRequest("No file selected");
            }
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await Document.CopyToAsync(stream);
            }
            
        }
    }
}