using ERP.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static ERP.Model.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
     public interface IPurchaseInvoiceService
     {
         // Define methods for handling purchase invoices
        Task<PurchaseInvoice> ProcessPurchaseInvoiceAsync(CreatePurchaseInvoiceDto request);
        Task<PurchaseInvoice> AmendPurchaseInvoiceAsync(Guid id, CreatePurchaseInvoiceDto request);
        Task DeletePurchaseInvoiceAsync(Guid id);
        Task<List<PurchaseInvoice>> GetAllPurchaseInvoicesAsync();
        Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id);
        Task<PurchaseInvoice?> GetPurchaseInvoiceByUserIdAsync(Guid userid);
        Task<decimal> GetPurchaseTaxReturnForQuarterAsync(int year, int quarter);
    }
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILlmService _llmService;
        private readonly IAccountingService _accountingService;
        private readonly INominalAccountResolutionService _nominalAccountResolutionService;
        public PurchaseInvoiceService(ApplicationDbContext dbContext, IDocumentService documentService, IAccountingService accountingService, ILlmService llmService, INominalAccountResolutionService nominalAccountResolutionService)
        {
            _dbContext = dbContext;
            _accountingService = accountingService;
            _llmService = llmService;
            _nominalAccountResolutionService = nominalAccountResolutionService;
            // Fix: assign documentService to a private field if needed
            // private readonly IDocumentService _documentService;
            // _documentService = documentService;
        }

        public async Task<PurchaseInvoice> ProcessPurchaseInvoiceAsync(CreatePurchaseInvoiceDto request)
        {
            if (request?.ParsedInvoice == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }
            var fullParsedInvoice = await _llmService.GeneratePromptFromPurchaseInvoiceAsync(request.ParsedInvoice);
            Guid payableNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                fullParsedInvoice.RecommendedPayableNominal, fullParsedInvoice.RecommendedPayableNominalType);
            Guid expenseNominalId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                fullParsedInvoice.RecommendedExpenseNominal, fullParsedInvoice.RecommendedExpenseNominalType);
            Guid taxNominalId = fullParsedInvoice.RecommendedTaxNominal != null
                ? await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(
                  fullParsedInvoice.RecommendedTaxNominal, fullParsedInvoice.RecommendedTaxNominalType)
                : Guid.Empty;

            var purchaseInvoice = new PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                BlobName = fullParsedInvoice.BlobName,
                PurchaseInvoiceNumber = fullParsedInvoice.InvoiceNumber,
                Supplier = fullParsedInvoice.SupplierName,
                SupplierAddress = fullParsedInvoice.Address,
                NetAmount = fullParsedInvoice.NetAmount,
                GrossAmount = fullParsedInvoice.TotalAmount,
                TaxAmount = fullParsedInvoice.TaxAmount,
                Description = "Purchase invoice processed",
                PurchaseInvoiceDate = fullParsedInvoice.InvoiceDate,
                DocumentType = "Purchase Invoice",
                Response = "Processed",
                DueDate = fullParsedInvoice.DueDate ?? DateTime.Now.AddDays(30),
                IsPaid = false,
                UserId = request.UserId,
                Lines = fullParsedInvoice.LineItems.Select(line => new PurchaseInvoiceLine
                {
                    Id = Guid.NewGuid(),
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = line.TotalAmount,
                    TaxAmount = line.TaxAmount,
                    NominalAccountId = null // Could be set if line-level nominal accounts are resolved
                }).ToList()
            };

            _dbContext.PurchaseInvoices.Add(purchaseInvoice);
            await _dbContext.SaveChangesAsync();

            // Create accounting entries for the purchase invoice
            await _accountingService.CreateAccountingForPurchaseInvoiceAsync(
                purchaseInvoice,
                payableNominalId,
                expenseNominalId,
                taxNominalId,
                request.UserId.HasValue ? (int?)request.UserId.Value.GetHashCode() : null);

            // Update purchase invoice with accounting entry id
            var accountingEntry = await _dbContext.AccountingEntries
                .FirstOrDefaultAsync(a => a.PurchaseInvoiceId == purchaseInvoice.Id);
            if (accountingEntry != null)
            {
                purchaseInvoice.AccountingEntryId = accountingEntry.Id;
                await _dbContext.SaveChangesAsync();
            }

            return purchaseInvoice;
        }
        public async Task<List<PurchaseInvoice>> GetAllPurchaseInvoicesAsync()
        {
            return await _dbContext.PurchaseInvoices
            .Include(pi => pi.Lines)
            .ToListAsync();
        }

        public async Task<PurchaseInvoice> AmendPurchaseInvoiceAsync(Guid id, CreatePurchaseInvoiceDto request)
        {
             if (request?.ParsedInvoice == null)
             {
                 throw new ArgumentNullException(nameof(request), "Request cannot be null");
             }

             var existingInvoice = await _dbContext.PurchaseInvoices
                 .Include(i => i.Lines)
                 .FirstOrDefaultAsync(i => i.Id == id);

             if (existingInvoice == null)
             {
                 throw new KeyNotFoundException($"Purchase invoice with id {id} not found");
             }

             var fullParsedInvoice = await _llmService.GeneratePromptFromPurchaseInvoiceAsync(request.ParsedInvoice);

             // Update properties
             existingInvoice.BlobName = fullParsedInvoice.BlobName;
             existingInvoice.PurchaseInvoiceNumber = fullParsedInvoice.InvoiceNumber;
             existingInvoice.Supplier = fullParsedInvoice.SupplierName;
             existingInvoice.SupplierAddress = fullParsedInvoice.Address;
             existingInvoice.NetAmount = fullParsedInvoice.NetAmount;
             existingInvoice.GrossAmount = fullParsedInvoice.TotalAmount;
             existingInvoice.TaxAmount = fullParsedInvoice.TaxAmount;
             existingInvoice.PurchaseInvoiceDate = fullParsedInvoice.InvoiceDate;
             existingInvoice.DueDate = fullParsedInvoice.DueDate ?? DateTime.Now.AddDays(30);
             existingInvoice.IsPaid = false;
             existingInvoice.UserId = request.UserId;

             // Remove existing lines
             _dbContext.PurchaseInvoiceLines.RemoveRange(existingInvoice.Lines);

             // Add new lines
             var newLines = fullParsedInvoice.LineItems.Select(line => new PurchaseInvoiceLine
             {
                 Id = Guid.NewGuid(),
                 Description = line.Description,
                 Quantity = line.Quantity,
                 UnitPrice = line.UnitPrice,
                 TotalPrice = line.TotalAmount,
                 TaxAmount = line.TaxAmount,
                 NominalAccountId = null, // Could be set if line-level nominal accounts are resolved
                 PurchaseInvoiceId = id
             }).ToList();

             existingInvoice.Lines = newLines;

             _dbContext.PurchaseInvoiceLines.AddRange(newLines);

             _dbContext.PurchaseInvoices.Update(existingInvoice);
             await _dbContext.SaveChangesAsync();

             // Removed call to non-existent accounting service method

             return existingInvoice;
         }

         public async Task DeletePurchaseInvoiceAsync(Guid id)
         {
             var existingInvoice = await _dbContext.PurchaseInvoices
                 .Include(i => i.Lines)
                 .FirstOrDefaultAsync(i => i.Id == id);

             if (existingInvoice == null)
             {
                 throw new KeyNotFoundException($"Purchase invoice with id {id} not found");
             }

             // Remove related lines first
             _dbContext.PurchaseInvoiceLines.RemoveRange(existingInvoice.Lines);

             _dbContext.PurchaseInvoices.Remove(existingInvoice);
             await _dbContext.SaveChangesAsync();

             // Removed call to non-existent accounting service method
         }
         public async Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id)
         {
             var existingInvoice = await _dbContext.PurchaseInvoices
                 .Include(i => i.Lines)
                 .FirstOrDefaultAsync(i => i.Id == id);

             if (existingInvoice == null)
             {
                 throw new KeyNotFoundException($"Purchase Invoice with id {id} not found");
             }

             return existingInvoice;
         }
         public async Task<PurchaseInvoice?> GetPurchaseInvoiceByUserIdAsync(Guid userId)
         {
             var existingInvoice = await _dbContext.PurchaseInvoices
                 .Include(i => i.Lines)
                 .FirstOrDefaultAsync(i => i.UserId == userId);

             if (existingInvoice == null)
             {
                 throw new KeyNotFoundException($"Purchase Invoice for user with id {userId} not found");
             }

             return existingInvoice;
         }
        public async Task<decimal> GetPurchaseTaxReturnForQuarterAsync(int year, int quarter)
        {
            var startDate = new DateTime(year, (quarter - 1) * 3 + 1, 1);
            var endDate = startDate.AddMonths(3);
            var totalPurchaseTax = await _dbContext.PurchaseInvoices
                .Where(i => i.PurchaseInvoiceDate >= startDate && i.PurchaseInvoiceDate < endDate)
                .SumAsync(i => (decimal?)i.TaxAmount) ?? 0m;

            return totalPurchaseTax;
        }
    }
}