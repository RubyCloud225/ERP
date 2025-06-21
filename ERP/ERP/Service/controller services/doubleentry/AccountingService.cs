using ERP.Model;

namespace ERP.Service
{
    public interface IAccountingService
    {
        Task CreateAccountingForSalesInvoiceAsync(ApplicationDbContext.SalesInvoice salesInvoice, int receivableNominalId, int revenueNominalId, int? taxNominalId, int? userId);
        // Add methods for creating accounting entries for purchase invoices and bank transactions
    }
    public class AccountingService : IAccountingService
    {
        private readonly ApplicationDbContext _dbContext;
        public AccountingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateAccountingForSalesInvoiceAsync(ApplicationDbContext.SalesInvoice salesInvoice, int receivableNominalId, int revenueNominalId, int? taxNominalId, int? userId)
        {
            // Create accounting entries for the sales invoice
            if (salesInvoice == null)
            {
                throw new ArgumentNullException(nameof(salesInvoice), "Sales invoice cannot be null");
            }
            if (receivableNominalId <= 0 || revenueNominalId <= 0 || (taxNominalId.HasValue && taxNominalId.Value <= 0))
            {
                throw new ArgumentException("Invalid nominal account IDs provided");
            }
            var receivableNominal = await _dbContext.NominalAccounts.FindAsync(receivableNominalId) ?? throw new InvalidOperationException("Receivable nominal account not found");
            var revenueNominal = await _dbContext.NominalAccounts.FindAsync(revenueNominalId) ?? throw new InvalidOperationException("Revenue nominal account not found");
            ApplicationDbContext.NominalAccount? taxNominal = null;
            if (receivableNominal == null || revenueNominal == null || taxNominal == null)
            {
                throw new InvalidOperationException("One or more nominal accounts not found");
            }
            var entryDescription = $"Sales Invoice #{salesInvoice.InvoiceNumber} for {salesInvoice.CustomerName}";
            var entryDate = salesInvoice.InvoiceDate;

            var accountingEntry = new ApplicationDbContext.AccountingEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = entryDate,
                Description = entryDescription,
                SalesInvoiceId = salesInvoice.Id,
                UserId = userId.HasValue ? Guid.Parse(userId.Value.ToString()) : (Guid?)null,
                Lines = new List<ApplicationDbContext.AccountingEntryLine>()
            };
            accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
            {
                Id = Guid.NewGuid(),
                NominalAccountId = revenueNominal.Id,
                Debit = salesInvoice.TotalAmount,
                Credit = 0,
                Description = $"Accounts Receivable for {salesInvoice.CustomerName}"
            });
            accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
            {
                Id = Guid.NewGuid(),
                NominalAccountId = receivableNominal.Id,
                Debit = 0,
                Credit = salesInvoice.TotalAmount,
                Description = $"Revenue from Sales Invoice #{salesInvoice.InvoiceNumber}"
            });
            if (taxNominal != null)
            {
                accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    NominalAccountId = taxNominal.Id,
                    Debit = salesInvoice.SalesTax,
                    Credit = 0,
                    Description = $"Sales Tax for Invoice #{salesInvoice.InvoiceNumber}"
                });
            }
            accountingEntry.TotalDebit = accountingEntry.Lines.Sum(line => line.Debit);
            accountingEntry.TotalCredit = accountingEntry.Lines.Sum(line => line.Credit);
            if (accountingEntry.TotalDebit != accountingEntry.TotalCredit)
            {
                throw new InvalidOperationException("Total debit must equal total credit in accounting entry");
            }
            // Add the accounting entry to the database
            _dbContext.AccountingEntries.Add(accountingEntry);
            await _dbContext.SaveChangesAsync();
        }
        // this method creates accounting entries for a purchase invoice
        // this methiod creates for bank transactions
    }
}