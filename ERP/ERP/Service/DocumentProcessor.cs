using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentService _documentService;
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _applicationDbContext;

        public DocumentProcessor(IDocumentService documentService, ILlmService llmService, ApplicationDbContext applicationDbContext)
        {
            _documentService = documentService;
            _llmService = llmService;
            _applicationDbContext = applicationDbContext;
        }
        public async Task<string> ProcessDocumentAsync(string blobName)
        {
            // read the document content
            var (header, body, footer) = await _documentService.ReadPdfContentAsync(blobName);
            string documentContent = string.Join(Environment.NewLine, header) + Environment.NewLine + 
                                    string.Join(Environment.NewLine, body) + Environment.NewLine + 
                                    string.Join(Environment.NewLine, footer);
            string documentType = await _llmService.GenerateDocumentCatagoryPromptAsync(blobName, documentContent);
            await SaveDocumentRecordAsync(blobName, documentType);
            // Create a prompt with the extracted information using the LLM service
            return await _llmService.GenerateDocumentCatagoryPromptAsync(blobName, documentType);
        }
        private async Task SaveDocumentRecordAsync(string blobName, string documentType)
        {
            var documentRecord = new ApplicationDbContext.DocumentRecord
            {
                BlobName = blobName,
                CreatedAt = DateTime.UtcNow,
                DocumentType = documentType
            };
            _applicationDbContext.DocumentRecords.Add(documentRecord);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<string> GeneratePromptFromSalesInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount)
        {
            // Logic for processing outstanding task
            return await _llmService.GeneratePromptFromSalesInvoiceAsync(blobName, invoiceDate, invoiceNumber, customerName, customerAddress, totalAmount, salesTax, netAmount);
        }
        public async Task<string> GeneratePromptFromPurchaseInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount, decimal purchaseTax, decimal netAmount)
        {
            // Logic for processing outstanding task
            return await _llmService.GeneratePromptFromPurchaseInvoiceAsync(blobName, invoiceDate, invoiceNumber, supplierName, supplierAddress, totalAmount, purchaseTax, netAmount);
        }
        public async Task<string> BankStatementProcessorAsync(string blobName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber)
        {
            return await _llmService.GeneratePromptFromBankStatementAsync(blobName, Date, Details, Amount, Balance, accountNumber);
        }
        public async Task<string> EmailDocumentProcessorAsync(string blobName, string message, string subject, string senderEmail, string recipientEmail)
        {
            return await _llmService.GeneratePromptFromEmailAsync(blobName, message, subject, senderEmail, recipientEmail);
        }
        public async Task<string> ProcessLetterAsync(string blobName, string message, string senderaddress, string recipientaddress, string subject)
        {
           return await _llmService.GenerateLetterPromptAsync(blobName, message, senderaddress, recipientaddress, subject);
        }
    }

    public interface IDocumentProcessor
    {
        Task<string> ProcessDocumentAsync(string blobName);
        Task<string> GeneratePromptFromPurchaseInvoiceAsync(string fileName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount, decimal purchaseTax, decimal netAmount);
        Task<string> GeneratePromptFromSalesInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount);
        Task<string> BankStatementProcessorAsync(string fileName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber);
        Task<string> EmailDocumentProcessorAsync(string fileName, string message, string subject, string senderEmail, string recipientEmail);
        Task<string> ProcessLetterAsync(string fileName, string message, string senderaddress, string recipientaddress, string subject);
    }

    public class DocumentProcessorFactory
    {
        private readonly IDocumentService _documentService;
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _dbContext;
        public DocumentProcessorFactory(IDocumentService documentService, ILlmService llmService, ApplicationDbContext dbContext)
        {
            _documentService = documentService;
            _llmService = llmService;
            _dbContext = dbContext;
        }
        public IDocumentProcessor GetDocumentProcessor(string documentType)
        {
            // Create a factory for the document processor
            return documentType switch
            {
                "purchase_invoice" => new DocumentProcessor(_documentService, _llmService, _dbContext),
                "sales_invoice" => new DocumentProcessor(_documentService, _llmService, _dbContext),
                "bank_statement" => new DocumentProcessor(_documentService, _llmService, _dbContext),
                "email_document" => new DocumentProcessor(_documentService, _llmService, _dbContext),
                "letter" => new DocumentProcessor(_documentService, _llmService, _dbContext),
                _ => throw new ArgumentException("Invalid document type", nameof(documentType)),
            };
        }
    }
}