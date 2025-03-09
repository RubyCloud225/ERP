using Microsoft.AspNetCore.Mvc;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class PurchaseInvoiceController : ControllerBase
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILlmService _llmService;

    public PurchaseInvoiceController(IDocumentProcessor documentProcessor, ApplicationDbContext dbContext, ILlmService llmService)
    {
        _documentProcessor = documentProcessor;
        _dbContext = dbContext;
        _llmService = llmService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadPurchaseInvoice(IFormFile document)
    {
        if (document == null || document.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = document.OpenReadStream())
        {
            // Here you would extract the necessary details from the PDF
            // For demonstration, let's assume we have the following extracted data:
            string fileName = "fileName";
            DateTime invoiceDate = DateTime.Now;
            string invoiceNumber = "invoiceNumber";
            string supplierName = "supplierName";
            string supplierAddress = "supplierAddress";
            decimal totalAmount = 0;

            // Step 1: Generate the prompt for the LLM
            string prompt = await _documentProcessor.GeneratePromptFromPurchaseInvoiceAsync(fileName, invoiceDate, invoiceNumber, supplierName, supplierAddress, totalAmount);

            // Step 2: Call the LLM service to get the response
            string llmResponse = await _llmService.GenerateResponseAsync(prompt);

            // Step 3: Parse the LLM response to extract nominal and expense accounts
            // For demonstration, let's assume the LLM response is structured as follows:
            // "Nominal Account: Purchases, Expense Account: Accounts Payable"
            var accounts = ParseLlmResponse(llmResponse);

            // Step 4: Create the invoice and accounting entries
            var purchaseinvoice = new ApplicationDbContext.PurchaseInvoice
            {
                PurchaseInvoiceNumber = invoiceNumber,
                Supplier = supplierName, // This should be extracted from the LLM response
                Amount = totalAmount,
                PurchaseInvoiceDate = invoiceDate,
                Description = "Purchase Invoice from " + supplierName + invoiceNumber,
                SupplierAddress = supplierAddress,
                DocumentType = "Purchase Invoice",
                Response = llmResponse,
                NominalAccount = accounts.NominalAccount,
                ExpenseAccount = accounts.ExpenseAccount
            };

            // Save the invoice to the database
            _dbContext.PurchaseInvoices.Add(purchaseinvoice);
            await _dbContext.SaveChangesAsync();

            // Create accounting entries for double-entry bookkeeping
            var debitEntry = new ApplicationDbContext.AccountingEntry
            {
                PurchaseInvoiceId = purchaseinvoice.Id,
                Account = accounts.NominalAccount,
                Credit = purchaseinvoice.Amount,
                Debit = 0,
                EntryDate = DateTime.UtcNow
            };

            var creditEntry = new ApplicationDbContext.AccountingEntry
            {
                PurchaseInvoiceId = purchaseinvoice.Id,
                Account = accounts.ExpenseAccount,
                Debit = purchaseinvoice.Amount,
                Credit = 0,
                EntryDate = DateTime.UtcNow
            };

            // Add accounting entries to the database
            _dbContext.AccountingEntries.Add(debitEntry);
            _dbContext.AccountingEntries.Add(creditEntry);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Purchase invoice processed successfully.", InvoiceId = purchaseinvoice.Id });
        }
    }

    private (string NominalAccount, string ExpenseAccount) ParseLlmResponse(string llmResponse)
    {
        // This is a simple parsing logic; you may need to adjust it based on the actual response format
        // For example, if the response is "Nominal Account: Purchases, Expense Account: Accounts Payable"
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
}