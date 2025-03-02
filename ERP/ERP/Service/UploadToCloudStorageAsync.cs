using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs; // Add this line for Azure Blob Storage

namespace ERP.Service
{
    public class CloudStorageService
    {
        public async Task<string> UploadToCloudStorageAsync(Stream pdfStream, string fileName)
        {
            string connectionString = "<Your_Azure_Connection_String>"; // Replace with actual connection string
            string containerName = "<Your_Container_Name>"; // Replace with actual container name

            // Create a BlobServiceClient
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // Create a unique name for the blob
            string blobName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // Upload the file
            await blobClient.UploadAsync(pdfStream, true);
            return blobClient.Uri.ToString();
        }
    }
}
