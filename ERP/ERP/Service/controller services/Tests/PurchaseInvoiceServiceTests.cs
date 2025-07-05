using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ERP.Service.Tests
{
    public class PurchaseInvoiceServiceTests
    {
        private readonly PurchaseInvoiceService _service;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ILlmService> _mockLlmService;
        private readonly Mock<IAccountingService> _mockAccountingService;
        private readonly Mock<INominalAccountResolutionService> _mockNominalAccountResolutionService;
        private readonly Mock<IDocumentService> _mockDocumentService = new Mock<IDocumentService>();
        private readonly Mock<IPurchaseInvoiceService> _mockPurchaseInvoiceService = new Mock<IPurchaseInvoiceService>();

        public PurchaseInvoiceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _mockLlmService = new Mock<ILlmService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockAccountingService = new Mock<IAccountingService>();
            _mockNominalAccountResolutionService = new Mock<INominalAccountResolutionService>();

            _service = new PurchaseInvoiceService(
                _dbContext,
                _mockDocumentService.Object,
                _mockAccountingService.Object,
                _mockLlmService.Object,
                _mockNominalAccountResolutionService.Object
            );
        }

        [Fact]
        public async Task ProcessPurchaseInvoiceAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessPurchaseInvoiceAsync(null!));
            
        }

        [Fact]
        public async Task GetAllPurchaseInvoicesAsync_ShouldReturnList()
        {
            var result = await _service.GetAllPurchaseInvoicesAsync();
            Assert.NotNull(result);
            Assert.IsAssignableFrom<System.Collections.IEnumerable>(result);
        }

        [Fact]
        public async Task DeletePurchaseInvoiceAsync_ShouldThrowKeyNotFoundException_WhenInvoiceNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeletePurchaseInvoiceAsync(nonExistentId));
        }

        [Fact]
        public async Task GetPurchaseInvoiceByIdAsync_ShouldThrowKeyNotFoundException_WhenInvoiceNotFound()
        {
            var nonExistentId = Guid.NewGuid();
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetPurchaseInvoiceByIdAsync(nonExistentId));
        }

        [Fact]
        public async Task GetPurchaseInvoiceByUserIdAsync_ShouldThrowKeyNotFoundException_WhenInvoiceNotFound()
        {
            var nonExistentUserId = Guid.NewGuid();
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetPurchaseInvoiceByUserIdAsync(nonExistentUserId));
        }
    }
}
