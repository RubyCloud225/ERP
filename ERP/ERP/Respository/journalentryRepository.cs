using System.ComponentModel;
using ERP.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ERP.Repository
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JournalEntryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationDbContext.Result<Guid>> AddJournalEntryAsync(ApplicationDbContext.JournalEntryDto dto)
        {
            if (dto.Lines.Count < 2 || dto.Lines.Count > 50)
                return ApplicationDbContext.Result<Guid>.Fail("Invalid number of lines"); // return error

            var accountCodes = await _dbContext.AccountingEntries
            .Select(a => a.Account)
            .ToListAsync();

            foreach (var line in dto.Lines)
            {
                if (!accountCodes.Contains(line.AccountName))
                    return ApplicationDbContext.Result<Guid>.Fail($"Invalid account code: {line.AccountName}"); // return error
            }
            var totalDebit = dto.Lines.Sum(line => line.Debit);
            var totalCredit = dto.Lines.Sum(line => line.Credit);
            if (totalDebit != totalCredit)
            {
                return ApplicationDbContext.Result<Guid>.Fail("Total debits must equal total credits"); // return error
            }
            var journalEntry = new ApplicationDbContext.JournalEntry
            {
                Description = dto.Description,
                EntryDate = dto.EntryDate,
                UserId = dto.UserId,
                Lines = dto.Lines.Select(line => new ApplicationDbContext.JournalEntryLine
                {
                    AccountName = line.AccountName,
                    Debit = line.Debit,
                    Credit = line.Credit
                }).ToList()
            };
            _dbContext.JournalEntries.Add(journalEntry);
            await _dbContext.SaveChangesAsync();
            return ApplicationDbContext.Result<Guid>.SuccessResult(journalEntry.Id);
        }
        public async Task<ApplicationDbContext.Result<Guid>> DeleteJournalEntryAsync(Guid id)
        {
            var journalEntry = await _dbContext.JournalEntries.FindAsync(id);
            if (journalEntry == null)
            {
                return ApplicationDbContext.Result<Guid>.Fail("no journal entry found");
            }
        }
    }

    public interface IJournalEntryRepository
    {
        Task<ApplicationDbContext.Result<Guid>> AddJournalEntryAsync(ApplicationDbContext.JournalEntryDto dto);
    }
}