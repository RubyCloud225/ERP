using ERP.Model;
using Microsoft.EntityFrameworkCore;
using static ERP.Model.ApplicationDbContext;

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
        public async Task<string> ProcessDocumentAsync(DocumentRecord documentRecord)
        {
            // Logic for processing outstanding task
            return await _llmService.GenerateDocumentCatagoryPromptAsync(documentRecord);
        }
        public async Task<string> GeneratePromptFromSalesInvoiceAsync(SalesInvoice salesInvoice)
        {
            // Logic for processing outstanding task
            return await _llmService.CreateSalesInvoiceAsync(salesInvoice);
        }
        public async Task<string> GeneratePromptFromPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice)
        {
            // Logic for processing outstanding task
            return await _llmService.GeneratePromptFromPurchaseInvoiceAsync(purchaseInvoice);
        }
        public async Task<string> BankStatementProcessorAsync(BankStatement bankStatement, BankAccount bankAccount, BankReceipt bankReciept, BankPayment bankPayment)
        {
            // Logic for processing outstanding task
            return await _llmService.GeneratePromptFromBankStatementAsync(bankAccount, bankReciept, bankPayment, bankStatement);
        }
        public async Task<string> GeneratePrompttoCreateBankNominalAsync(BankAccount bankAccount, BankStatement bankStatement)
        {
            // Logic for processing outstanding task
            return await _llmService.GeneratePrompttoCreateBankNominalAsync(bankAccount, bankStatement);
        }
    }

    public interface IDocumentProcessor
    {
        Task<string> ProcessDocumentAsync(DocumentRecord documentRecord);
        Task<string> GeneratePromptFromSalesInvoiceAsync(SalesInvoice salesInvoice);
        Task<string> GeneratePromptFromPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice);
        Task<string> BankStatementProcessorAsync(BankStatement bankStatement, BankAccount bankAccount, BankReceipt bankReciept, BankPayment bankPayment);
        Task<string> GeneratePrompttoCreateBankNominalAsync(BankAccount bankAccount, BankStatement bankStatement);
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