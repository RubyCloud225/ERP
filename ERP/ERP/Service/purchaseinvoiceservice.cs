using ERP.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static ERP.Model.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;        
        private readonly IDocumentProcessor _documentProcessor;
        private readonly ILlmService _llmService;

        public PurchaseInvoiceService(ApplicationDbContext dbContext, IDocumentProcessor documentProcessor, ILlmService llmService)
        {
            _dbContext = dbContext;
            _documentProcessor = documentProcessor;
            _llmService = llmService;
        }

        public async Task UploadPurchaseInvoiceAsync(IFormFile document, int userId)
        {
            using (var stream = document.OpenReadStream())
            {
                string fileName = "fileName";
                DateTime invoiceDate = DateTime.Now;
                string invoiceNumber = "invoiceNumber";
                string supplierName = "supplierName";
                string supplierAddress = "supplierAddress";
                decimal taxAmount = 0;
                decimal netAmount = 0;
                decimal grossAmount = 0;

                //step 1: generate prompt for the llm
                var purchaseInvoiceInstance = new PurchaseInvoice
                {
                    PurchaseInvoiceNumber = invoiceNumber,
                    Supplier = supplierName,
                    NetAmount = netAmount,
                    TaxAmount = taxAmount,
                    GrossAmount = grossAmount,
                    PurchaseInvoiceDate = invoiceDate,
                    Description = "Purchase Invoice from " + supplierName + invoiceNumber,
                    SupplierAddress = supplierAddress,
                    BlobName = fileName,
                    UserId = userId
                };
                string prompt = await _documentProcessor.GeneratePromptFromPurchaseInvoiceAsync(purchaseInvoiceInstance);
                // step 2: process the document using the llm
                string llmResponse = await _llmService.GenerateResponseAsync(prompt);
                // step 3: process the response from the llm
                var accounts = ParseLlmResponse(llmResponse);
                // step 4: save the data to the database
                var purchaseinvoice = purchaseInvoiceInstance;
                _dbContext.PurchaseInvoices.Add(purchaseinvoice);
                await _dbContext.SaveChangesAsync();
                
                // Create accounting entries for double-entry bookkeeping
                var debitEntry = new AccountingEntry
                {
                    Account = accounts.NominalAccount,
                    Credit = 0,
                    Debit = purchaseinvoice.NetAmount,
                    EntryDate = DateTime.UtcNow
                };

                var debitEntry2 = new AccountingEntry
                {
                    Account = accounts.NominalAccount,
                    Credit = 0,
                    Debit = purchaseinvoice.TaxAmount,
                    EntryDate = DateTime.UtcNow
                };

                var creditEntry = new AccountingEntry
                {
                    Account = accounts.ExpenseAccount,
                    Debit = purchaseinvoice.GrossAmount,
                    Credit = 0,
                    EntryDate = DateTime.UtcNow
                };

                // Add accounting entries to the database
                _dbContext.AccountingEntries.Add(debitEntry);
                _dbContext.AccountingEntries.Add(debitEntry2);
                _dbContext.AccountingEntries.Add(creditEntry);
                await _dbContext.SaveChangesAsync();
            }
        }

        private (string NominalAccount, string ExpenseAccount) ParseLlmResponse(string llmResponse)
        {
            // This is a simple parsing logic; you may need to adjust it based on the actual response format
            // For example, if the response is "Nominal Account: Purchases, Expense Account: Accounts Payable"
            if (string.IsNullOrEmpty(llmResponse))
            {
                return ("", "");
            }
            
            var lines = llmResponse.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string nominalAccount = string.Empty;
            string expenseAccount = string.Empty;

            foreach (var line in lines)
            {
                if (line.Contains(" Nominal Account:"))
                {
                    nominalAccount = line.Replace("Nominal Account:", "").Trim();
                }
                else if (line.Contains("Expense Account:"))
                {
                    expenseAccount = line.Replace("Expense Account:", "").Trim();
                }
            }

            return (nominalAccount, expenseAccount);
        }

        public async Task<bool> DeletePurchaseInvoiceAsync(int purchaseInvoiceId)
        {
            var purchaseInvoice = await _dbContext.PurchaseInvoices.FindAsync(purchaseInvoiceId);
            if (purchaseInvoice == null)
            {
                return false;
            }
            _dbContext.PurchaseInvoices.Remove(purchaseInvoice);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AmendPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice updatedInvoice, int userId)
        {
            if (updatedInvoice == null)
            {
                Console.WriteLine("Updated invoice cannot null");
                return false;
            }

            // Find the existing invoice by ID and UserId
            var existingInvoice = await _dbContext.PurchaseInvoices.FirstOrDefaultAsync(pi => pi.Id == updatedInvoice.Id && pi.UserId == userId);
            if (existingInvoice == null)
            {
                Console.WriteLine("Invoice does not exist in the database for the specified user.");
                return false;
            }

            // Update properties
            existingInvoice.PurchaseInvoiceNumber = updatedInvoice.PurchaseInvoiceNumber;
            existingInvoice.Supplier = updatedInvoice.Supplier;
            existingInvoice.NetAmount = updatedInvoice.NetAmount;
            existingInvoice.GrossAmount = updatedInvoice.GrossAmount;
            existingInvoice.TaxAmount = updatedInvoice.TaxAmount; // Ensure this is included
            existingInvoice.Description = updatedInvoice.Description;
            existingInvoice.SupplierAddress = updatedInvoice.SupplierAddress;
            existingInvoice.PurchaseInvoiceDate = updatedInvoice.PurchaseInvoiceDate;
            // Save changes asynchronously
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Invoice updated successfully.");
            return true;
        }

        public async Task<ApplicationDbContext.PurchaseInvoice?> GetPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice purchaseInvoiceId)
        {
            return await _dbContext.PurchaseInvoices.FindAsync(purchaseInvoiceId);
        }

        public async Task<ApplicationDbContext.PurchaseInvoice?> GetPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice purchaseInvoiceId, int userId)
        {
            return await _dbContext.PurchaseInvoices.FirstOrDefaultAsync(pi => pi.Id == purchaseInvoiceId.Id && pi.UserId == userId);
        }

        public async Task<bool> DeletePurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice deletedInvoice)
        {
            if (deletedInvoice == null)
            {
                Console.WriteLine("Invoice does not exist in the database.");
                return false;
            }
            _dbContext.PurchaseInvoices.Remove(deletedInvoice);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Invoice deleted successfully.");
            return true;
        }

        public async Task<bool> DeletePurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice deletedInvoice, int userId)
        {
            if (deletedInvoice == null || deletedInvoice.UserId != userId)
            {
                Console.WriteLine("Invoice does not exist in the database for the specified user.");
                return false;
            }
            _dbContext.PurchaseInvoices.Remove(deletedInvoice);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Invoice deleted successfully.");
            return true;
        }
    }

    public interface IPurchaseInvoiceService
    {
        Task UploadPurchaseInvoiceAsync(IFormFile document, int userId);
        Task<bool> AmendPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice updatedInvoice, int userId);
        Task<ApplicationDbContext.PurchaseInvoice?> GetPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice purchaseInvoiceId, int userId);
        Task<bool> DeletePurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice deletedInvoice, int userId);
    }
}