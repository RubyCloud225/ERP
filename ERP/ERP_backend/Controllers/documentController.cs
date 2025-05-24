using System.ComponentModel.DataAnnotations;
using ERP.Attributes;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly DocumentProcessor _documentProcessor;
        private readonly CloudStorageService _cloudStorageService;
        private readonly ApplicationDbContext _dbContext;

        public DocumentController(DocumentService documentService, DocumentProcessor documentProcessor, CloudStorageService cloudStorageService, ApplicationDbContext dbContext)
        {
            _documentService = documentService;
            _documentProcessor = documentProcessor;
            _cloudStorageService = cloudStorageService;
            _dbContext = dbContext;
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
                var documentRecord = await _dbContext.DocumentRecords.FirstOrDefaultAsync(d => d.BlobName == blobName);
                if (documentRecord == null)
                {
                    return NotFound("Document record not found.");
                }
                var Response = await _documentProcessor.ProcessDocumentAsync(documentRecord);
                var Type = await _documentService.CategorizeDocumentAsync(blobName);
                return Ok(new {FileUrl = fileUrl, Response, Type});
            }
        }
        //get all documents
        [HttpGet("type/{documentType}")]
        public async Task<ActionResult<List<string>>> GetDocumentsByType(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return BadRequest("Document type is required");
            }
            List<string> documents;
            try
            {
                documents = await _documentService.GetDocumentsByTypeAsync(documentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            if (documents == null || documents.Count == 0)
            {
                return NotFound();
            }
            return Ok(documents);
        }
        //Delete Documents
        [HttpDelete("{documentId}")]
        public async Task<ActionResult> DeleteDocument(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return BadRequest("Document Id is required");
            }
            try
            {
                await _documentService.DeleteDocumentAsync(documentId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return NoContent();
        }
        [HttpGet("download/{documentId}")]
        public async Task<ActionResult> DownloadDocument(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return BadRequest("Document Id is required");
            }
            try
            {
                var document = await _documentService.DownloadDocumentAsync(documentId);
                if (document == null)
                {
                    return NotFound();
                }
                var contentType = "application/octet-stream";
                var fileName = Path.GetFileName(documentId);
                return File(document, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}