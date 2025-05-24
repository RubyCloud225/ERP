using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ERP.Service
{
    public interface ILlmService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> GenerateDocumentCatagoryPromptAsync(DocumentRecord documentRecord);
        Task<string> CreateSalesInvoiceAsync(SalesInvoice salesInvoice);
        Task<string> GeneratePromptFromPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice);
        Task<string> GeneratePromptFromBankStatementAsync(BankAccount bankAccount, BankReceipt bankReciept, BankPayment bankPayment, BankStatement bankStatement);
        Task<string> GeneratePrompttoCreateBankNominalAsync(BankAccount bankAccount, BankStatement bankStatement);       
    }

    public class LlmService : ILlmService
    {
        private readonly HttpClient _httpClient;
        public LlmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt,
                max_tokens = 2048,
            };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            var response = await _httpClient.PostAsync("https://api.your-llm-service.com/generate", content); // placeholder
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApplicationDbContext.LlmResponse>(responseBody);
                if (result != null && !string.IsNullOrEmpty(result.Response))
                {
                    return result.Response;
                }
                else
                {
                    return "Error generating response";
                }
            }
            else
            {
                throw new HttpRequestException($"Error calling API: {response.StatusCode}");
            }
        }
        public async Task<string> GenerateDocumentCatagoryPromptAsync(DocumentRecord documentRecord)
        {
            string prompt = $"Analyze the following document content and categorize it:\n\n" +
                            $"Document Name: {documentRecord.BlobName}\n\n" +
                            $"{documentRecord.DocumentContent}" +
                            $"Categorize the document into one of the following categories: Sales Invoice, Purchase Invoice, Bank Statement, Email, Letter, or Miscellaneous.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> CreateSalesInvoiceAsync(SalesInvoice salesInvoice)
        {
            string prompt = $"You are tasked with creating a sales invoice. Please use the following details to generate the invoice: \n" +
                            $"- **Document Name: {salesInvoice.BlobName}\n" +
                            $"- **Invoice Date: {salesInvoice.InvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"- **Invoice Number: {salesInvoice.InvoiceNumber}\n" +
                            $"- **Customer Name: {salesInvoice.CustomerName}\n" +
                            $"- **Customer Address: {salesInvoice.CustomerAddress}\n" +
                            $"- **Total Amount: {salesInvoice.TotalAmount:C} (in currency)\n" +
                            $"- **Sales Tax: {salesInvoice.SalesTax:C} (in currency)\n" +
                            $"- **Net Amount: {salesInvoice.NetAmount:C} (in currency)\n" +
                            "Please generate a detailed sales invoice based on the above information, including the necessary sections such as:\n" +
                            "- Invoice Header\n" +
                            "- Customer Information\n" +
                            "- Itemized List of Products/Services (if applicable)\n" +
                            "- Net Amount\n" +
                            "- Sales Tax\n" +
                            "- Total Amount\n" +
                            "- Net Amount\n" +
                            "- Payment Terms\n" +
                            "- Signature Block\n" +
                            "Ensure the invoice is well-formatted, easy to read, and includes all necessary details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromPurchaseInvoiceAsync(PurchaseInvoice purchaseInvoice)
        {
            string prompt = $"You are tasked with reading {purchaseInvoice.BlobName} classified as a PurchaseInvoice" +
                            $"Please use the following details: \n" +
                            $"Document Name: {purchaseInvoice.BlobName}\n" +
                            $"Invoice Date: {purchaseInvoice.PurchaseInvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"Invoice Number: {purchaseInvoice.PurchaseInvoiceNumber}\n" +
                            $"Supplier Name: {purchaseInvoice.Supplier}\n" +
                            $"Supplier Address: {purchaseInvoice.SupplierAddress}\n" +
                            $"Total Amount: {purchaseInvoice.GrossAmount:C} (in currency)\n" +
                            $"Purchase Tax: {purchaseInvoice.TaxAmount:C} (in currency)\n" +
                            $"Net Amount: {purchaseInvoice.NetAmount:C} (in currency)\n" +
                            $"Please post a purchase invoice to the appropriate nominal and expense account based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        // prompt for bank accounts
        public async Task<string> GeneratePrompttoCreateBankNominalAsync(BankAccount bankAccount, BankStatement bankStatement)
        {
            string prompt = $"You are tasked with reading {bankStatement.BlobName}" +
                            $"Please use the following details: \n" +
                            $"Document Name: {bankStatement.BlobName}\n" +
                            $"Bank Account: {bankAccount.AccountName}\n" +
                            $"Bank Account Number: {bankAccount.AccountNumber}\n" +
                            $"Bank Name: {bankAccount.BankName}\n" +
                            $"Please create a bank nominal account based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromBankStatementAsync(BankAccount bankAccount, BankReceipt bankReciept, BankPayment bankPayment, BankStatement bankStatement)
        {
            string prompt = $"You are tasked with reading {bankStatement.BlobName} classified as a BankStatement" +
                            $"Please use the following details: \n" +
                            $"Document Name: {bankStatement.BlobName}\n" +
                            $"Date: {bankStatement.StatementDate.ToString("yyyy-MM-dd")}\n" +
                            $"Balance: {bankStatement.Balance}\n" +
                            $"Bank Account: {bankAccount.AccountName}\n" +
                            $"Bank Account Number: {bankAccount.AccountNumber}\n" +
                            $"Bank Name: {bankAccount.BankName}\n" +
                            $"Bank Receipt: {bankReciept.Payer}, {bankReciept.Amount}, {bankReciept.ReceiptDate}\n" +
                            $"Bank Payment: {bankPayment.Payee}, {bankPayment.Amount}, {bankPayment.PaymentDate}\n" +
                            $"Please post the bank statement to the appropriate bank account based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
    }
}
