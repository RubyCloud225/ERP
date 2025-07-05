using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ERP.Controllers;
using ERP.Service;
using ERP.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ERP.Controller;

namespace ERP.Service.Tests
{
    public class DocumentControllerTest
    {
        private readonly Mock<IDocumentProcessor> _mockDocumentProcessor;
        private readonly Mock<CloudStorageService> _mockCloudStorageService;
        private readonly Mock<ApplicationDbContext> _dbContext;
        private readonly DocumentController _controller;

        public DocumentControllerTest()
        {
            _mockDocumentProcessor = new Mock<IDocumentProcessor>();
            _mockCloudStorageService = new Mock<CloudStorageService>(null);
            _dbContext = new Mock<ApplicationDbContext>(null);

            // Mock Add and SaveChangesAsync for DocumentRecords
            _dbContext.Setup(db => db.DocumentRecords.Add(It.IsAny<ERP.Model.ApplicationDbContext.DocumentRecord>()));
            _dbContext.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            _controller = new DocumentController(_mockDocumentProcessor.Object, null!, _mockCloudStorageService.Object, _dbContext.Object);
        }

        [Fact]
        public async Task ImportDocument_ReturnsBadRequest_WhenNoFile()
        {
            var result = await _controller.ImportDocument(null!);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file selected", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportDocument_ReturnsBadRequest_WhenFileTooLarge()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6 MB
            var result = await _controller.ImportDocument(mockFile.Object);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File size is too large", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportDocument_ReturnsNotFound_WhenDocumentRecordNotFound()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            _mockCloudStorageService.Setup(s => s.UploadToCloudStorageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("http://fakeurl.com/blob.pdf");

            _dbContext.Setup(db => db.DocumentRecords.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationDbContext.DocumentRecord, bool>>>(), default))
                .ReturnsAsync((ApplicationDbContext.DocumentRecord)null!);

            var result = await _controller.ImportDocument(mockFile.Object);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Document record not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ImportDocument_ReturnsOk_WhenSuccess()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var documentRecord = new ERP.Model.ApplicationDbContext.DocumentRecord
            {
                BlobName = "blob",
                Id = Guid.NewGuid(),
                DocumentContent = "content",
                DocumentType = "type"
            };

            _mockCloudStorageService.Setup(s => s.UploadToCloudStorageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("http://fakeurl.com/blob.pdf");

            _dbContext.Setup(db => db.DocumentRecords.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ERP.Model.ApplicationDbContext.DocumentRecord, bool>>>(), default))
                .ReturnsAsync(documentRecord);

            _mockDocumentProcessor.Setup(p => p.ProcessDocumentAsync(documentRecord))
                .ReturnsAsync("Processed");

            _mockDocumentProcessor.Setup(p => p.CategorizeDocumentAsync("blob"))
                .ReturnsAsync("Type");

            var result = await _controller.ImportDocument(mockFile.Object);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<Dictionary<string, object>>(okResult.Value);
            Assert.Equal("http://fakeurl.com/blob.pdf", value["FileUrl"]);
            Assert.Equal("Processed", value["Response"]);
            Assert.Equal("Type", value["Type"]);
        }

        [Fact]
        public async Task GetDocumentsByType_ReturnsBadRequest_WhenTypeIsNull()
        {
            var result = await _controller.GetDocumentsByType(null!);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Document type is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GetDocumentsByType_ReturnsNotFound_WhenNoDocuments()
        {
            _mockDocumentProcessor.Setup(s => s.GetDocumentsByTypeAsync("type"))
                .ReturnsAsync(new List<string>());

            var result = await _controller.GetDocumentsByType("type");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetDocumentsByType_ReturnsOk_WhenDocumentsExist()
        {
            var documents = new List<string> { "doc1", "doc2" };
            _mockDocumentProcessor.Setup(s => s.GetDocumentsByTypeAsync("type"))
                .ReturnsAsync(documents);

            var result = await _controller.GetDocumentsByType("type");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(documents, okResult.Value);
        }

        [Fact]
        public async Task DeleteDocument_ReturnsBadRequest_WhenIdIsNull()
        {
            var result = await _controller.DeleteDocument(null!);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Document Id is required", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteDocument_ReturnsNoContent_WhenSuccess()
        {
            _mockDocumentProcessor.Setup(s => s.DeleteDocumentAsync("id"))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteDocument("id");
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDocument_ReturnsServerError_WhenException()
        {
            _mockDocumentProcessor.Setup(s => s.DeleteDocumentAsync("id"))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.DeleteDocument("id");
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DownloadDocument_ReturnsBadRequest_WhenIdIsNull()
        {
            var result = await _controller.DownloadDocument(null!);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Document Id is required", badRequestResult.Value);
        }

        [Fact]
        public async Task DownloadDocument_ReturnsNotFound_WhenDocumentNull()
        {
            _mockDocumentProcessor.Setup(s => s.DownloadDocumentAsync("id"))
                .ReturnsAsync((Stream)null!);

            var result = await _controller.DownloadDocument("id");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DownloadDocument_ReturnsFile_WhenSuccess()
        {
            var fileStream = new MemoryStream(new byte[] { 1, 2, 3 });
            _mockDocumentProcessor.Setup(s => s.DownloadDocumentAsync("id"))
                .ReturnsAsync(fileStream);

            var result = await _controller.DownloadDocument("id");
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/octet-stream", fileResult.ContentType);
            Assert.Equal("id", fileResult.FileDownloadName);
        }
    }
}
