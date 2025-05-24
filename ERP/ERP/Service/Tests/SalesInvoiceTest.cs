using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ERP.Service.Tests
{
    public class SalesInvoiceServiceTests
    {
        private readonly Mock<ILlmService> _llmServiceMock;
        private readonly Mock<IDocumentProcessor> _documentServiceMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly SalesInvoiceService _salesInvoiceService;
        public SalesInvoiceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _llmServiceMock = new Mock<ILlmService>();
            _documentServiceMock = new Mock<IDocumentProcessor>();
            _salesInvoiceService = new SalesInvoiceService(_dbContext, _llmServiceMock.Object, _documentServiceMock.Object);
        }
        // Create sales invoice service with mocked dependencies
        [Fact]
        public async Task CreateSalesInvoice_with_Accounting_entries()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
                .Options;

            var mockDocumentProcessor = new Mock<IDocumentProcessor>();
            var mockLlmService = new Mock<ILlmService>();
            var dbContext = new ApplicationDbContext(options);
            var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = 1,
                BlobName = "test_blob",
                InvoiceDate = DateTime.Now,
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test Address",
                TotalAmount = 1000.00m,
                SalesTax = 100.00m,
                NetAmount = 1100.00m
            };
            mockDocumentProcessor.Setup(p => p.GeneratePromptFromSalesInvoiceAsync(It.IsAny<ApplicationDbContext.SalesInvoice>()))
                .ReturnsAsync("Mocked Prompt");
            mockLlmService.Setup(l => l.GenerateResponseAsync(It.IsAny<string>()))
                .ReturnsAsync("Mocked LLM Response");
            // Act
            await salesInvoiceService.GenerateSalesInvoiceAsync(
                salesInvoice.Id, // Id
                salesInvoice.BlobName, // BlobName
                salesInvoice.InvoiceDate, // InvoiceDate
                salesInvoice.InvoiceNumber, // InvoiceNumber
                salesInvoice.CustomerName, // CustomerName
                salesInvoice.CustomerAddress, // CustomerAddress
                salesInvoice.TotalAmount, // TotalAmount
                salesInvoice.SalesTax, // SalesTax
                salesInvoice.NetAmount // NetAmount
            );
            // Assert
            var salesInvoices = dbContext.SalesInvoices.ToList();
            Assert.Single(salesInvoices);

        }
        // Update sales invoice service with mocked dependencies
        [Fact]
        public async Task UpdateSalesInvoice_ShouldUpdateInvoiceNumber_whenInvoiceExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
                .Options;
            
            var dbContext = new ApplicationDbContext(options);
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = 1,
                BlobName = "test_blob",
                InvoiceDate = DateTime.Now,
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test Address",
                TotalAmount = 1000.00m,
                SalesTax = 100.00m,
                NetAmount = 1100.00m
            };
            dbContext.SalesInvoices.Add(salesInvoice);
            await dbContext.SaveChangesAsync();
            var mockDocumentProcessor = new Mock<IDocumentProcessor>();
            var mockLlmService = new Mock<ILlmService>();
            var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
            // Act
            await salesInvoiceService.UpdateSalesInvoiceAsync(
                salesInvoice.Id, // Id
                salesInvoice.BlobName, // BlobName
                salesInvoice.InvoiceDate, // InvoiceDate
                salesInvoice.InvoiceNumber, // InvoiceNumber
                salesInvoice.CustomerName, // CustomerName
                salesInvoice.CustomerAddress, // CustomerAddress
                salesInvoice.TotalAmount, // TotalAmount
                salesInvoice.SalesTax, // SalesTax
                salesInvoice.NetAmount // NetAmount
            );
            // Assert
            var updatedInvoice = dbContext.SalesInvoices.ToList();
            Assert.NotNull(updatedInvoice);
            Assert.Equal(salesInvoice.InvoiceNumber, updatedInvoice.First().InvoiceNumber);
        }
        // Delete Sales Invoices 
        [Fact]
        public async Task DeleteSalesInvoice_ShouldDeleteInvoice_whenInvoiceExists()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
                .Options;
            var mockDocumentProcessor = new Mock<IDocumentProcessor>();
            var mockLlmService = new Mock<ILlmService>();
            var dbContext = new ApplicationDbContext(options);
            var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = 1,
                BlobName = "test_blob",
                InvoiceDate = DateTime.Now,
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test Address",
                TotalAmount = 1000.00m,
                SalesTax = 100.00m,
                NetAmount = 1100.00m
            };
            dbContext.SalesInvoices.Add(salesInvoice);
            await dbContext.SaveChangesAsync();
            // Act
            await salesInvoiceService.DeleteSalesInvoiceAsync(salesInvoice.Id);
            // Assert
            var deletedInvoice = dbContext.SalesInvoices.ToList();
            Assert.Empty(deletedInvoice);
        }
        // Edge cases
        [Fact]
        public async Task GenerateSalesInvoiceRequest_ShouldAddSalesInvoiceAndAccountingEntries()
        {
            // Arrange
            await _salesInvoiceService.GenerateSalesInvoiceAsync(
                2, // Id
                "test_blob", // BlobName
                DateTime.Now, // InvoiceDate
                "INV-001", // InvoiceNumber
                "Test Customer", // CustomerName
                "123 Test Address", // CustomerAddress
                1000.00m, // TotalAmount
                100.00m, // SalesTax
                1100.00m // NetAmount
            );
            // Assert
            var salesInvoice = await _dbContext.SalesInvoices.FindAsync(2);
            Assert.NotNull(salesInvoice);
            Assert.Equal("INV-001", salesInvoice.InvoiceNumber);
            var accountingEntries = await _dbContext.AccountingEntries.ToListAsync();
            Assert.Equal(3, accountingEntries.Count);
        }
        [Fact]
        public async Task GenerateSalesInvoiceAsync_withEmptyLLMResponse_shouldStillCreateInvoice()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
                .Options;
            
            using var dbContext = new ApplicationDbContext(options);
            var documentMock = new Mock<IDocumentProcessor>();
            var salesInvoiceService = new SalesInvoiceService(dbContext, _llmServiceMock.Object, documentMock.Object);

            _llmServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            await _salesInvoiceService.GenerateSalesInvoiceAsync(
                3, // Id
                "blob", // BlobName
                DateTime.Now, // InvoiceDate
                "InvoiceNumber", // InvoiceNumber
                "Test Customer", // CustomerName
                "Test Address", // CustomerAddress
                1000.00m, // TotalAmount
                100.00m, // SalesTax
                1100.00m // NetAmount
            );

            var salesInvoice = await _dbContext.SalesInvoices.FindAsync(3);
            Assert.NotNull(salesInvoice);
            var entries = await _dbContext.AccountingEntries.ToListAsync();
            Assert.Equal(3, entries.Count);
        }
        [Fact]
        public async Task UpdateSalesInvoiceAsync_WithInvalidId_ShouldThrowExemption()
        {
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _salesInvoiceService.UpdateSalesInvoiceAsync(
                    999, // Id
                    "test_blob", // BlobName
                    DateTime.Now, // InvoiceDate
                    "INV-999", // InvoiceNumber
                    "Test Customer", // CustomerName
                    "123 Test Address", // CustomerAddress
                    1000.00m, // TotalAmount
                    100.00m, // SalesTax
                    1100.00m // NetAmount
                );
            });
            Assert.Contains("Sales Invoice with Id", ex.Message);
        }
        [Fact]
        public async Task DeleteSalesInvoiceAsync_WithInvalidId_ShouldThrowException()
        {
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _salesInvoiceService.DeleteSalesInvoiceAsync(999);
            });
            Assert.Contains("Sales Invoice with Id", ex.Message);
        }
    }
}