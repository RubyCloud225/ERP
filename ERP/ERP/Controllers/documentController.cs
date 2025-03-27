using ERP.Attributes;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly DocumentProcessor _documentProcessor;
        private readonly CloudStorageService _cloudStorageService;
        public DocumentController(DocumentService documentService, DocumentProcessor documentProcessor, CloudStorageService cloudStorageService)
        {
            _documentService = documentService;
            _documentProcessor = documentProcessor;
            _cloudStorageService = cloudStorageService;
        }
        //import Document fron front end
        [HttpPost("import")]
        public async Task<IActionResult> ImportDocument(IFormFile document)
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
                // Create a blobName and save to database
                // save directly to cloud storage.
                var blobName = Guid.NewGuid().ToString();
                var filePath = $"{blobName}.pdf";
                // save to cloud and get the url
                var fileUrl = await _cloudStorageService.UploadToCloudStorageAsync(blobName, filePath);
                var Response = await _documentProcessor.ProcessDocumentAsync(blobName);
                var Type = await _documentService.CategorizeDocumentAsync(blobName);
                return Ok(new {FileUrl = fileUrl, Response, Type});
            }
        }
        //get all documents
        //Delete Documents
        //Update Documents
    }
}