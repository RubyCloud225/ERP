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
        public DbSet<Document> Documents { get; set; }
        public DbSet<LlmResponse> LlmResponses { get; set; }
        public DbSet<AccountingEntry> AccountingEntries { get; set; }

        public class LlmResponse
        {
            public int Id { get; set; }
            public required string Response { get; set; }
        }

        public class Document
        {
            public int Id { get; set; }
            public required string FileName { get; set; }
            public required string FileUrl { get; set; }
            public DateTime UploadedAt { get; set; }
            public required string Amendments { get; set; }
            public required string DocumentType { get; set; }
        }

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
            public required string PurchaseInvoiceNumber { get; set; }
            public required string Supplier { get; set; }
            public decimal Amount { get; set; }
            public DateTime PurchaseInvoiceDate { get; set; }
            public required string DocumentType { get; set; }
            public required string Response { get; set; }
            public required string NominalAccount { get; set; }
            public required string ExpenseAccount { get; set; }
        }

        public class AccountingEntry
        {
            public int Id { get; set; }
            public int ? PurchaseInvoiceId { get; set; }
            public required string Account { get; set; }
            public decimal Amount { get; set; }
            public bool IsDebit { get; set; }
            public DateTime EntryDate { get; set; }
            public required PurchaseInvoice PurchaseInvoice { get; set; }
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
