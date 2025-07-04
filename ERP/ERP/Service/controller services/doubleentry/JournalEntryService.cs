using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Service
{
    public interface IJournalEntryService
    {
        Task<AccountingEntry> CreateJournalEntryAsync(CreateJournalEntryDto request, Guid? userId);
    }

    public class JournalEntryService : IJournalEntryService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountingService _accountingService;

        public JournalEntryService(ApplicationDbContext dbContext, IAccountingService accountingService)
        {
            _dbContext = dbContext;
            _accountingService = accountingService;
        }

        public async Task<AccountingEntry> CreateJournalEntryAsync(CreateJournalEntryDto request, Guid? userId)
        {
            if (request == null || request.Lines == null || !request.Lines.Any())
            {
                throw new ArgumentException("Journal entry request must have at least one line");
            }

            var accountingEntry = new AccountingEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = request.EntryDate,
                Description = request.Description,
                UserId = userId,
                Lines = new List<AccountingEntryLine>()
            };

            foreach (var line in request.Lines)
            {
                accountingEntry.Lines.Add(new AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    Description = line.Description,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    NominalAccountId = line.NominalAccountId
                });
            }

            accountingEntry.TotalDebit = accountingEntry.Lines.Sum(l => l.Debit);
            accountingEntry.TotalCredit = accountingEntry.Lines.Sum(l => l.Credit);

            if (accountingEntry.TotalDebit != accountingEntry.TotalCredit)
            {
                throw new InvalidOperationException("Total debit must equal total credit in journal entry");
            }

            _dbContext.AccountingEntries.Add(accountingEntry);
            await _dbContext.SaveChangesAsync();

            return accountingEntry;
        }
    }

    public class CreateJournalEntryDto
    {
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public List<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
    }

    public class JournalEntryLineDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Guid NominalAccountId { get; set; }
    }
}
