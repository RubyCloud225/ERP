using System.Reflection.Metadata;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ERP.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly CloudStorageService _cloudStorageService;
        public DocumentService(CloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService ?? throw new ArgumentNullException(nameof(cloudStorageService));
        }
        public async Task<Stream> GetDocumentByBlobNameAsync(string blobName)
        {
            var containerClient = _cloudStorageService.GetBlobContainerClient();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync())
            {
                MemoryStream memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            else
            {
                throw new FileNotFoundException($"Blob {blobName} not found.");
            }
        }
        public async Task<(List<string> Header, List<string> Body, List<string> Footer)> ReadPdfContentAsync(string blobName)
        {
            using (Stream pdfStream = await GetDocumentByBlobNameAsync(blobName))
            {
                if (pdfStream == null)
                {
                    throw new FileNotFoundException($"Blob {blobName} not found.");
                }
                byte[] headerBytes = new byte[5];
                int bytesRead = await pdfStream.ReadAsync(headerBytes, 0, headerBytes.Length);
                
                if (bytesRead < headerBytes.Length || !Encoding.ASCII.GetString(headerBytes).StartsWith("%PDF-"))
                {
                    throw new InvalidDataException("The File is not Valid");
                }

                pdfStream.Position = 0;

                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(pdfStream))
                {
                    string? line;
                    if ((line = await reader.ReadLineAsync()) != null)
                    {
                        lines.Add(line);
                    }
                }
                if (lines.Count == 0)
                {
                    throw new InvalidDataException("The file is empty.");
                }
                return ExtractPdfContent(lines);
            }
        }
        public async Task<string> CategorizeDocumentAsync(string content)
        {
            // Simulate LLM Categorization logic
            await Task.Delay(100); // asyncwork
            return "Category"; // example
        }
        public async Task<string> ExtractRelevantInfoFromDocumentAsync(string content, string documentType)
        {
            // Simulate LLM Extraction logic
            await Task.Delay(100); // asyncwork
            return "Relevant Info"; // example
        }

        private static (List<string> Header, List<string> Body, List<string> Footer) ExtractPdfContent(List<string> lines)
        {
            const int HeaderLinesCount = 5;
            const int FooterLinesCount = 5;
            List<string> header = new List<string>();
            List<string> body = new List<string>();
            List<string> footer = new List<string>();
            if (lines == null || lines.Count == 0)
            {
                return (header, body, footer);
            }
            for (int i = 0; i < Math.Min(HeaderLinesCount, lines.Count); i++)
            {
                header.Add(lines[i]);
            }
            for (int i = 0; i < Math.Min(FooterLinesCount, lines.Count); i++)
            {
                footer.Add(lines[lines.Count - 1 - i]);
            }
            for (int i = HeaderLinesCount; i < lines.Count - FooterLinesCount; i++)
            {
                body.Add(lines[i]);
            }
            return (header, body, footer);
        }

        public async Task<List<string>> GetDocumentsByTypeAsync(string documentType)
        {
            var containerClient = _cloudStorageService.GetBlobContainerClient();
            List<string> documentNames = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (Path.GetExtension(blobItem.Name).Equals($".{documentType}", StringComparison.OrdinalIgnoreCase))
                {
                    documentNames.Add(blobItem.Name);
                }
            }
            return documentNames;
        }
    }

    public interface IDocumentService
    {
        Task<string> CategorizeDocumentAsync(string content);
        Task<string> ExtractRelevantInfoFromDocumentAsync(string content, string documentType);
        Task<List<string>> GetDocumentsByTypeAsync(string documentType);
        Task<(List<string> Header, List<string> Body, List<string> Footer)> ReadPdfContentAsync(string blobName);
        Task<Stream> GetDocumentByBlobNameAsync(string blobName);

    }
}