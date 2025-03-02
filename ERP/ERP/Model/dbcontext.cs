using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ERP.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<BankPayment> BankPayments { get; set; }
        public DbSet<BankReceipt> BankReceipts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<PropertyLog> PropertyLogs { get; set; }

        public class PropertyLog
        {
            public int Id { get; set; }
            public required string PropertyName { get; set; }
            public required string PropertyValue { get; set; }
            public required string PropertyType { get; set; }
            public DateTime LoggedAt { get; set; }
            public int? UserId { get; set; }
        }

        public class PurchaseInvoice
        {
            public int Id { get; set; }
            public required string Supplier { get; set; }
            public decimal Amount { get; set; }
            public DateTime InvoiceDate { get; set; }
        }

        public class SalesInvoice
        {
            public int Id { get; set; }
            public required string Customer { get; set; }
            public decimal Amount { get; set; }
            public DateTime InvoiceDate { get; set; }
        }

        public class BankPayment
        {
            public int Id { get; set; }
            public required string Payee { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        public class BankReceipt
        {
            public int Id { get; set; }
            public required string Payer { get; set; }
            public decimal Amount { get; set; }
            public DateTime ReceiptDate { get; set; }
        }

        public class JournalEntry
        {
            public int Id { get; set; }
            public required string Description { get; set; }
            public decimal Amount { get; set; }
            public DateTime EntryDate { get; set; }
        }
    }
}
