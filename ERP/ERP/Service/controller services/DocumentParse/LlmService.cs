using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Azure.Core;
using Microsoft.AspNetCore.Razor.Language;

namespace ERP.Service
{
    public interface ILlmService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> GenerateDocumentCatagoryPromptAsync(DocumentRecord documentRecord);
        Task<parsedSalesInvoiceDto> GeneratePromptFromSalesInvoiceAsync(GenerateSalesInvoiceDto request);
        Task<ParsedPurchaseInvoiceDto> GeneratePromptFromPurchaseInvoiceAsync(ParsedPurchaseInvoiceDto request);
           
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
        // prompt for sales invoices
        public async Task<parsedSalesInvoiceDto> GeneratePromptFromSalesInvoiceAsync(GenerateSalesInvoiceDto request)
        {
            await Task.Delay(1000); // Simulate some processing delay
            decimal netAmount = request.LineItems.Sum(line => line.TotalPrice); // Calculate net amount from line items
            decimal taxRate = 0.20M; // Assuming a fixed tax rate of 20% // TODO Change to user entry
            decimal taxAmount = netAmount * taxRate;
            decimal grossAmount = netAmount + taxAmount;
            string invoiceNumber = $"INV-{DateTime.Now.Year}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}"; // Generate a unique invoice number
            string blobName = $"{invoiceNumber}.pdf"; // Placeholder for blob name, should be replaced with actual blob storage logic
                                                      // Generate a prompt for the LLM to recommend nominal and income accounts based on the sales invoice details
            // Generate a prompt for the LLM to recommend nominal and income accounts based on the sales invoice details

            string prompt = $"Recommend the following details for a Sales Invoice:\n" +
                            $"Document Name: {blobName}\n" +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"Invoice Number: {invoiceNumber}\n" +
                            $"Customer Name: {request.CustomerName}\n" +
                            $"Customer Address: {request.CustomerAddress}\n" +
                            $"Total Amount: {grossAmount:C} (in currency)\n" +
                            $"Sales Tax: {taxAmount:C} (in currency)\n" +
                            $"Net Amount: {netAmount:C} (in currency)\n" +
                            $"Please recommend a sales invoice to the appropriate nominal and income account based on the above details\n" +
                            $"and the following lines:\n" +
                            $"{string.Join("\n", request.LineItems.Select(line =>
                                $"Line Item: {line.Description}, Quantity: {line.Quantity}, Unit Price: {line.UnitPrice:C}, Total Line Amount: {line.TotalPrice:C}"))}" +
                            $"\n\nPlease provide a detailed response with the recommended nominal and income accounts. Consider the following input:\n" +
                            $"Customer: {{request.CustomerName}}, Address: {request.CustomerAddress}, " +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}, " +
                            $"Due Date: {request.DueDate.ToString("yyyy-MM-dd")}, " +
                            $"Items: {string.Join(", ", request.LineItems.Select(item => $"{item.Description} (Qty: {item.Quantity}, Price: {item.UnitPrice:C})"))}" +
                            $"If an item's description or the overall nature of the sale suggests a revenue category not represented by typical 'Sales Revenue', propose a new, specific nominal account name and its appropriate type ('Income'). Calculate all amounts accurately." +
                            $"If the customer is not a standard customer, please specify the customer's name and address in the response." +
                            $"Ensure the response includes the following fields:\n" +
                            $"Recommended Receivable Nominal: The nominal account to be used for accounts receivable: {request.RecommendedRecieivableNominal}\n" +
                            $"Recommended Income Account: The income account to be used for the sales revenue: {request.RecommendedRevenueNominal}\n" +
                            $"Recommended Tax Nominal: The tax nominal account to be used for sales tax: {request.RecommendedTaxNominal}\n" +
                            $"\n\nEnsure the response is concise and formatted for easy parsing.";
            // Return a parsedSalesInvoiceDto object as the result
            return new parsedSalesInvoiceDto
            {
                InvoiceNumber = invoiceNumber,
                BlobName = blobName,
                TotalAmount = grossAmount,
                SalesTax = taxAmount,
                NetAmount = netAmount,
                CustomerName = request.CustomerName,
                CustomerAddress = request.CustomerAddress,
                RecommendedRecieivableNominal = request.RecommendedRecieivableNominal, // Placeholder for recommended nominal account
                RecommendedRecieivableNominalType = NominalAccountType.Asset, // Placeholder for recommended nominal type
                RecommendedRevenueNominal = request.RecommendedRevenueNominal, // Placeholder for recommended income account
                RecommendedRevenueNominalType = NominalAccountType.Revenue, // Placeholder for recommended income typ
                RecommendedTaxNominal = request.RecommendedTaxNominal, // Placeholder for recommended tax nominal
                RecommendedTaxNominalType = NominalAccountType.Liability, // Placeholder for recommended tax nominal type
            };
        }
        public async Task<ParsedPurchaseInvoiceDto> GeneratePromptFromPurchaseInvoiceAsync(ParsedPurchaseInvoiceDto request)
        {
            string prompt = $"You are an expert accounting AI. I have a purchase invoice with the following details:\n\n" +
                            $"Document Name: {request.BlobName}\n" +
                            $"Invoice Name: {request.InvoiceNumber}\n" +
                            $"Invoice Date: {request.InvoiceDate.ToString("yyyy-MM-dd")}\n" +
                            $"SupplierName: {request.SupplierName}\n" +
                            $"Supplier Address: {request.Address}\n" +
                            $"Total Amount: {request.TotalAmount:C} (in currency)\n" +
                            $"Purchase Tax: {request.TaxAmount:C} (in currency)\n" +
                            $"Net Amount: {request.NetAmount:C} (in currency)\n" +
                            $"Due Date: {request.DueDate:C}\n" +
                            $"Line Items: {string.Join("\n", request.LineItems.Select(line =>
                                $"Line Item: {line.Description}, Quantity: {line.Quantity}, Unit Price: {line.UnitPrice:C}, Total Line Amount: {line.TotalAmount:C}"))}\n\n" +
                            $"Based on these details, please recommend the appropriate nominal accounts for double-entry accounting record. \n" +
                            $"Focus on the primary nominal accounts for the purchase invoice, including:\n" +
                            $"RecommendedExpenseNominal: {request.RecommendedExpenseNominal}\n" +
                            $"RecommendedPayableNominal: {request.RecommendedPayableNominal}\n" +
                            $"RecommendedTaxNominal: {request.RecommendedTaxNominal}\n" +
                            $"Please recommend a purchase invoice to the appropriate nominal and expense account based on the above details";
            await Task.Delay(1000); // Simulate some processing delay
            return new ParsedPurchaseInvoiceDto
            {
                BlobName = request.BlobName,
                InvoiceNumber = request.InvoiceNumber,
                InvoiceDate = request.InvoiceDate,
                SupplierName = request.SupplierName,
                Address = request.Address,
                TotalAmount = request.TotalAmount,
                TaxAmount = request.TaxAmount,
                NetAmount = request.NetAmount,
                DueDate = request.DueDate,
                LineItems = request.LineItems,
                RecommendedExpenseNominal = request.RecommendedExpenseNominal,
                RecommendedPayableNominal = request.RecommendedPayableNominal,
                RecommendedTaxNominal = request.RecommendedTaxNominal
            };
        }

        
        
    }
}
