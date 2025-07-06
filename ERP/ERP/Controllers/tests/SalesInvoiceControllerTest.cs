using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Controllers.Tests
{
        public class SalesInvoiceControllerTest
        {
            private readonly Mock<ISalesInvoiceService> _mockService;
            private readonly SalesInvoiceController _controller;

            public SalesInvoiceControllerTest()
            {
                _mockService = new Mock<ISalesInvoiceService>();
                _controller = new SalesInvoiceController(_mockService.Object);
            }

        [Fact]
        public async Task CreateSalesInvoice_ReturnsCreatedAtAction()
        {
            var request = new ApplicationDbContext.GenerateSalesInvoiceDto
            {
                CustomerName = "Test Customer",
                LineItems = new List<ApplicationDbContext.SalesInvoiceLineDto>()
            };
            var blobName = "TestBlob";
            Guid? userId = Guid.NewGuid();
            var resultInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = Guid.NewGuid(),
                BlobName = "TestBlob",
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test St",
                UserId = userId.Value,
                User = new ApplicationDbContext.User
                {
                    Id = userId.Value,
                    Name = "Test User",
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Password = "password"
                }
            };

            var newId = Guid.NewGuid();

            var result = await _controller.CreateSalesInvoice(request, blobName, userId);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedInvoice = Assert.IsType<ApplicationDbContext.SalesInvoice>(createdAtActionResult.Value);
            Assert.Equal(resultInvoice.Id, returnedInvoice.Id);
            Assert.Equal("GetSalesInvoiceById", createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task AmendSalesInvoice_ReturnsOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var request = new ApplicationDbContext.GenerateSalesInvoiceDto
            {
                CustomerName = "Test Customer",
                LineItems = new System.Collections.Generic.List<ApplicationDbContext.SalesInvoiceLineDto>()
            };
            var blobName = "TestBlob";
            Guid? userId = Guid.NewGuid();
            var resultInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = id,
                BlobName = "TestBlob",
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test St",
                UserId = userId.Value,
                User = new ApplicationDbContext.User
                {
                    Id = userId.Value,
                    Name = "Test User",
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Password = "password"
                }
            };

            var newId = Guid.NewGuid();
            var result = await _controller.AmendSalesInvoice(newId, request, blobName, userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedInvoice = Assert.IsType<ApplicationDbContext.SalesInvoice>(okResult.Value);
            Assert.Equal(newId, returnedInvoice.Id);
        }

        [Fact]
        public async Task AmendSalesInvoice_ReturnsNotFound_WhenKeyNotFoundException()
        {
            var id = Guid.NewGuid();
            var request = new ApplicationDbContext.GenerateSalesInvoiceDto
            {
                CustomerName = "Test Customer",
                LineItems = new System.Collections.Generic.List<ApplicationDbContext.SalesInvoiceLineDto>()
            };
            var blobName = "TestBlob";
            Guid? userId = Guid.NewGuid();

            var result = await _controller.AmendSalesInvoice(id, request, blobName, userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            Assert.Contains("not found", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task DeleteSalesInvoice_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteSalesInvoiceAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteSalesInvoice(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSalesInvoice_ReturnsNotFound_WhenKeyNotFoundException()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteSalesInvoiceAsync(id)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteSalesInvoice(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            Assert.Contains("not found", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task GetSalesInvoiceById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = id,
                BlobName = "TestBlob",
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test St",
                UserId = Guid.NewGuid(),
                User = new ApplicationDbContext.User
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User",
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Password = "password"
                }
            };

            _mockService.Setup(s => s.GetSalesInvoiceByIdAsync(id)).ReturnsAsync(salesInvoice);

            var result = await _controller.GetSalesInvoiceById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedInvoice = Assert.IsType<ApplicationDbContext.SalesInvoice>(okResult.Value);
            Assert.Equal(id, returnedInvoice.Id);
        }

        [Fact]
        public async Task GetSalesInvoiceById_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.GetSalesInvoiceByIdAsync(id)).ReturnsAsync((ApplicationDbContext.SalesInvoice?)null);

            var result = await _controller.GetSalesInvoiceById(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFoundResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task GetSalesInvoiceByUserId_ReturnsOk_WhenFound()
        {
            var userId = Guid.NewGuid();
            var salesInvoices = new List<ApplicationDbContext.SalesInvoice>
            {
                new ApplicationDbContext.SalesInvoice
                {
                    Id = Guid.NewGuid(),
                    BlobName = "TestBlob1",
                    InvoiceNumber = "INV-001",
                    CustomerName = "Test Customer",
                    CustomerAddress = "123 Test St",
                    UserId = userId,
                    User = new ApplicationDbContext.User
                    {
                        Id = userId,
                        Name = "Test User",
                        Username = "testuser",
                        Email = "testuser@example.com",
                        Password = "password"
                    }
                },
                new ApplicationDbContext.SalesInvoice
                {
                    Id = Guid.NewGuid(),
                    BlobName = "TestBlob2",
                    InvoiceNumber = "INV-002",
                    CustomerName = "Test Customer 2",
                    CustomerAddress = "456 Test Ave",
                    UserId = userId,
                    User = new ApplicationDbContext.User
                    {
                        Id = userId,
                        Name = "Test User 2",
                        Username = "testuser2",
                        Email = "testuser2@example.com",
                        Password = "password2"
                    }
                }
            };

            _mockService.Setup(s => s.GetSalesInvoiceByUserIdAsync(userId));

            var result = await _controller.GetSalesInvoiceByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<ApplicationDbContext.SalesInvoice>>(okResult.Value);
            Assert.Equal(2, ((List<ApplicationDbContext.SalesInvoice>)returnedList).Count);
        }

        [Fact]
        public async Task GetSalesInvoiceByUserId_ReturnsNotFound_WhenNotFound()
        {
            var userId = Guid.NewGuid();

            _mockService.Setup(s => s.GetSalesInvoiceByUserIdAsync(It.Is<Guid>(u => u == userId)));

            var result = await _controller.GetSalesInvoiceByUserId(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFoundResult.Value?.ToString() ?? string.Empty);
        }
    }
}
