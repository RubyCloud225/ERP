using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration; // Add this line for Azure Blob Storage

namespace ERP.Service
{
    public class CloudStorageService
    {
        private readonly string _configurationString;
        private readonly string _blobContainerName;

        public CloudStorageService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration), "Configuration is required.");
            }
            _configurationString = configuration["AzureBlobStorageConnectionString"]
            ?? throw new InvalidOperationException("AzureBlobStorageConnectionString is required in the configuration.");
            _blobContainerName = configuration["AzureBlobStorageContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorageContainerName is required in the configuration.");
        }

        public async Task<string> DownloadFileFromBlobAsync(string blobName, string filePath)
        {
            var (blobServiceClient, containerClient) = CreateBlobClient();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            Console.WriteLine($"Downloading blob to {filePath} ...");
            await blobClient.DownloadToAsync(filePath);
            string decompressedFilePath = Path.ChangeExtension(filePath, null);
            using (FileStream compressedFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream decompressedFileStream = File.Create(decompressedFilePath))
            using (GZipStream decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
            {
                await decompressionStream.CopyToAsync(decompressedFileStream);
            }
            return decompressedFilePath;
        }

        public async Task<string> UploadToCloudStorageAsync(string blobName, string filePath)
        {
            var (blobServiceClient, containerClient) = CreateBlobClient();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            Console.WriteLine($"Uploading blob from {filePath} ...");
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                await blobClient.UploadAsync(fileStream, overwrite: true);
            }
            string compressedFilePath = Path.ChangeExtension(filePath, ".zip");
            using (FileStream originalFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (FileStream compressedFileStream = File.Create(compressedFilePath))
            using (GZipStream zipStream = new GZipStream(compressedFileStream, CompressionLevel.Optimal))
            {
                await originalFileStream.CopyToAsync(zipStream);
            }
            return GetDocumentType(compressedFilePath);
        }
        private string GetDocumentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "PDF",
                ".docx" => "Word Document",
                ".xlsx" => "Excel Document",
                ".pptx" => "PowerPoint Presentation",
                ".mp4" => "Video",
                ".mp3" => "Audio",
                ".jpeg" => "Image",
                ".jpg" => "Image",
                _ => "Unknown"
            };
        }
        public BlobContainerClient GetBlobContainerClient()
        {
            return new BlobContainerClient(_configurationString, _blobContainerName);
        }
        private (BlobServiceClient, BlobContainerClient) CreateBlobClient()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configurationString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            return (blobServiceClient , containerClient);
        }
    }
}
