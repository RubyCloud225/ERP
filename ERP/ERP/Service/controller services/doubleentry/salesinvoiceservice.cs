using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using ERP.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ERP.Service
{
    public interface ISalesInvoiceService
    {
        Task<ApplicationDbContext.SalesInvoice> GenerateSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null, Guid? customerId = null);
        Task<ApplicationDbContext.SalesInvoice> AmendSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null, Guid? customerId = null);
        Task DeleteSalesInvoiceAsync(Guid id);
        Task<ApplicationDbContext.SalesInvoice?> GetSalesInvoiceByIdAsync(Guid id);
        Task<ApplicationDbContext.SalesInvoice?> GetSalesInvoiceByUserIdAsync(Guid userId);
        Task<decimal> GetSalesTaxReturnForQuarterAsync(int year, int quarter);
        Task<decimal> GetTotalSalesAmountForCustomerAsync(Guid customerId);
        Task<SalesGrowthRateDto> GetSalesGrowthRateAsync();
    }
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AccountingService _accountingService;
        private readonly INominalAccountResolutionService _nominalAccountResolutionService;
        private readonly ILlmService _llmService;
        public SalesInvoiceService(ApplicationDbContext dbContext, ILlmService llmService, AccountingService accountingService, INominalAccountResolutionService nominalAccountResolutionService)
        {
            _dbContext = dbContext;
            _accountingService = accountingService;
            _nominalAccountResolutionService = nominalAccountResolutionService;
            _llmService = llmService;
        }
        // use LLM to create a sales invoice
        // Generates Sales invoice based on user input and processes it through LLM for further processing
        // saves it, and creates the corresponding accounting entries

        public async Task<SalesGrowthRateDto> GetSalesGrowthRateAsync()
        {
            // Example implementation: Calculate monthly sales totals and compute growth rate
            var salesData = await _dbContext.SalesInvoices
                .Where(si => si.InvoiceDate >= DateTime.UtcNow.AddYears(-1)) // Get last year's data
                .GroupBy(si => new { Year = si.InvoiceDate.Year, Month = si.InvoiceDate.Month })
                .Select(g => new
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                    TotalSales = g.Sum(si => si.TotalAmount)
                })
                .OrderBy(x => x.YearMonth)
                .ToListAsync();

            var labels = salesData.Select(x => x.YearMonth).ToArray();
            var totals = salesData.Select(x => x.TotalSales).ToArray();

            // Calculate growth rate as percentage change month over month
            var growthRates = new decimal[totals.Length];
            growthRates[0] = 0;
            for (int i = 1; i < totals.Length; i++)
            {
                if (totals[i - 1] == 0)
                {
                    growthRates[i] = 0;
                }
                else
                {
                    growthRates[i] = (totals[i] - totals[i - 1]) / totals[i - 1] * 100;
                }
            }

            return new SalesGrowthRateDto
            {
                Labels = labels,
                Data = growthRates
            };
        }

        public async Task<ApplicationDbContext.SalesInvoice> GenerateSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null, Guid? customerId = null)
        {
            if (request.LineItems == null || request.LineItems.Count == 0)
            {
                throw new ArgumentException("Line items cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new ArgumentException("Customer name cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
            {
                throw new ArgumentException("Customer address cannot be null or empty");
            }
            if (request.InvoiceDate == DateTime.MinValue)
            {
                throw new ArgumentException("Invoice date cannot be the default value");
            }

            var existingInvoice = await _dbContext.SalesInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingInvoice == null)
            {
                throw new KeyNotFoundException($"Sales invoice with id {id} not found");
            }

            // Update properties
            existingInvoice.InvoiceDate = request.InvoiceDate;
            // InvoiceNumber is not part of GenerateSalesInvoiceDto, so do not update it here
            existingInvoice.CustomerName = request.CustomerName;
            existingInvoice.CustomerAddress = request.CustomerAddress;
            existingInvoice.DueDate = request.DueDate;
            existingInvoice.BlobName = blobName;
            existingInvoice.UserId = userId ?? existingInvoice.UserId;
            existingInvoice.CustomerId = customerId ?? existingInvoice.CustomerId;

            // Remove existing lines
            _dbContext.SalesInvoiceLines.RemoveRange(existingInvoice.Lines);

            // Add new lines
            var newLines = new List<ApplicationDbContext.SalesInvoiceLine>();
            foreach (var line in request.LineItems)
            {
                newLines.Add(new ApplicationDbContext.SalesInvoiceLine
                {
                    Description = line.Description,
                    quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = line.TotalPrice,
                    // NominalAccountId is not part of SalesInvoiceLineDto, so set to null or resolve if needed
                    NominalAccountId = null,
                    SalesInvoiceId = id
                });
            }
            existingInvoice.Lines = newLines;

            _dbContext.SalesInvoiceLines.AddRange(newLines);

            _dbContext.SalesInvoices.Update(existingInvoice);
            await _dbContext.SaveChangesAsync();

            // Removed call to non-existent accounting service method

            return existingInvoice;
        }

        public async Task<decimal> GetTotalSalesAmountForCustomerAsync(Guid customerId)
        {
            return await _dbContext.SalesInvoices
                .Where(si => si.CustomerId == customerId)
                .SumAsync(si => (decimal?)si.TotalAmount) ?? 0m;
        }

        public async Task<ApplicationDbContext.SalesInvoice?> GetSalesInvoiceByIdAsync(Guid id)
        {
            return await _dbContext.SalesInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<ApplicationDbContext.SalesInvoice?> GetSalesInvoiceByUserIdAsync(Guid userId)
        {
            return await _dbContext.SalesInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

        public async Task<ApplicationDbContext.SalesInvoice> AmendSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId = null)
        {
            if (request.LineItems == null || request.LineItems.Count == 0)
            {
                throw new ArgumentException("Line items cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new ArgumentException("Customer name cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
            {
                throw new ArgumentException("Customer address cannot be null or empty");
            }
            if (request.InvoiceDate == DateTime.MinValue)
            {
                throw new ArgumentException("Invoice date cannot be the default value");
            }

            var existingInvoice = await _dbContext.SalesInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingInvoice == null)
            {
                throw new KeyNotFoundException($"Sales invoice with id {id} not found");
            }

            // Update properties
            existingInvoice.InvoiceDate = request.InvoiceDate;
            // InvoiceNumber is not part of GenerateSalesInvoiceDto, so do not update it here
            existingInvoice.CustomerName = request.CustomerName;
            existingInvoice.CustomerAddress = request.CustomerAddress;
            existingInvoice.DueDate = request.DueDate;
            existingInvoice.BlobName = blobName;
            existingInvoice.UserId = userId ?? existingInvoice.UserId;

            // Remove existing lines
            _dbContext.SalesInvoiceLines.RemoveRange(existingInvoice.Lines);

            // Add new lines
            var newLines = new List<ApplicationDbContext.SalesInvoiceLine>();
            foreach (var line in request.LineItems)
            {
                newLines.Add(new ApplicationDbContext.SalesInvoiceLine
                {
                    Description = line.Description,
                    quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = line.TotalPrice,
                    // NominalAccountId is not part of SalesInvoiceLineDto, so set to null or resolve if needed
                    NominalAccountId = null,
                    SalesInvoiceId = id
                });
            }
            existingInvoice.Lines = newLines;

            _dbContext.SalesInvoiceLines.AddRange(newLines);

            _dbContext.SalesInvoices.Update(existingInvoice);
            await _dbContext.SaveChangesAsync();

            // Removed call to non-existent accounting service method

            return existingInvoice;
        }

        public async Task DeleteSalesInvoiceAsync(Guid id)
        {
            var existingInvoice = await _dbContext.SalesInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existingInvoice == null)
            {
                throw new KeyNotFoundException($"Sales invoice with id {id} not found");
            }

            // Remove related lines first
            _dbContext.SalesInvoiceLines.RemoveRange(existingInvoice.Lines);

            _dbContext.SalesInvoices.Remove(existingInvoice);
            await _dbContext.SaveChangesAsync();

            // Removed call to non-existent accounting service method
        }

        public async Task<decimal> GetSalesTaxReturnForQuarterAsync(int year, int quarter)
        {
            var startDate = new DateTime(year, (quarter - 1) * 3 + 1, 1);
            var endDate = startDate.AddMonths(3);

            var totalSalesTax = await _dbContext.SalesInvoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate < endDate)
                .SumAsync(i => (decimal?)i.SalesTax) ?? 0m;

            return totalSalesTax;
        }

        Task<ApplicationDbContext.SalesInvoice> ISalesInvoiceService.GenerateSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId, Guid? customerId)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationDbContext.SalesInvoice> ISalesInvoiceService.AmendSalesInvoiceAsync(Guid id, ApplicationDbContext.GenerateSalesInvoiceDto request, string blobName, Guid? userId, Guid? customerId)
        {
            throw new NotImplementedException();
        }

        Task ISalesInvoiceService.DeleteSalesInvoiceAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationDbContext.SalesInvoice?> ISalesInvoiceService.GetSalesInvoiceByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationDbContext.SalesInvoice?> ISalesInvoiceService.GetSalesInvoiceByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        Task<decimal> ISalesInvoiceService.GetSalesTaxReturnForQuarterAsync(int year, int quarter)
        {
            throw new NotImplementedException();
        }

        Task<decimal> ISalesInvoiceService.GetTotalSalesAmountForCustomerAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
