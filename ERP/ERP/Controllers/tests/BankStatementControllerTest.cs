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
    public class BankStatementControllerTest
    {
        private readonly Mock<IBankStatementService> _mockService;
        private readonly BankStatementController _controller;

        public BankStatementControllerTest()
        {
            _mockService = new Mock<IBankStatementService>();
            _controller = new BankStatementController(_mockService.Object);
        }

        [Fact]
        public async Task GetBankStatementById_ReturnsOk_WhenBankStatementExists()
        {
            var id = Guid.NewGuid();
            var bankStatement = new ApplicationDbContext.BankStatement
            {
                Id = id,
                BlobName = "TestBlob",
                OpeningBalance = 100m,
                ClosingBalance = 200m
            };
            _mockService.Setup(s => s.GetBankStatementByIdAsync(id)).ReturnsAsync(bankStatement);

            var result = await _controller.GetBankStatementById(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBankStatement = Assert.IsType<ApplicationDbContext.BankStatement>(okResult.Value);
            Assert.Equal(id, returnedBankStatement.Id);
        }

        [Fact]
        public async Task GetBankStatementById_ReturnsNotFound_WhenBankStatementDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetBankStatementByIdAsync(id)).ReturnsAsync((ApplicationDbContext.BankStatement?)null);

            var result = await _controller.GetBankStatementById(id);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetBankStatementsByUser_ReturnsOk_WithBankStatements()
        {
            var userId = Guid.NewGuid();
            var bankStatements = new List<ApplicationDbContext.BankStatement>
            {
                new ApplicationDbContext.BankStatement
                {
                    Id = Guid.NewGuid(),
                    BlobName = "TestBlob1",
                    OpeningBalance = 100m,
                    ClosingBalance = 200m
                },
                new ApplicationDbContext.BankStatement
                {
                    Id = Guid.NewGuid(),
                    BlobName = "TestBlob2",
                    OpeningBalance = 300m,
                    ClosingBalance = 400m
                }
            };
            _mockService.Setup(s => s.GetBankStatementsByUserAsync(userId)).ReturnsAsync(bankStatements);

            var result = await _controller.GetBankStatementsByUser(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<ApplicationDbContext.BankStatement>>(okResult.Value);
            Assert.Equal(2, ((List<ApplicationDbContext.BankStatement>)returnedList).Count);
        }

        [Fact]
        public async Task DeleteBankStatement_ReturnsNoContent_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteBankStatementAsync(id)).ReturnsAsync(true);

            var result = await _controller.DeleteBankStatement(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBankStatement_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteBankStatementAsync(id)).ReturnsAsync(false);

            var result = await _controller.DeleteBankStatement(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var amendedBankStatement = new ApplicationDbContext.BankStatement
            {
                Id = id,
                BlobName = "TestBlob",
                OpeningBalance = 100m,
                ClosingBalance = 200m
            };
            _mockService.Setup(s => s.AmendBankStatementAsync(amendedBankStatement)).ReturnsAsync(amendedBankStatement);

            var result = await _controller.AmendBankStatement(id, amendedBankStatement);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBankStatement = Assert.IsType<ApplicationDbContext.BankStatement>(okResult.Value);
            Assert.Equal(id, returnedBankStatement.Id);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsBadRequest_WhenIdMismatch()
        {
            var id = Guid.NewGuid();
            var amendedBankStatement = new ApplicationDbContext.BankStatement
            {
                Id = Guid.NewGuid(),
                BlobName = "DefaultBlobName",
                OpeningBalance = 0m,
                ClosingBalance = 0m
            };

            var result = await _controller.AmendBankStatement(id, amendedBankStatement);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsNotFound_WhenExceptionThrown()
        {
            var id = Guid.NewGuid();
            var amendedBankStatement = new ApplicationDbContext.BankStatement
            {
                Id = id,
                BlobName = "TestBlob",
                OpeningBalance = 100m,
                ClosingBalance = 200m
            };
            _mockService.Setup(s => s.AmendBankStatementAsync(amendedBankStatement)).ThrowsAsync(new ArgumentException());

            var result = await _controller.AmendBankStatement(id, amendedBankStatement);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task ReconcileBankStatement_ReturnsOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            decimal userInputBalance = 100m;
            _mockService.Setup(s => s.ReconcileBankStatementAsync(id, userInputBalance)).ReturnsAsync(true);

            var result = await _controller.ReconcileBankStatement(id, userInputBalance);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task ReconcileBankStatement_ReturnsNotFound_WhenBankStatementNotFound()
        {
            var id = Guid.NewGuid();
            decimal userInputBalance = 100m;
            _mockService.Setup(s => s.ReconcileBankStatementAsync(id, userInputBalance)).ReturnsAsync(false);

            var result = await _controller.ReconcileBankStatement(id, userInputBalance);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
