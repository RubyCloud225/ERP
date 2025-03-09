using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ERP.Service
{
    public class DocumentService
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
}