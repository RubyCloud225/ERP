using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service
{
    public interface INominalLedgerService
    {
        IEnumerable<ApplicationDbContext.NominalLedgerEntry> GetEntries(DateTime startDate, DateTime endDate);
    }
    public class NominalLedgerService : INominalLedgerService
    {
        private readonly ApplicationDbContext _dbContext;

        public NominalLedgerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ApplicationDbContext.NominalLedgerEntry> GetEntries(DateTime startDate, DateTime endDate)
        {
            var entries = _dbContext.AccountingEntries
                .Include(ae => ae.Lines)
                .Where(ae => ae.EntryDate >= startDate && ae.EntryDate <= endDate)
                .ToList();

            var ledgerEntries = new List<ApplicationDbContext.NominalLedgerEntry>();

            foreach (var entry in entries)
            {
                foreach (var line in entry.Lines)
                {
                    ledgerEntries.Add(new ApplicationDbContext.NominalLedgerEntry
                    {
                        AccountCode = line.NominalAccount?.Code ?? "Unknown",
                        EntryDate = entry.EntryDate,
                        Debit = line.Debit,
                        Credit = line.Credit,
                        Description = line.Description
                    });
                }
            }

            return ledgerEntries;
        }
    }
    

}

