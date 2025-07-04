using ERP.Model;

namespace ERP.Service
{
    public interface IAccountingService
    {
        Task CreateAccountingForSalesInvoiceAsync(ApplicationDbContext.SalesInvoice salesInvoice, int receivableNominalId, int revenueNominalId, int? taxNominalId, int? userId);
        Task CreateAccountingForPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice purchaseInvoice, Guid payableNominalId, Guid expenseNominalId, Guid? taxNominalId, int? userId);
        Task CreateAccountingForBankTransactionAsync(ApplicationDbContext.BankTransaction bankTransaction, Guid nominalAccountId, Guid? userId);
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
        public async Task CreateAccountingForPurchaseInvoiceAsync(ApplicationDbContext.PurchaseInvoice purchaseInvoice, Guid payableNominalId, Guid expenseNominalId, Guid? taxNominalId, int? userId)
        {
            if (purchaseInvoice == null)
            {
                throw new ArgumentNullException(nameof(purchaseInvoice), "Purchase invoice cannot be null");
            }
            if (payableNominalId == Guid.Empty || expenseNominalId == Guid.Empty || (taxNominalId.HasValue && taxNominalId == Guid.Empty))
            {
                throw new ArgumentException("Invalid nominal account IDs provided");
            }
            var payableNominal = await _dbContext.NominalAccounts.FindAsync(payableNominalId) ?? throw new InvalidOperationException("Payable nominal account not found");
            var expenseNominal = await _dbContext.NominalAccounts.FindAsync(expenseNominalId) ?? throw new InvalidOperationException("Expense nominal account not found");
            ApplicationDbContext.NominalAccount? taxNominal = null;
            if (taxNominalId.HasValue)
            {
                taxNominal = await _dbContext.NominalAccounts.FindAsync(taxNominalId.Value);
            }
            var entryDescription = $"Purchase Invoice #{purchaseInvoice.PurchaseInvoiceNumber} from {purchaseInvoice.Supplier}";
            var entryDate = purchaseInvoice.PurchaseInvoiceDate;

            var accountingEntry = new ApplicationDbContext.AccountingEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = entryDate,
                Description = entryDescription,
                UserId = userId.HasValue ? Guid.Parse(userId.Value.ToString()) : (Guid?)null,
                Lines = new List<ApplicationDbContext.AccountingEntryLine>()
            };
            accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
            {
                Id = Guid.NewGuid(),
                NominalAccountId = expenseNominal.Id,
                Debit = purchaseInvoice.NetAmount,
                Credit = 0,
                Description = $"Expense for {purchaseInvoice.Supplier}"
            });
            accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
            {
                Id = Guid.NewGuid(),
                NominalAccountId = payableNominal.Id,
                Debit = 0,
                Credit = purchaseInvoice.NetAmount,
                Description = $"Payable for Purchase Invoice #{purchaseInvoice.PurchaseInvoiceNumber}"
            });
            if (taxNominal != null)
            {
                accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    NominalAccountId = taxNominal.Id,
                    Debit = purchaseInvoice.TaxAmount,
                    Credit = 0,
                    Description = $"Tax for Purchase Invoice #{purchaseInvoice.PurchaseInvoiceNumber}"
                });
            }
            accountingEntry.TotalDebit = accountingEntry.Lines.Sum(line => line.Debit);
            accountingEntry.TotalCredit = accountingEntry.Lines.Sum(line => line.Credit);
            if (accountingEntry.TotalDebit != accountingEntry.TotalCredit)
            {
                throw new InvalidOperationException("Total debit must equal total credit in accounting entry");
            }
            _dbContext.AccountingEntries.Add(accountingEntry);
            await _dbContext.SaveChangesAsync();
        }
        // this method creates for bank transactions
        public async Task CreateAccountingForBankTransactionAsync(ApplicationDbContext.BankTransaction bankTransaction, Guid nominalAccountId, Guid? userId)
        {
            if (bankTransaction == null)
            {
                throw new ArgumentNullException(nameof(bankTransaction), "Bank transaction cannot be null");
            }
            if (nominalAccountId == Guid.Empty)
            {
                throw new ArgumentException("Invalid nominal account ID provided");
            }
            var nominalAccount = await _dbContext.NominalAccounts.FindAsync(nominalAccountId) ?? throw new InvalidOperationException("Nominal account not found");

            var entryDescription = $"Bank Transaction #{bankTransaction.TransactionNumber} on {bankTransaction.TransactionDate:yyyy-MM-dd}";
            var entryDate = bankTransaction.TransactionDate;

            var accountingEntry = new ApplicationDbContext.AccountingEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = entryDate,
                Description = entryDescription,
                UserId = userId.HasValue ? Guid.Parse(userId.Value.ToString()) : (Guid?)null,
                Lines = new List<ApplicationDbContext.AccountingEntryLine>()
            };

            // Debit or credit depends on transaction type
            if (bankTransaction.TransactionType == ApplicationDbContext.TransactionType.Debit)
            {
                accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    NominalAccountId = nominalAccount.Id,
                    Debit = bankTransaction.Amount,
                    Credit = 0,
                    Description = $"Debit for bank transaction {bankTransaction.TransactionNumber}"
                });
            }
            else if (bankTransaction.TransactionType == ApplicationDbContext.TransactionType.Credit)
            {
                accountingEntry.Lines.Add(new ApplicationDbContext.AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    NominalAccountId = nominalAccount.Id,
                    Debit = 0,
                    Credit = bankTransaction.Amount,
                    Description = $"Credit for bank transaction {bankTransaction.TransactionNumber}"
                });
            }
            else
            {
                throw new InvalidOperationException("Unknown transaction type");
            }

            accountingEntry.TotalDebit = accountingEntry.Lines.Sum(line => line.Debit);
            accountingEntry.TotalCredit = accountingEntry.Lines.Sum(line => line.Credit);
            if (accountingEntry.TotalDebit != accountingEntry.TotalCredit)
            {
                throw new InvalidOperationException("Total debit must equal total credit in accounting entry");
            }

            _dbContext.AccountingEntries.Add(accountingEntry);
            await _dbContext.SaveChangesAsync();
        }
    }
}
