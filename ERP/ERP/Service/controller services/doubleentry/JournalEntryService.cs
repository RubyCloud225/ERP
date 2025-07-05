using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public interface IJournalEntryService
    {
        Task<AccountingEntry> CreateJournalEntryAsync(CreateJournalEntryDto request, Guid? userId);
        Task<AccountingEntry> AmendJournalEntryAsync(Guid id, CreateJournalEntryDto request, Guid? userId);
        Task DeleteJournalEntryAsync(Guid id);
        Task<AccountingEntry?> GetJournalEntryByIdAsync(Guid id);
        Task<AccountingEntry?> GetJournalEntryByUserIdAsync(Guid userId);
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

        public async Task<AccountingEntry> AmendJournalEntryAsync(Guid id, CreateJournalEntryDto request, Guid? userId)
        {
            if (request == null || request.Lines == null || !request.Lines.Any())
            {
                throw new ArgumentException("Journal entry request must have at least one line");
            }

            var existingEntry = await _dbContext.AccountingEntries.FindAsync(id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException($"Journal entry with id {id} not found");
            }

            // Update basic properties
            existingEntry.EntryDate = request.EntryDate;
            existingEntry.Description = request.Description;
            existingEntry.UserId = userId;

            // Remove existing lines
            var existingLines = _dbContext.AccountingEntryLines.Where(l => l.AccountingEntryId == id);
            _dbContext.AccountingEntryLines.RemoveRange(existingLines);

            // Add new lines
            existingEntry.Lines = new List<AccountingEntryLine>();
            foreach (var line in request.Lines)
            {
                existingEntry.Lines.Add(new AccountingEntryLine
                {
                    Id = Guid.NewGuid(),
                    Description = line.Description,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    NominalAccountId = line.NominalAccountId,
                    AccountingEntryId = id
                });
            }

            existingEntry.TotalDebit = existingEntry.Lines.Sum(l => l.Debit);
            existingEntry.TotalCredit = existingEntry.Lines.Sum(l => l.Credit);

            if (existingEntry.TotalDebit != existingEntry.TotalCredit)
            {
                throw new InvalidOperationException("Total debit must equal total credit in journal entry");
            }

            _dbContext.AccountingEntries.Update(existingEntry);
            await _dbContext.SaveChangesAsync();

            return existingEntry;
        }

        public async Task DeleteJournalEntryAsync(Guid id)
        {
            var existingEntry = await _dbContext.AccountingEntries.FindAsync(id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException($"Journal entry with id {id} not found");
            }

            // Remove related lines first
            var existingLines = _dbContext.AccountingEntryLines.Where(l => l.AccountingEntryId == id);
            _dbContext.AccountingEntryLines.RemoveRange(existingLines);

            _dbContext.AccountingEntries.Remove(existingEntry);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<AccountingEntry?> GetJournalEntryByIdAsync(Guid id)
        {
            var existingEntry = await _dbContext.AccountingEntries
                .Include(e => e.Lines)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEntry == null)
            {
                throw new KeyNotFoundException($"Journal entry with id {id} not found");
            }

            return existingEntry;
        }
        public async Task<AccountingEntry?> GetJournalEntryByUserIdAsync(Guid userId)
        {
            var existingEntry = await _dbContext.AccountingEntries
                .Include(e => e.Lines)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (existingEntry == null)
            {
                throw new KeyNotFoundException($"Journal entry with user {userId} not found");
            }

            return existingEntry;
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
