using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ERP.Model;

namespace ERP.Service
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentService _documentService;
        private readonly ILlmService _llmService;

        public DocumentProcessor(IDocumentService documentService, ILlmService llmService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _llmService = llmService ?? throw new ArgumentNullException(nameof(llmService));
        }

        public async Task<string> CategorizeDocumentAsync(string blobName)
        {
            // Get document content from DocumentService
            var documentStream = await _documentService.GetDocumentByBlobNameAsync(blobName);
            using (var reader = new StreamReader(documentStream))
            {
                string content = await reader.ReadToEndAsync();
                // Use LlmService to categorize document
                return await _llmService.GenerateDocumentCatagoryPromptAsync(new ApplicationDbContext.DocumentRecord
                {
                    BlobName = blobName,
                    DocumentContent = content,
                    DocumentType = "Unknown" // Set a default or placeholder DocumentType to satisfy required member
                });
            }
        }

        public async Task<string> ExtractRelevantInfoFromDocumentAsync(string blobName, string documentType)
        {
            return await _documentService.ExtractRelevantInfoFromDocumentAsync(blobName, documentType);
        }

        public async Task<List<string>> GetDocumentsByTypeAsync(string documentType)
        {
            return await _documentService.GetDocumentsByTypeAsync(documentType);
        }

        public async Task DeleteDocumentAsync(string documentId)
        {
            await _documentService.DeleteDocumentAsync(documentId);
        }

        public async Task<Stream> DownloadDocumentAsync(string documentId)
        {
            return await _documentService.DownloadDocumentAsync(documentId);
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            return await _llmService.GenerateResponseAsync(prompt);
        }

        public async Task<(List<string> Header, List<string> Body, List<string> Footer)> ReadPdfContentAsync(string blobName)
        {
            return await _documentService.ReadPdfContentAsync(blobName);
        }

        public async Task<string> ProcessDocumentAsync(ApplicationDbContext.DocumentRecord documentRecord)
        {
            // Example processing: categorize and extract info
            string category = await CategorizeDocumentAsync(documentRecord.BlobName);
            string info = await ExtractRelevantInfoFromDocumentAsync(documentRecord.BlobName, category);
            return $"Category: {category}, Info: {info}";
        }
    }
}
