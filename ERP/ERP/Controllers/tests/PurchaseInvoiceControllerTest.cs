using ERP.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace ERP.Service.Tests
{
    public class PurchaseInvoiceControllerTest : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public PurchaseInvoiceControllerTest(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task UploadPurchaseInvoice_ShouldReturnOk()
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            content.Add(fileContent, "document", "test.pdf");
            content.Add(new StringContent("1"), "userId");

            var response = await _client.PostAsync("api/PurchaseInvoice/upload", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeletePurchaseInvoice_ShouldReturnOk()
        {
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                BlobName = "test_blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Test Supplier",
                Description = "Test Description",
                SupplierAddress = "Test Address"
            };
            var json = JsonSerializer.Serialize(purchaseInvoice);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/PurchaseInvoice/Delete?userId=1")
            {
                Content = content
            };
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllPurchaseInvoice_ShouldReturnOk()
        {
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                BlobName = "test_blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Test Supplier",
                Description = "Test Description",
                SupplierAddress = "Test Address"
            };
            var response = await _client.GetAsync($"api/PurchaseInvoice/GetAll?purchaseInvoiceId.Id={purchaseInvoice.Id}&userId={purchaseInvoice.UserId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePurchaseInvoice_ShouldReturnOk()
        {
            var updatedInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                BlobName = "test_blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Test Supplier",
                Description = "Test Description",
                SupplierAddress = "Test Address"
            };
            var content = JsonContent.Create(updatedInvoice);
            var response = await _client.PostAsync("api/PurchaseInvoice/update?userId=1", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

