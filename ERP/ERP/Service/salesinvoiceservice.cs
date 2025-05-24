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
            // use LLM to create a sales invoice
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = Id,
                BlobName = blobName,
                InvoiceDate = invoiceDate,
                InvoiceNumber = invoiceNumber,
                CustomerName = customerName,
                CustomerAddress = customerAddress,
                TotalAmount = totalAmount,
                SalesTax = salesTax,
                NetAmount = netAmount
            };
            string prompt = await _documentProcessor.GeneratePromptFromSalesInvoiceAsync(salesInvoice);
            string llmResponse = await _llmService.GenerateResponseAsync(prompt);
            var accounts = ParseLlmResponse(llmResponse);
            _dbContext.SalesInvoices.Add(salesInvoice);
            await _dbContext.SaveChangesAsync();

            var creditEntry = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.NominalAccount,
                Credit = salesInvoice.NetAmount,
                Debit = 0,
                EntryDate = DateTime.UtcNow
            };
            var creditEntry2 = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.ExpenseAccount,
                Credit = salesInvoice.SalesTax,
                Debit = 0,
                EntryDate = DateTime.UtcNow
            };
            var debitEntry = new ApplicationDbContext.AccountingEntry
            {
                Account = accounts.ExpenseAccount,
                Credit = 0,
                Debit = salesInvoice.TotalAmount,
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
