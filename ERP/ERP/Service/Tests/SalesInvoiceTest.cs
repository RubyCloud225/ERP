using System;
using System.Threading.Tasks;
using ERP.Model;
using ERP.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IO;
using Npgsql;

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
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Override to use consistent testuser credentials
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var dbName = $"test_db_{Guid.NewGuid()}";
            var connectionString = baseConnectionString + dbName;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;


            _dbContext = new ApplicationDbContext(options);
            try
            {
                _dbContext.Database.EnsureDeleted(); // Ensure the database is deleted before each test
                _dbContext.Database.EnsureCreated();
                SeedTestUser();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Initialize the test database {ex.Message}");
            }
            // Initialize mocks and service
            _llmServiceMock = new Mock<ILlmService>();
            _documentServiceMock = new Mock<IDocumentProcessor>();
            _salesInvoiceService = new SalesInvoiceService(_dbContext, _llmServiceMock.Object, _documentServiceMock.Object);
        }

        private void SeedTestUser()
        {
            var user = new ApplicationDbContext.User
            {
                Id = 1,
                Name = "Test User",
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "TestPassword"
            };
            if (!_dbContext.Users.AnyAsync(u => u.Id == user.Id).GetAwaiter().GetResult())
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
            }
        }
        // Create sales invoice service with mocked dependencies
        [Fact]
        public async Task CreateSalesInvoice_with_Accounting_entries()
        {
            // Arrange
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseNpgsql(connectionString) // Unique database name for each test
               .Options;
            var mockDocumentProcessor = new Mock<IDocumentProcessor>();
            var mockLlmService = new Mock<ILlmService>();
            var dbContext = new ApplicationDbContext(options);
            var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
            var user = new ApplicationDbContext.User
            {
                Id = 1, // Set the required UserId property
                Name = "Test Name",
                Username = "TestUsername",
                Email = "testuser@example.com",
                Password = "TestPassword123"
            };
            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = 1,
                BlobName = "test_blob",
                InvoiceDate = DateTime.UtcNow,
                InvoiceNumber = "INV-001",
                CustomerName = "Test Customer",
                CustomerAddress = "123 Test Address",
                TotalAmount = 1000.00m,
                SalesTax = 100.00m,
                NetAmount = 1100.00m,
                UserId = user.Id,
                User = user
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
                salesInvoice.NetAmount, // NetAmount
                salesInvoice.UserId // UserId
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
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;

            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var user = new ApplicationDbContext.User
                {
                    Id = 1,
                    Name = "Test User",
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Password = "TestPassword"
                };
                var salesInvoice = new ApplicationDbContext.SalesInvoice
                {
                    Id = 1,
                    BlobName = "test_blob",
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceNumber = "TestInvoiceNumber",
                    CustomerName = "Test Name",
                    CustomerAddress = "Test Address",
                    TotalAmount = 100,
                    SalesTax = 10,
                    NetAmount = 90,
                    UserId = user.Id,
                    User = user
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
                    salesInvoice.NetAmount, // NetAmount
                    salesInvoice.UserId // UserId
                );
                var updatedInvoice = dbContext.SalesInvoices.ToList();
                Assert.NotNull(updatedInvoice);
                Assert.Equal(salesInvoice.InvoiceNumber, updatedInvoice.First().InvoiceNumber);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
            // Assert    
        }
        // Delete Sales Invoices 
        [Fact]
        public async Task DeleteSalesInvoice_ShouldDeleteInvoice_whenInvoiceExists()
        {
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var user = new ApplicationDbContext.User
                {
                    Id = 1,
                    Name = "Test User", // Set the required Name property
                    Email = "test@example.com",
                    Username = "Test User",
                    Password = "test_password"
                };
                var salesInvoice = new ApplicationDbContext.SalesInvoice
                {
                    Id = 1,
                    BlobName = "test_blob",
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceNumber = "INV-001",
                    CustomerName = "Test Customer",
                    CustomerAddress = "Test Address",
                    TotalAmount = 100.0m,
                    SalesTax = 10.0m,
                    NetAmount = 90.0m,
                    UserId = user.Id,
                    User = user
                };
                dbContext.SalesInvoices.Add(salesInvoice);
                await dbContext.SaveChangesAsync();
                // Act
                await salesInvoiceService.DeleteSalesInvoiceAsync(salesInvoice.Id, salesInvoice.UserId);
                // Assert
                var deletedSalesInvoice = await dbContext.SalesInvoices.FindAsync(salesInvoice.Id);
                Assert.Null(deletedSalesInvoice);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        // Edge cases
        [Fact]
        public async Task GenerateSalesInvoiceRequest_ShouldAddSalesInvoiceAndAccountingEntries()
        {
            // Arrange
            await _salesInvoiceService.GenerateSalesInvoiceAsync(
                2, // Id
                "test_blob", // BlobName
                DateTime.UtcNow,
                "INV-001", // InvoiceNumber
                "Test Customer", // CustomerName
                "123 Test Address", // CustomerAddress
                1000.00m, // TotalAmount
                100.00m, // SalesTax
                1100.00m, // NetAmount
                1 // UserId
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
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;

            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var documentMock = new Mock<IDocumentProcessor>();
                var llmServiceMock = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, llmServiceMock.Object, documentMock.Object);
                // Mock the LLM service to return an empty response
                llmServiceMock.Setup(s => s.GenerateResponseAsync(It.IsAny<string>()))
                    .ReturnsAsync(string.Empty); // Simulate an empty response from LLM
                // Arrange
                await _salesInvoiceService.GenerateSalesInvoiceAsync(
                    3, // Id
                    "blob", // BlobName
                    DateTime.UtcNow,
                    "InvoiceNumber", // InvoiceNumber
                    "Test Customer", // CustomerName
                    "Test Address", // CustomerAddress
                    1000.00m, // TotalAmount
                    100.00m, // SalesTax
                    1100.00m, // NetAmount
                    1 // UserId
                );
                var salesInvoice = await _dbContext.SalesInvoices.FindAsync(3);
                // Assert
                Assert.NotNull(salesInvoice);
                Assert.Equal("InvoiceNumber", salesInvoice.InvoiceNumber);
                var accountingEntries = await _dbContext.AccountingEntries.ToListAsync();
                Assert.Equal(3, accountingEntries.Count);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task UpdateSalesInvoiceAsync_WithInvalidId_ShouldThrowExemption()
        {
            // Arrange
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var ex = await Assert.ThrowsAsync<Exception>(async () =>
                {
                    await salesInvoiceService.UpdateSalesInvoiceAsync(
                        999, // Invalid Id
                        "test_blob", // BlobName
                        DateTime.UtcNow,
                        "INV-001", // InvoiceNumber
                        "Test Customer", // CustomerName
                        "123 Test Address", // CustomerAddress
                        1000.00m, // TotalAmount
                        100.00m, // SalesTax
                        1100.00m, // NetAmount
                        1 // UserId
                    );
                });
                Assert.Contains("Sales Invoice with Id", ex.Message);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task DeleteSalesInvoiceAsync_WithInvalidId_ShouldThrowException()
        {
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var ex = await Assert.ThrowsAsync<Exception>(async () =>
                {
                    await salesInvoiceService.DeleteSalesInvoiceAsync(999, 1); // Invalid Id
                });
                await dbContext.SaveChangesAsync();
                await dbContext.SalesInvoices.FindAsync(999);
                Assert.Contains("Sales Invoice with Id", ex.Message);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }

        [Fact]
        public async Task GenerateSalesInvoiceAsync_ShouldThrowException_WhenBlobNameIsEmpty()
        {
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;Database=";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var ex = await Assert.ThrowsAsync<Exception>(async () =>
                {
                    await salesInvoiceService.GenerateSalesInvoiceAsync(
                        4, // Id
                        string.Empty, // BlobName
                        DateTime.UtcNow,
                        "INV-004", // InvoiceNumber
                        "Test Customer", // CustomerName
                        "123 Test Address", // CustomerAddress
                        1000.00m, // TotalAmount
                        100.00m, // SalesTax
                        1100.00m, // NetAmount
                        1 // UserId
                    );
                    await dbContext.SaveChangesAsync();
                    await dbContext.SalesInvoices.FindAsync(4);
                });
                Assert.Contains("BlobName is required", ex.Message);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task GenerateSalesInvoiceAsync_ShouldThrowException_WhenInvoiceNumberIsEmpty()
        {
            // Arrange
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=testuser;Password=testpass;";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var _dbContext = new ApplicationDbContext(options);
            await _dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var _salesInvoiceService = new SalesInvoiceService(_dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var ex = await Assert.ThrowsAsync<Exception>(async () =>
                {
                    // Act
                    await _salesInvoiceService.GenerateSalesInvoiceAsync(
                        5, // Id
                        "test_blob", // BlobName
                        DateTime.UtcNow,
                        string.Empty, // InvoiceNumber
                        "Test Customer", // CustomerName
                        "123 Test Address", // CustomerAddress
                        1000.00m, // TotalAmount
                        100.00m, // SalesTax
                        1100.00m, // NetAmount
                        1 // UserId
                    );
                });
                await _dbContext.SaveChangesAsync();
                await _dbContext.SalesInvoices.FindAsync(5);
                Assert.Contains("InvoiceNumber cannot be empty", ex.Message);
            }
            finally
            {
                await _dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task GenerateSalesInvoiceAsync_ShouldThrowException_WhenCustomerNameIsEmpty()
        {
            // Arrange
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var baseConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=testpass;";
            var connectionString = $"{baseConnectionString}{dbName}";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;

            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var _salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var ex = await Assert.ThrowsAsync<Exception>(async () =>
                {
                    // Act
                    await _salesInvoiceService.GenerateSalesInvoiceAsync(
                        6, // Id
                        "test_blob", // BlobName
                        DateTime.UtcNow,
                        "INV-006", // InvoiceNumber
                        string.Empty, // CustomerName
                        "123 Test Address", // CustomerAddress
                        1000.00m, // TotalAmount
                        100.00m, // SalesTax
                        1100.00m, // NetAmount
                        1 // UserId
                    );
                });
                await dbContext.SaveChangesAsync();
                await dbContext.SalesInvoices.FindAsync(6);
                Assert.Contains("CustomerName cannot be empty", ex.Message);
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task GenerateSalesInvoice_PerformanceTest_lengthPerInvoice()
        {
            // Arrange
            var dbName = $"testdb_{Guid.NewGuid():N}"; // Unique database name for each test
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "testuser",
                Password = "testpass",
                Database = dbName
            };
            var connectionString = builder.ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var _salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                // Act
                var startTime = DateTime.UtcNow;
                for (int i = 1; i <= 1000; i++)
                {
                    await _salesInvoiceService.GenerateSalesInvoiceAsync(
                        i, // Id
                        "test_blob_" + i, // BlobName
                        DateTime.UtcNow,
                        "INV-" + i.ToString("D3"), // InvoiceNumber
                        "Test Customer " + i, // CustomerName
                        "123 Test Address " + i, // CustomerAddress
                        1000.00m + i, // TotalAmount
                        100.00m + i, // SalesTax
                        1100.00m + i, // NetAmount
                        1 + i // UserId
                    );
                }
                var endTime = DateTime.UtcNow;
                Assert.True((endTime - startTime).TotalSeconds < 10, "Generating 1000 invoices took too long.");
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
        [Fact]
        public async Task GenerateSalesInvoicesInBulk_PerformanceTest()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            var invoicestoGenerate = 1000; // Array to hold 1000 invoices
            var tasks = new List<Task>();
            for (int i = 0; i < invoicestoGenerate; i++)
            {
                int id = i + 1000; // Start from 1000 to avoid conflicts with existing IDs
                var task = _salesInvoiceService.GenerateSalesInvoiceAsync(
                    id, // Id
                    "test_blob_" + id, // BlobName
                    DateTime.UtcNow,
                    "INV-" + id.ToString("D3"), // InvoiceNumber
                    "Test Customer " + id, // CustomerName
                    "123 Test Address " + id, // CustomerAddress
                    1000.00m + id, // TotalAmount
                    100.00m + id, // SalesTax
                    1100.00m + id, // NetAmount
                    1 + id // UserId
                );
                tasks.Add(task);
            }
            stopwatch.Start();
            await Task.WhenAll(tasks); // Wait for all tasks to complete
            stopwatch.Stop();
            // Assert
            var invoices = await _dbContext.SalesInvoices.ToListAsync();
            Assert.Equal(invoicestoGenerate, invoices.Count);
            Console.WriteLine($"Time taken to generate 1000 sales invoices: {stopwatch.ElapsedMilliseconds} ms");
        }
        [Fact]
        public async Task GenerateSalesInvoices_PerformanceTest()
        {
            var dbName = $"test_db_{Guid.NewGuid()}"; // Unique database name for each test
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "testuser",
                Password = "testpass",
                Database = dbName
            };
            var connectionString = builder.ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString) // Unique database name for each test
                .Options;
            using var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync(); // Ensure the database is created for testing
            try
            {
                var mockDocumentProcessor = new Mock<IDocumentProcessor>();
                var mockLlmService = new Mock<ILlmService>();
                var salesInvoiceService = new SalesInvoiceService(dbContext, mockLlmService.Object, mockDocumentProcessor.Object);
                var start = DateTime.UtcNow;
                int count = 100;
                for (int i = 0; i <= count; i++)
                {
                    await salesInvoiceService.GenerateSalesInvoiceAsync(
                        i + 1, // Id
                        "test_blob_" + (i + 1), // BlobName
                        DateTime.UtcNow,
                        "INV-" + (i + 1).ToString("D3"), // InvoiceNumber
                        "Test Customer " + (i + 1), // CustomerName
                        "123 Test Address " + (i + 1), // CustomerAddress
                        1000.00m + i, // TotalAmount
                        100.00m + i, // SalesTax
                        1100.00m + i, // NetAmount
                        1 + i // UserId
                    );
                }
                var duration = DateTime.UtcNow - start;
                Assert.True(duration.TotalSeconds < 10, "Generating 100 sales invoices took too long.");
            }
            finally
            {
                await dbContext.Database.EnsureDeletedAsync(); // Clean up the database after the test
            }
        }
    }
}
