using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ERP.Service.Tests
{
    public class PurchaseInvoiceControllerTest
    {
        private readonly Mock<IPurchaseInvoiceService> _mockService;
        private readonly PurchaseInvoiceController _controller;

        public PurchaseInvoiceControllerTest()
        {
            _mockService = new Mock<IPurchaseInvoiceService>();
            _controller = new PurchaseInvoiceController(_mockService.Object);
        }

        [Fact]
        public async Task CreatePurchaseInvoice_ReturnsCreatedResult()
        {
            var dto = new ApplicationDbContext.CreatePurchaseInvoiceDto { UserId = Guid.NewGuid() };
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                BlobName = "blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Supplier",
                Description = "Description",
                SupplierAddress = "Address",
                DocumentType = "Purchase Invoice",
                Response = "Processed"
            };

            _mockService.Setup(s => s.ProcessPurchaseInvoiceAsync(dto))
                .ReturnsAsync(purchaseInvoice);

            var result = await _controller.CreatePurchaseInvoice(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetPurchaseInvoiceById), createdResult.ActionName);
            Assert.Equal(purchaseInvoice, createdResult.Value);
        }

        [Fact]
        public async Task AmendPurchaseInvoice_ReturnsOkResult()
        {
            var id = Guid.NewGuid();
            var dto = new ApplicationDbContext.CreatePurchaseInvoiceDto { UserId = Guid.NewGuid() };
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                BlobName = "blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Supplier",
                Description = "Description",
                SupplierAddress = "Address",
                DocumentType = "Purchase Invoice",
                Response = "Processed",
                Id = id,
                UserId = dto.UserId
            };

            _mockService.Setup(s => s.AmendPurchaseInvoiceAsync(id, dto))
                .ReturnsAsync(purchaseInvoice);

            var result = await _controller.AmendPurchaseInvoice(id, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(purchaseInvoice, okResult.Value);
        }

        [Fact]
        public async Task AmendPurchaseInvoice_ReturnsNotFound_WhenKeyNotFound()
        {
            var id = Guid.NewGuid();
            var dto = new ApplicationDbContext.CreatePurchaseInvoiceDto();

            _mockService.Setup(s => s.AmendPurchaseInvoiceAsync(id, dto))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.AmendPurchaseInvoice(id, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeletePurchaseInvoice_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeletePurchaseInvoiceAsync(id))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeletePurchaseInvoice(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePurchaseInvoice_ReturnsNotFound_WhenKeyNotFound()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeletePurchaseInvoiceAsync(id))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeletePurchaseInvoice(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPurchaseInvoiceById_ReturnsOkResult()
        {
            var id = Guid.NewGuid();
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = id,
                BlobName = "blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Supplier",
                Description = "Description",
                SupplierAddress = "Address",
                DocumentType = "Purchase Invoice",
                Response = "Processed",
                UserId = Guid.NewGuid()
            };

            _mockService.Setup(s => s.GetPurchaseInvoiceByIdAsync(id))
                .ReturnsAsync(purchaseInvoice);

            var result = await _controller.GetPurchaseInvoiceById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(purchaseInvoice, okResult.Value);
        }

        [Fact]
        public async Task GetPurchaseInvoiceById_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.GetPurchaseInvoiceByIdAsync(id))
                .ReturnsAsync((ApplicationDbContext.PurchaseInvoice?)null);

            var result = await _controller.GetPurchaseInvoiceById(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPurchaseInvoiceByUserId_ReturnsOkResult()
        {
            var userId = Guid.NewGuid();
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                BlobName = "blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Supplier",
                Description = "Description",
                SupplierAddress = "Address",
                DocumentType = "Purchase Invoice",
                Response = "Processed",
                UserId = userId
            };

            _mockService.Setup(s => s.GetPurchaseInvoiceByUserIdAsync(userId))
                .ReturnsAsync(purchaseInvoice);

            var result = await _controller.GetPurchaseInvoiceByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(purchaseInvoice, okResult.Value);
        }

        [Fact]
        public async Task GetPurchaseInvoiceByUserId_ReturnsNotFound_WhenNull()
        {
            var userId = Guid.NewGuid();

            _mockService.Setup(s => s.GetPurchaseInvoiceByUserIdAsync(userId))
                .ReturnsAsync((ApplicationDbContext.PurchaseInvoice?)null);

            var result = await _controller.GetPurchaseInvoiceByUserId(userId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllPurchaseInvoices_ReturnsOkResult()
        {
            var list = new List<ApplicationDbContext.PurchaseInvoice> { new ApplicationDbContext.PurchaseInvoice {
                Id = Guid.NewGuid(),
                BlobName = "blob",
                PurchaseInvoiceNumber = "INV-001",
                Supplier = "Supplier",
                Description = "Description",
                SupplierAddress = "Address",
                DocumentType = "Purchase Invoice",
                Response = "Processed",
                UserId = Guid.NewGuid()
            }};

            _mockService.Setup(s => s.GetAllPurchaseInvoicesAsync())
                .ReturnsAsync(list);

            var result = await _controller.GetAllPurchaseInvoices();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }
    }
}
