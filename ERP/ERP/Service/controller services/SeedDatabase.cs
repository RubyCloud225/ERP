using System.Data.Common;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests;

public class SeedDatabaseForTests
{
    private readonly ApplicationDbContext? _dbContext;
    private ApplicationDbContext.User? _seededUser;
    public void Seed(ApplicationDbContext dbContext)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var baseConnectionString = "Host=localhost;Port=5432;Database=ERP_Test;Username=erpuser;Password=erppassword";
        var dbName = $"TestDb_{Guid.NewGuid()}";
        var connectionString = baseConnectionString + dbName;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        // Ensure the database is created
        dbContext = new ApplicationDbContext(options);
        try
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.EnsureDeleted();
            SeedUsers();
            SeedUser();
            SeedSalesInvoices();
            SeedPurchaseInvoices();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during database setup: {ex.Message}");
        }
    }
    public void SeedUsers()
    {
        for (int i = 1; i <= 2000; i++)
        {
            var user = new ApplicationDbContext.User
            {
                Id = Guid.NewGuid(),
                Name = $"Test User {i}",
                Email = $"testuser{i}@erp.com",
                Password = "password123",
                Username = $"testuser{i}"
            };
            if (_dbContext != null && !_dbContext.Users.Any(u => u.Id == user.Id))
            {
                _dbContext.Users.Add(user);
            }
        }
        if (_dbContext != null)
        {
            _dbContext.SaveChanges();
        }
    }
    public void SeedUser()
    {
        var user = new ApplicationDbContext.User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "testuser@erp.com",
            Password = "password123",
            Username = "testuser"
        };
        if (_dbContext != null && !_dbContext.Users.Any())
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        _seededUser = user;
    }
    public void SeedPurchaseInvoices()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var purchaseInvoice = new ApplicationDbContext.PurchaseInvoice
            {
                Id = Guid.NewGuid(),
                UserId = i,
                BlobName = $"purchase_invoice_{i}.pdf",
                PurchaseInvoiceNumber = $"INV-{i:000}",
                Supplier = $"Supplier {i}",
                Description = $"Description for invoice {i}",
                SupplierAddress = $"Address for supplier {i}"
            };
            if (_dbContext != null && !_dbContext.PurchaseInvoices.Any(pi => pi.Id == purchaseInvoice.Id))
            {
                _dbContext.PurchaseInvoices.Add(purchaseInvoice);
            }
        }
        if (_dbContext != null)
        {
            _dbContext.SaveChanges();
        }
    }
    public void SeedSalesInvoices()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var seededUserId = _seededUser != null ? _seededUser.Id : Guid.Empty;
            var user = _dbContext?.Users.FirstOrDefault(u => u.Id == seededUserId) ?? new ApplicationDbContext.User
            {
                Id = Guid.NewGuid(),
                Name = $"Customer User {i}",
                Email = $"customer{i}@erp.com",
                Password = "password123",
                Username = $"customer{i}"
            };

            var salesInvoice = new ApplicationDbContext.SalesInvoice
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                User = user,
                BlobName = $"sales_invoice_{i}.pdf",
                InvoiceDate = DateTime.Now.AddDays(-i),
                InvoiceNumber = $"INV-{i:000}",
                CustomerName = $"Customer {i}",
                CustomerAddress = $"Address for customer {i}",
                TotalAmount = 1000 + i * 10,
                SalesTax = 100 + i,
                NetAmount = 1100 + i * 10
            };
            if (_dbContext != null && !_dbContext.SalesInvoices.Any(si => si.Id == salesInvoice.Id))
            {
                _dbContext.SalesInvoices.Add(salesInvoice);
            }
        }
        _dbContext?.SaveChanges();
    }
}