using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using ERP.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ERP.Service
{
    public interface ISalesInvoiceService
    {
        Task<ApplicationDbContext.SalesInvoice> GenerateSalesInvoiceAsync(ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null);
    }
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AccountingService _accountingService;
        private readonly INominalAccountResolutionService _nominalAccountResolutionService;
        private readonly ILlmService _llmService;
        public SalesInvoiceService(ApplicationDbContext dbContext, ILlmService llmService, AccountingService accountingService, INominalAccountResolutionService nominalAccountResolutionService)
        {
            _dbContext = dbContext;
            _accountingService = accountingService;
            _nominalAccountResolutionService = nominalAccountResolutionService;
            _llmService = llmService;
        }
        // use LLM to create a sales invoice
        // Generates Sales invoice based on user input and processes it through LLM for further processing
        // saves it, and creates the corresponding accounting entries

        public async Task<ApplicationDbContext.SalesInvoice> GenerateSalesInvoiceAsync(ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null)
        {
            //validation
            if (request.LineItems == null || request.LineItems.Count == 0)
            {
                throw new ArgumentException("Line items cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new ArgumentException("Customer name cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
            {
                throw new ArgumentException("Customer address cannot be null or empty");
            }
            if (request.InvoiceDate == DateTime.MinValue)
            {
                throw new ArgumentException("Invoice date cannot be the default value");
            }
            // LLm Generation and recommended Nominal
            var generatedInvoiceData = await _llmService.GeneratePromptFromSalesInvoiceAsync(request);
            // Check for generated Invoice Number
            var existingInvoice = await _dbContext.SalesInvoices
                .FirstOrDefaultAsync(i => i.InvoiceNumber == generatedInvoiceData.InvoiceNumber);

            if (existingInvoice != null)
            {
                throw new InvalidOperationException($"An invoice with the number {generatedInvoiceData.InvoiceNumber} already exists.");
            }
            // create nominal accounts if they do not exist
            var revenueNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                generatedInvoiceData.RecommendedRevenueNominal,
                generatedInvoiceData.RecommendedRevenueNominalType);
            var receivableNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                generatedInvoiceData.RecommendedRecieivableNominal,
                generatedInvoiceData.RecommendedRecieivableNominalType);

            Guid? taxNominalId = null;
            if (!string.IsNullOrWhiteSpace(generatedInvoiceData.RecommendedTaxNominal))
            {
                // Only resolve tax nominal if it is provided
                taxNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                generatedInvoiceData.RecommendedTaxNominal,
                generatedInvoiceData.RecommendedTaxNominalType);
            }
            // Create the Sales Invoice
            var salesInvoiceLines = new List<ApplicationDbContext.SalesInvoiceLine>();
            foreach (var parsedLine in generatedInvoiceData.ParsedSalesInvoiceLines)
            {
                // Validate each line item
                if (parsedLine.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero for each line item.");
                }
                if (parsedLine.UnitPrice < 0)
                {
                    throw new ArgumentException("Unit price cannot be negative for each line item.");
                }
                if (parsedLine.TotalPrice < 0)
                {
                    throw new ArgumentException("Total price cannot be negative for each line item.");
                }
                Guid? lineNominalId = null;
                if (!string.IsNullOrWhiteSpace(parsedLine.RecommendedNominal))
                {
                    lineNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                        parsedLine.RecommendedNominal,
                        parsedLine.RecommendedNominalType);
                }
                salesInvoiceLines.Add(new ApplicationDbContext.SalesInvoiceLine
                {
                    Description = parsedLine.Description,
                    quantity = parsedLine.Quantity,
                    UnitPrice = parsedLine.UnitPrice,
                    TotalPrice = parsedLine.TotalPrice,
                    NominalAccountId = lineNominalId
                });
            }
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = Guid.NewGuid(),
                InvoiceDate = generatedInvoiceData.InvoiceDate,
                InvoiceNumber = generatedInvoiceData.InvoiceNumber,
                CustomerName = generatedInvoiceData.CustomerName,
                CustomerAddress = generatedInvoiceData.CustomerAddress,
                TotalAmount = generatedInvoiceData.TotalAmount,
                SalesTax = generatedInvoiceData.SalesTax,
                NetAmount = generatedInvoiceData.NetAmount,
                DueDate = request.DueDate,
                IsPaid = false,
                Lines = salesInvoiceLines,
                BlobName = blobName,
                UserId = userId.HasValue
                    ? Guid.NewGuid() // Replace with appropriate logic to generate or fetch a Guid
                    : throw new ArgumentNullException(nameof(userId)),
                User = await _dbContext.Users.FindAsync(userId) ?? throw new InvalidOperationException($"User with ID {userId} not found.")
            };
            _dbContext.SalesInvoices.Add(salesInvoice);
            await _dbContext.SaveChangesAsync();
            // Create Accounting Entries
            await _accountingService
                .CreateAccountingForSalesInvoiceAsync(
                    salesInvoice,
                    receivableNominalId.GetHashCode(),
                    revenueNominalId.GetHashCode(),
                    taxNominalId?.GetHashCode(),
                    null);
            salesInvoice.AccountingEntryId = salesInvoice.Id; // Adjust logic as needed
            _dbContext.SalesInvoices.Update(salesInvoice);
            await _dbContext.SaveChangesAsync();
            return salesInvoice;
        }
    }
}
