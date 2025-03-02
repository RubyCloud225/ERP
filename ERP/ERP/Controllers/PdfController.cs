using ERP.Attributes;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly PdfService _pdfService;
        public PdfController(PdfService pdfService)
        {
            _pdfService = pdfService;
        }
        //import PDF Document fron front end
        [HttpPost("import")]
        [ERP(".pdf", 5 * 1024 * 1024)] // 5mb limit (may want to increase this)
        public async Task<IActionResult> ImportPdf(IFormFile document, string inputParameter, string documentType)
        {
            if (document == null || document.Length == 0)
            {
                return BadRequest("No file selected");
            }
            if (document.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size is too large");
            }
            // Save Pdf to the cloud
            using (var stream = new MemoryStream())
            {
                await document.CopyToAsync(stream);
                stream.Position = 0;
                // save to cloud and get the url
                var fileUrl = await _pdfService.SavePdfToCloudAsync(stream, document.FileName);
                // Process the PDF with an external API
                var apiResponse = await _pdfService.ProcessPdfWithApiAsync(fileUrl, inputParameter);
                // Save Documents metadata 
                var amendments = "No Amendments";
                var savedDocument = await _pdfService.SaveDocumentMetadataAsync(document.FileName, fileUrl, amendments, documentType);
               
                return Ok(new {FileUrl = fileUrl, ApiResponse = apiResponse, DocumentId = savedDocument.Id  });
                
            }
        }
    }
}