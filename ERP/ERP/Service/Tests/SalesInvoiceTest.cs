using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ERP.Service.Tests
{
    public class SalesInvoiceTests
    {
        private readonly Mock<ILlmService> _llmServiceMock;
        private readonly Mock<IDocumentService> _documentServiceMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly SalesInvoiceService _salesInvoiceService;

        public SalesInvoiceServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _llmServiceMock = new Mock<ILlmService>();
            _documentServiceMock = new Mock<IDocumentService>();
            _llmServiceMock.Setup(x => x.GenerateSalesInvoiceRequest(It.IsAny<SalesInvoice>()))
                .ReturnsAsync(new LlmResponse { Response = "Mocked LLM Response" });
            _documentProcessorMock.Setup(It.IsAny<ApplicationDBContext.SalesInvoice>())
                .ReturnsAsync(new DocumentRecord { BlobName = "mocked_blob_name" });
            _service = new SalesInvoiceService(
                _dbContext,
                _llmServiceMock.Object,
                _documentServiceMock.Object
            );
            [FACT]
            public async Task GenerateSalesInvoiceRequest_ShouldAddSalesInvoiceAndAccountingEntries()
            {
                // Assert
                var salesInvoice = await _dbContext.SalesInvoices.FindAsync(1);
                Assert.NotNull(salesInvoice);
                Assert.Equal("Mocked LLM Response", salesInvoice.InvoiceNumber);
                var accountingEntries = await _dbContext.AccountingEntries.ToListAsync();
                Assert.Equal(3, accountingEntries.Count);
            }
            [FACT]
            public async Task GenerateSalesInvoiceAsync_withEmptyLLMResponse_shouldStillCreateInvoice()
            {
                _llmServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>()))
                    .ReturnsAsync(new LlmResponse { Response = string.Empty });

                await _service.GenerateSalesInvoiceAsync(2, "test2.pdf", DateTimeNow, "InvoiceNumber", "Test Customer", "Test Description", "Test Address", 1000.00m, "USD", "Test User");

                var salesInvoice = await _dbContext.SalesInvoices.FindAsync(2);
                Assert.NotNull(salesInvoice);
                var entries = await _dbContext.AccountingEntries.ToListAsync();
                Assert.Equal(3, entries.Count);
            }
            [FACT]
            public async Task UpdateSalesInvoiceAsync_WithInvalidId_ShouldThrowExemption()
            {
                var ex = await Assert.ThrowsAsync<Exception>(() =>
                    _service.UpdateSalesInvoiceAsync(999, "blob", DateTime.Now, "InvoiceNumber", "Test Customer", "Test Description", "Test Address", 1, 1, 1));
                Assert.Equal("Sales Invoice not found", ex.Message);
            }
            [FACT]
            public async Task DeleteSalesInvoiceAsync_WithInvalidId_ShouldThrowException()
            {
                var fakeInvoice = new SalesInvoiceTests {
                    Id = 999,
                    InvoiceNumber = "INV-999",
                    CustomerName = "Test Customer",
                    Description = "Test Description",
                    Address = "Test Address",
                    Amount = 1000.00m,
                    Currency = "USD",
                    CreatedBy = "Test User",
                    CreatedAt = DateTime.Now
                };
                var ex = await Assert.ThrowsAsync<Exception>(() =>
                    _service.DeleteSalesInvoiceAsync(fakeInvoice.Id));
                Assert.contains("Sales Invoice Id", ex.Message);
            }
        }
    }
}
