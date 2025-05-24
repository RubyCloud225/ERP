using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using ERP.Model;
using Microsoft.Identity.Client;

namespace ERP.Service
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private ApplicationDbContext _dbContext;
        private readonly ILlmService _llmService;
        private readonly IDocumentProcessor _documentProcessor;
        public SalesInvoiceService(ApplicationDbContext dbContext, ILlmService llmService, IDocumentProcessor documentProcessor)
        {
            _dbContext = dbContext;
            _documentProcessor = documentProcessor;
            _llmService = llmService;
        }
        // use LLM to create a sales invoice
        public async Task GenerateSalesInvoiceAsync(int Id, string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new Exception("BlobName cannot be empty");
            }
            if (string.IsNullOrEmpty(invoiceNumber))
            {
                throw new Exception("InvoiceNumber cannot be empty");
            }
            if (string.IsNullOrEmpty(customerName))
            {
                throw new Exception("CustomerName cannot be empty");
            }

            // Check if the sales invoice already exists to avoid tracking issues
            var existingInvoice = await _dbContext.SalesInvoices.FindAsync(Id);
            if (existingInvoice != null)
            {
                // Update existing invoice properties
                existingInvoice.BlobName = blobName;
                existingInvoice.InvoiceDate = invoiceDate;
                existingInvoice.InvoiceNumber = invoiceNumber;
                existingInvoice.CustomerName = customerName;
                existingInvoice.CustomerAddress = customerAddress;
                existingInvoice.TotalAmount = totalAmount;
                existingInvoice.SalesTax = salesTax;
                existingInvoice.NetAmount = netAmount;
                _dbContext.SalesInvoices.Update(existingInvoice);
            }
            else
            {
                var salesInvoice = new ApplicationDbContext.SalesInvoice
                {
                    Id = Id,
                    UserId = await _dbContext.Users.FindAsync(1), // assuming a default user ID for now, you can modify this as needed
                    BlobName = blobName,
                    InvoiceDate = invoiceDate,
                    InvoiceNumber = invoiceNumber,
                    CustomerName = customerName,
                    CustomerAddress = customerAddress,
                    TotalAmount = totalAmount,
                    SalesTax = salesTax,
                    NetAmount = netAmount
                };
                _dbContext.SalesInvoices.Add(salesInvoice);
            }

            var salesInvoiceForPrompt = existingInvoice ?? _dbContext.SalesInvoices.Local.FirstOrDefault(i => i.Id == Id);
            if (salesInvoiceForPrompt == null)
            {
                throw new Exception($"Sales Invoice with Id {Id} could not be found or created.");
            }
            string prompt = await _documentProcessor.GeneratePromptFromSalesInvoiceAsync(salesInvoiceForPrompt);
            string llmResponse = await _llmService.GenerateResponseAsync(prompt);
            var accounts = ParseLlmResponse(llmResponse);
            await _dbContext.SaveChangesAsync();

            var creditEntry = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.NominalAccount,
                Credit = (existingInvoice ?? _dbContext.SalesInvoices.Local.FirstOrDefault(i => i.Id == Id))?.NetAmount ?? 0,
                Debit = 0,
                EntryDate = DateTime.UtcNow
            };
            var creditEntry2 = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.ExpenseAccount,
                Credit = (existingInvoice ?? _dbContext.SalesInvoices.Local.FirstOrDefault(i => i.Id == Id))?.SalesTax ?? 0,
                Debit = 0,
                EntryDate = DateTime.UtcNow
            };
            var debitEntry = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.ExpenseAccount,
                Credit = 0,
                Debit = (existingInvoice ?? _dbContext.SalesInvoices.Local.FirstOrDefault(i => i.Id == Id))?.TotalAmount ?? 0,
                EntryDate = DateTime.UtcNow
            };
            _dbContext.AccountingEntries.Add(creditEntry);
            _dbContext.AccountingEntries.Add(creditEntry2);
            _dbContext.AccountingEntries.Add(debitEntry);
            await _dbContext.SaveChangesAsync();
        }
        private (string NominalAccount, string ExpenseAccount) ParseLlmResponse(string llmResponse)
        {
            // This is a simple parsing logic; you may need to adjust it based on the actual response format
            // For example, if the response is "Nominal Account: Purchases, Expense Account: Accounts Payable"
            if (string.IsNullOrEmpty(llmResponse))
            {
                return ("", "");
            }
            
            var lines = llmResponse.Split([','], StringSplitOptions.RemoveEmptyEntries);
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
        public async Task UpdateSalesInvoiceAsync(int Id, string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount)
        {
            var salesInvoice = await _dbContext.SalesInvoices.FindAsync(Id);
            if (salesInvoice == null)
            {
                throw new Exception($"Sales Invoice with Id {Id} not found");
            }
            // update only the InvoiceNumber field to make it efficient and easier for the controller
            salesInvoice.InvoiceNumber = invoiceNumber;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> DeleteSalesInvoiceAsync(int Id)
        {
            var salesInvoice = await _dbContext.SalesInvoices.FindAsync(Id);
            if (salesInvoice == null)
            {
                throw new Exception($"Sales Invoice with Id {Id} not found");
            }
            _dbContext.SalesInvoices.Remove(salesInvoice);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
    public interface ISalesInvoiceService
    {
        Task GenerateSalesInvoiceAsync(int Id, string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount);
        Task UpdateSalesInvoiceAsync(int Id, string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount);
        Task<bool> DeleteSalesInvoiceAsync(int Id);
    }
}
