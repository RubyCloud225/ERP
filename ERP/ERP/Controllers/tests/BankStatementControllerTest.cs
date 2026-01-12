using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Controllers.Tests
{
    public class BankStatementControllerTest : IDisposable
    {
       private readonly ApplicationDbContext _context;
        private readonly BankStatementService _service;
        private readonly BankStatementController _controller;

        private readonly Mock<ILlmService> _mockLlmService = new Mock<ILlmService>();
        private readonly Mock<IAccountingService> _mockAccountingService = new Mock<IAccountingService>();
        private readonly Mock<INominalAccountResolutionService> _mockNominalAccountResolutionService = new Mock<INominalAccountResolutionService>();

        public BankStatementControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _service = new BankStatementService(
                _context,
                _mockLlmService.Object,
                _mockAccountingService.Object,
                _mockNominalAccountResolutionService.Object);

            _controller = new BankStatementController(_service);

            // Seed data
            _context.BankStatements.AddRange(new[]
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
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetBankStatementById_ReturnsOk_WhenBankStatementExists()
        {
            var bankStatement = await _context.BankStatements.FirstAsync();

            var result = await _controller.GetBankStatementById(bankStatement.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBankStatement = Assert.IsType<ApplicationDbContext.BankStatement>(okResult.Value);
            Assert.Equal(bankStatement.Id, returnedBankStatement.Id);
        }

        [Fact]
        public async Task GetBankStatementById_ReturnsNotFound_WhenBankStatementDoesNotExist()
        {
            var result = await _controller.GetBankStatementById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetBankStatementsByUser_ReturnsOk_WithBankStatements()
        {
            // Assuming your BankStatement entity has a UserId property or similar for filtering
            // Since it's missing in your example, simulate with all statements for now

            var result = await _controller.GetBankStatementsByUser(Guid.NewGuid());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<ApplicationDbContext.BankStatement>>(okResult.Value);
            Assert.NotNull(returnedList);
        }

        [Fact]
        public async Task DeleteBankStatement_ReturnsNoContent_WhenDeleted()
        {
            var bankStatement = await _context.BankStatements.FirstAsync();

            var result = await _controller.DeleteBankStatement(bankStatement.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBankStatement_ReturnsNotFound_WhenNotFound()
        {
            var result = await _controller.DeleteBankStatement(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsOk_WhenSuccessful()
        {
            var bankStatement = await _context.BankStatements.FirstAsync();
            bankStatement.BlobName = "UpdatedBlob";

            var result = await _controller.AmendBankStatement(bankStatement.Id, bankStatement);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBankStatement = Assert.IsType<ApplicationDbContext.BankStatement>(okResult.Value);
            Assert.Equal(bankStatement.Id, returnedBankStatement.Id);
            Assert.Equal("UpdatedBlob", returnedBankStatement.BlobName);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsBadRequest_WhenIdMismatch()
        {
            var bankStatement = await _context.BankStatements.FirstAsync();

            var result = await _controller.AmendBankStatement(Guid.NewGuid(), bankStatement);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task AmendBankStatement_ReturnsNotFound_WhenExceptionThrown()
        {
            // Simulate exception by passing invalid data or modifying the service for test if needed
            // Here just using a non-existing id and expecting NotFound

            var bankStatement = await _context.BankStatements.FirstAsync();
            var invalidId = Guid.NewGuid();

            var result = await _controller.AmendBankStatement(invalidId, bankStatement);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task ReconcileBankStatement_ReturnsOk_WhenSuccessful()
        {
            var bankStatement = await _context.BankStatements.FirstAsync();
            decimal userInputBalance = 100m;

            var result = await _controller.ReconcileBankStatement(bankStatement.Id, userInputBalance);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task ReconcileBankStatement_ReturnsNotFound_WhenBankStatementNotFound()
        {
            var result = await _controller.ReconcileBankStatement(Guid.NewGuid(), 100m);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
