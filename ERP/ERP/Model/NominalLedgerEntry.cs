using System;

namespace ERP.Model
{
    public class NominalLedgerEntry
    {
        public int Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; } = string.Empty;

        public NominalLedgerEntry()
        {
        }
    }
}
