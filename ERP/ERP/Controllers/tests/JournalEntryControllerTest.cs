using ERP.Controllers;
using ERP.Model;
using ERP.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
// Removed duplicate using directive for 'ERP.Service'
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERP.Controllers.Tests
{
    public class AccountingEntryControllerTest
    {
        private readonly Mock<IJournalEntryService> _mockService;
        private readonly JournalEntryController _controller;

        public AccountingEntryControllerTest()
        {
            _mockService = new Mock<IJournalEntryService>();
            _controller = new JournalEntryController(_mockService.Object);
        }

        [Fact]
        public async Task CreateAccountingEntry_ReturnsCreatedAtAction()
        {
            var request = new CreateJournalEntryDto();
            Guid? userId = Guid.NewGuid();
            var resultEntry = new ApplicationDbContext.AccountingEntry
            {
                Id = Guid.NewGuid(),
                Description = "Test Description",
                EntryDate = DateTime.UtcNow,
                UserId = userId
            };

            _mockService.Setup(s => s.CreateJournalEntryAsync(request, userId)).ReturnsAsync(resultEntry);

            var result = await _controller.CreateJournalEntry(request, userId);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedEntry = Assert.IsType<ApplicationDbContext.AccountingEntry>(createdAtActionResult.Value);
            Assert.Equal(resultEntry.Id, returnedEntry.Id);
        }

        [Fact]
        public async Task AmendAccountingEntry_ReturnsOk_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var request = new CreateJournalEntryDto();
            Guid? userId = Guid.NewGuid();
            var resultEntry = new ApplicationDbContext.AccountingEntry
            {
                Id = id,
                Description = "Test Description",
                EntryDate = DateTime.UtcNow,
                UserId = userId
            };

            _mockService.Setup(s => s.AmendJournalEntryAsync(id, request, userId)).ReturnsAsync(resultEntry);

            var result = await _controller.AmendJournalEntry(id, request, userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEntry = Assert.IsType<ApplicationDbContext.AccountingEntry>(okResult.Value);
            Assert.Equal(id, returnedEntry.Id);
        }

        [Fact]
        public async Task AmendJournalEntry_ReturnsNotFound_WhenKeyNotFoundException()
        {
            var id = Guid.NewGuid();
            var request = new CreateJournalEntryDto();
            Guid? userId = Guid.NewGuid();

            _mockService.Setup(s => s.AmendJournalEntryAsync(id, request, userId)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.AmendJournalEntry(id, request, userId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteJournalEntry_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteJournalEntryAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteJournalEntry(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteJournalEntry_ReturnsNotFound_WhenKeyNotFoundException()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteJournalEntryAsync(id)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteJournalEntry(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetJournalEntryById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var accountingEntry = new ApplicationDbContext.AccountingEntry { Id = id, Description = "Sample Description" };

            _mockService.Setup(s => s.GetJournalEntryByIdAsync(id)).ReturnsAsync(accountingEntry);

            var result = await _controller.GetJournalEntryById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEntry = Assert.IsType<ApplicationDbContext.JournalEntry>(okResult.Value);
            Assert.Equal(id, returnedEntry.Id);
        }

        [Fact]
        public async Task GetJournalEntryById_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.GetJournalEntryByIdAsync(id)).ReturnsAsync((ApplicationDbContext.AccountingEntry?)null);

            var result = await _controller.GetJournalEntryById(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetJournalEntryByUserId_ReturnsOk_WhenFound()
        {
            var userId = Guid.NewGuid();
            var journalEntries = new List<ApplicationDbContext.JournalEntry>
            {
                new ApplicationDbContext.JournalEntry { Id = Guid.NewGuid(), Description = "Sample Description" },
                new ApplicationDbContext.JournalEntry { Id = Guid.NewGuid(), Description = "Another Sample Description" }
            };

            ApplicationDbContext.AccountingEntry? journalEntry = null; // or a fake AccountingEntry object
            _mockService.Setup(s => s.GetJournalEntryByUserIdAsync(userId)).ReturnsAsync(journalEntry);

            var result = await _controller.GetJournalEntryByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<ApplicationDbContext.JournalEntry>>(okResult.Value);
            Assert.Equal(2, ((List<ApplicationDbContext.JournalEntry>)returnedList).Count);
        }

        [Fact]
        public async Task GetJournalEntryByUserId_ReturnsNotFound_WhenNotFound()
        {
            var userId = Guid.NewGuid();

            _mockService.Setup(s => s.GetJournalEntryByUserIdAsync(userId)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.GetJournalEntryByUserId(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Journal entry with user not found", notFoundResult.Value);}
    }
}
