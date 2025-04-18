using ERP.Model;

namespace ERP.Service
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private ApplicationDbContext _dbContext;
        private readonly ILlmService _llmService;
        public SalesInvoiceService(ApplicationDbContext dbContext, ILlmService llmService)
        {
            _dbContext = dbContext;
            _llmService = llmService;
        }
        // use LLM to create a sales invoice
        public async Task<ApplicationDbContext.SalesInvoice> CreateSalesInvoiceAsync(IFormFile document)
        {
            using (var stream = document.OpenReadStream())
            {
                string blobName = "blobName";
                // prompt top create an invoice number
                // prompt to create a invoice number
                
            }
            // use LLM to generate a unique invoice number
            var invoiceNumber = await _llmService.GeneratePromptFromSalesInvoiceAsync(blobName, invoiceDate, invoiceNumber, customerName, customerAddress, netAmount, salesTax, totalAmount);
            salesInvoice.InvoiceNumber = invoiceNumber;
            // save the sales invoice to the database
            _dbContext.SalesInvoices.Add(salesInvoice);
            await _dbContext.SaveChangesAsync();
            return salesInvoice;
        }
    }

    public interface ISalesInvoiceService
    {
        Task<ApplicationDbContext.SalesInvoice> CreateSalesInvoiceAsync(IFormFile document);
    }
}