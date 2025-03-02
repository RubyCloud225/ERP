using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using ERP.Model;

namespace ERP.Service
{
    public interface IPdfService
    {
        Task<string> SavePdfToCloudAsync(Stream pdfStream, string filename);
        Task<string> ProcessPdfWithApiAsync(string fileUrl, string inputParameter);
        Task<ApplicationDbContext.Document> SaveDocumentMetadataAsync(string fileName, string fileUrl, string amendments, string documentType);
    };

    public class PdfService : IPdfService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;

        public PdfService(IConfiguration configuration, HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<string> SavePdfToCloudAsync(Stream pdfStream, string fileName)
        {
            // Save pdf to cloud storage
            string cloudUrl = await new CloudStorageService().UploadToCloudStorageAsync(pdfStream, fileName);
            await Task.Delay(100);
            return cloudUrl;
        }

        public async Task<string> ProcessPdfWithApiAsync(string fileUrl, string inputParameter)
        {
            var apiUrl = _configuration["ExternalApi:Url"];
            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new InvalidOperationException("API URL is not configured.");
            }
            var response = await _httpClient.PostAsJsonAsync(apiUrl, new { fileUrl, inputParameter });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            throw new HttpRequestException($"Error calling external API: {response.StatusCode} - {response.ReasonPhrase}");
        }

        public async Task<ApplicationDbContext.Document> SaveDocumentMetadataAsync(string fileName, string fileUrl, string amendments, string documentType)
        {
            var document = new ApplicationDbContext.Document
            {
                FileName = fileName,
                FileUrl = fileUrl,
                UploadedAt = DateTime.Now,
                Amendments = amendments,
                DocumentType = documentType
            };
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }
    }
}
