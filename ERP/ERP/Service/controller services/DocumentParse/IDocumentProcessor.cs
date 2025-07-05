using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ERP.Model;

namespace ERP.Service
{
    public interface IDocumentProcessor
    {
        Task<string> CategorizeDocumentAsync(string blobName);
        Task<string> ExtractRelevantInfoFromDocumentAsync(string blobName, string documentType);
        Task<List<string>> GetDocumentsByTypeAsync(string documentType);
        Task DeleteDocumentAsync(string documentId);
        Task<Stream> DownloadDocumentAsync(string documentId);
        Task<string> GenerateResponseAsync(string prompt);
        Task<(List<string> Header, List<string> Body, List<string> Footer)> ReadPdfContentAsync(string blobName);
        Task<string> ProcessDocumentAsync(ERP.Model.ApplicationDbContext.DocumentRecord documentRecord);
    }
}
