using System.Reflection.Metadata.Ecma335;
using ERP.Model;

namespace ERP.Service
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IPdfService _pdfService;
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _dbContext;

        public DocumentProcessor(IPdfService pdfService, ILlmService llmService, ApplicationDbContext dbContext)
        {
            _pdfService = pdfService;
            _llmService = llmService;
            _dbContext = dbContext;
        }

        public async Task<string> GeneratePromptFromSalesInvoiceAsync(string fileName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount)
        {
            // Logic for processing outstanding task
            string prompt = $"Create a sales invoice for {invoiceDate} with the following details: {fileName}\n" + $"Invoice Number: {invoiceNumber}\n" + $"Invoice Date: {invoiceDate}\n" + $"Customer Name: {customerName}\n" + $"Customer Address: {customerAddress}\n" + $"Total Amount: {totalAmount}" + $"Please allocate to the appropriate nominal and expense accounts for this invoice.";
            return await Task.FromResult(prompt);
        }
        public async Task<string> GeneratePromptFromPurchaseInvoiceAsync(string fileName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount)
        {
            // Logic for processing outstanding task
            string prompt = $"Create a sales invoice for {invoiceDate} with the following details: {fileName}\n" + $"Invoice Number: {invoiceNumber}\n" + $"Invoice Date: {invoiceDate}\n" + $"Customer Name: {supplierName}\n" + $"Customer Address: {supplierAddress}\n" + $"Total Amount: {totalAmount}" + $"Please allocate to the appropriate nominal and expense accounts for this invoice.";
            return await Task.FromResult(prompt);
        }
        public async Task<string> BankStatementProcessorAsync(string fileName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber)
        {
            // Process the bank statement
            string prompt = $"Please process the followng bank statement details:\n" + $"File Name: {fileName}\n" + $"Please extract the transactions per: {Date}, {Details}, {Amount}, {Balance}\n" + $"Account number: {accountNumber}" + $"Assign the transactions to the appropriate nominal and appropriate bank account"; 
            return await Task.FromResult(prompt);
        }
        public async Task<string> EmailDocumentProcessorAsync(string fileName, string message, string subject, string senderEmail, string recipientEmail)
        {
            // Process the email document
            string prompt = $"Please process the followng email document details:\n" + $"File Name: {fileName}, \n" + $"Message: {message},\n" + $"Subject: {subject}, \n" + $"Sender Email: {senderEmail},\n" + $"Recipient Email: {recipientEmail}, \n" + $"Please extract a summary of the message and provide actions to be taken";
            return await Task.FromResult(prompt);
        }
        public async Task<string> ProcessLetterAsync(string fileName, string message, string senderaddress, string recipientaddress, string subject)
        {
            // Process the document
            string prompt = $"Please process the followng letter document details:\n" + $"FileName: {fileName}\n" + $"Message: {message}\n" + $"Sender Address: {senderaddress}\n" + $"Recipient Address: {recipientaddress}\n" + $"Subject: {subject}\n" + $"Please extract a summary of the message and provide actions to be taken";
            return await Task.FromResult(prompt);
        }
    }
    public interface IDocumentProcessor
    {
        Task<string> GeneratePromptFromPurchaseInvoiceAsync(string fileName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount);
        Task<string> GeneratePromptFromSalesInvoiceAsync(string fileName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount);
        Task<string> BankStatementProcessorAsync(string fileName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber);
        Task<string> EmailDocumentProcessorAsync(string fileName, string message, string subject, string senderEmail, string recipientEmail);
        Task<string> ProcessLetterAsync(string fileName, string message, string senderaddress, string recipientaddress, string subject);
    }

    public class DocumentProcessorFactory
    {
        private readonly IPdfService _pdfService;
        private readonly ILlmService _llmService;
        private readonly ApplicationDbContext _dbContext;
        public DocumentProcessorFactory(IPdfService pdfService, ILlmService llmService, ApplicationDbContext dbContext)
        {
            _pdfService = pdfService;
            _llmService = llmService;
            _dbContext = dbContext;
        }
        public IDocumentProcessor GetDocumentProcessor(string documentType)
        {
            // Create a factory for the document processor
            return documentType switch
            {
                "purchase_invoice" => new DocumentProcessor(_pdfService, _llmService, _dbContext),
                "sales_invoice" => new DocumentProcessor(_pdfService, _llmService, _dbContext),
                "bank_statement" => new DocumentProcessor(_pdfService, _llmService, _dbContext),
                "email_document" => new DocumentProcessor(_pdfService, _llmService, _dbContext),
                "letter" => new DocumentProcessor(_pdfService, _llmService, _dbContext),
                _ => throw new ArgumentException("Invalid document type", nameof(documentType)),
            };
        }
    }
}