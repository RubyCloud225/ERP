using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public interface ILlmService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> GenerateDocumentCatagoryPromptAsync(string blobName, string documentContent);
        Task<string> GeneratePromptFromSalesInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount);
        Task<string> GeneratePromptFromPurchaseInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount, decimal purchaseTax, decimal netAmount);
        Task<string> GeneratePromptFromBankStatementAsync(string blobName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber);
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
        public async Task<string> GenerateDocumentCatagoryPromptAsync(string blobName, string documentContent)
        {
            string prompt = $"Analyze the following document content and categorize it:\n\n" +
                            $"Document Name: {blobName}\n\n" +
                            $"{documentContent}" +
                            $"Categorize the document into one of the following categories: Sales Invoice, Purchase Invoice, Bank Statement, Email, Letter, or Miscellaneous.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromSalesInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string customerName, string customerAddress, decimal totalAmount, decimal salesTax, decimal netAmount)
        {
            // prompt for the llm
            string prompt = $"You are tasked with creating a sales invoice. Please use the following details: \n" +
                            $"Document Name: {blobName}\n" +
                            $"Invoice Date: {invoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"Invoice Number: {invoiceNumber}\n" +
                            $"Customer Name: {customerName}\n" +
                            $"Customer Address: {customerAddress}\n" +
                            $"Total Amount: {totalAmount}\n" +
                            $"Sales Tax: {salesTax}\n" +
                            $"Net Amount: {netAmount}\n" +
                            $"Please generate a sales invoice based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromPurchaseInvoiceAsync(string blobName, DateTime invoiceDate, string invoiceNumber, string supplierName, string supplierAddress, decimal totalAmount, decimal purchaseTax, decimal netAmount)
        {
            string prompt = $"You are tasked with reading {blobName} classified as a PurchaseInvoice" +
            $"Please use the following details: \n" +
            $"Document Name: {blobName}\n" +
            $"Invoice Date: {invoiceDate.ToString("yyyy-MM-dd")}\n" +
            $"Invoice Number: {invoiceNumber}\n" +
            $"Supplier Name: {supplierName}\n" +
            $"Supplier Address: {supplierAddress}\n" +
            $"Total Amount: {totalAmount}\n" +
            $"Purchase Tax: {purchaseTax}\n" +
            $"Net Amount: {netAmount}\n" +
            $"Please post a purchase invoice to the appropriate nominal and expense account based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromBankStatementAsync(string blobName, DateTime Date, string Details, decimal Amount, decimal Balance, string accountNumber)
        {
            string prompt = $"You are tasked with reading {blobName} classified as a BankStatement" +
            $"Please use the following details: \n" +
            $"Document Name: {blobName}\n" +
            $"Date: {Date.ToString("yyyy-MM-dd")}\n" +
            $"Details: {Details}\n" +
            $"Amount: {Amount}\n" +
            $"Balance: {Balance}\n" +
            $"Account Number: {accountNumber}\n" +
            $"Please post and reconcile the bank statement to the general ledger based on the above details.";
            string response = await GenerateResponseAsync(prompt);
            return response.Trim();
        }
        public async Task<string> GeneratePromptFromEmailAsync

    }
}
