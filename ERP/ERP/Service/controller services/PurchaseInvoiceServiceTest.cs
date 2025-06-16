using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using ERP.Tests;

namespace ERP.Service.Tests
{
    public class PurchaseInvoiceServiceTest
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SeedDatabaseForTests _seedDatabaseForTests = new SeedDatabaseForTests();
        private readonly Mock<IDocumentProcessor> _documentProcessorMock;
        private readonly Mock<ILlmService> _llmServiceMock;
        private readonly PurchaseInvoiceService _purchaseInvoiceService;
        private ApplicationDbContext.User? _seededUser;

        public PurchaseInvoiceServiceTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var baseConnectionString = "Host=localhost;Port=5432;Database=ERP_Test;Username=erpuser;Password=erppassword";
            var dbName = $"PurchaseInvoiceTestDb_{Guid.NewGuid()}";
            var connectionString = $"{baseConnectionString};Database={dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString)
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _seedDatabaseForTests.Seed(_dbContext);
            try
            {
                _dbContext.Database.EnsureCreated();
                _dbContext.Database.EnsureDeleted();
                _seedDatabaseForTests.SeedUser();
                _seedDatabaseForTests.SeedUsers();
                _seedDatabaseForTests.SeedPurchaseInvoices();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database setup: {ex.Message}");
            }
            _documentProcessorMock = new Mock<IDocumentProcessor>();
            _llmServiceMock = new Mock<ILlmService>();
            _purchaseInvoiceService = new PurchaseInvoiceService(_dbContext, _documentProcessorMock.Object, _llmServiceMock.Object);
        }

        [Fact]
        public async Task UploadPurchaseInvoiceAsync_ShouldAddPurchaseInvoice()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            _documentProcessorMock.Setup(dp => dp.GeneratePromptFromPurchaseInvoiceAsync(It.IsAny<ApplicationDbContext.PurchaseInvoice>()))
                .ReturnsAsync("Prompt");

            _llmServiceMock.Setup(llm => llm.GenerateResponseAsync(It.IsAny<string>()))
                .ReturnsAsync("Nominal Account: Purchases, Expense Account: Accounts Payable");

            // Act
            await _purchaseInvoiceService.UploadPurchaseInvoiceAsync(fileMock.Object, 1);

            // Assert
            var purchaseInvoices = await _dbContext.PurchaseInvoices.ToListAsync();
            Assert.Single(purchaseInvoices);
        }

        // Additional tests for AmendPurchaseInvoiceAsync, DeletePurchaseInvoiceAsync, GetPurchaseInvoiceAsync can be added similarly
    }
}
