using Humanizer;
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
        public DbSet<User> Users { get; set; }
        public DbSet<UserDto> userDtos { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<PropertyLog> PropertyLogs { get; set; }
        public DbSet<DocumentRecord> DocumentRecords { get; set; }
        public DbSet<LlmResponse> LlmResponses { get; set; }
        public DbSet<AccountingEntry> AccountingEntries { get; set; }
        public DbSet<loginDto> loginDtos { get; set; }
        public DbSet<PurchaseInvoiceDto> purchaseInvoiceDtos { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankStatement> BankStatements { get; set; }
        public DbSet<BankPayment> BankPayments { get; set; }
        public DbSet<BankReceipt> BankReceipts { get; set; }


        public class BankAccount
        {
            public int Id { get; set; }
            public required string AccountNumber { get; set; }
            public required string AccountName { get; set; }
            public required string BankName { get; set; }
            public decimal Balance { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }
        public class BankStatement
        {
            public int Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime StatementDate { get; set; }
            public required decimal Balance { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }
        public class BankPayment
        {
            public int Id { get; set; }
            public required string Payee { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }
        public class BankReceipt
        {
            public int Id { get; set; }
            public required string Payer { get; set; }
            public decimal Amount { get; set; }
            public DateTime ReceiptDate { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class PurchaseInvoiceDto
        {
            public int Id { get; set; }
            public required string PurchaseInvoiceNumber { get; set; }
            public required string Supplier { get; set; }
            public decimal NetAmount { get; set; }
            public decimal GrossAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public required string Description { get; set; }
            public required string SupplierAddress { get; set; }
            public DateTime PurchaseInvoiceDate { get; set; }
            public required string DocumentType { get; set; }
            public required string Response { get; set; }
            public required string NominalAccount { get; set; }
            public required string ExpenseAccount { get; set; }
        }

        public class loginDto
        {
            public int Id { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Password { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required string Username { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class LlmResponse
        {
            public int Id { get; set; }
            public required string Response { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class DocumentRecord
        {
            public int Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime CreatedAt { get; set; }
            public required string DocumentContent { get; set; }
            public required string DocumentType { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class PropertyLog
        {
            public int Id { get; set; }
            public required string PropertyName { get; set; }
            public required string PropertyValue { get; set; }
            public required string PropertyType { get; set; }
            public DateTime LoggedAt { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class PurchaseInvoice
        {
            public int Id { get; set; }
            public required string BlobName { get; set; }
            public required string PurchaseInvoiceNumber { get; set; }
            public required string Supplier { get; set; }
            public decimal NetAmount { get; set; }
            public decimal GrossAmount { get; set; }
            public decimal TaxAmount { get; set; } // Ensure this property is included
            public required string Description { get; set; }
            public required string SupplierAddress { get; set; }
            public DateTime PurchaseInvoiceDate { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class AccountingEntry
        {
            public int Id { get; set; }
            public int? PurchaseInvoiceId { get; set; }
            public required string Account { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public DateTime EntryDate { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }

        public class SalesInvoice
        {
            public int Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime InvoiceDate { get; set; }
            public required string InvoiceNumber { get; set; }
            public required string CustomerName { get; set; }
            public required string CustomerAddress { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTax { get; set; }
            public decimal TotalAmount { get; set; }
            public required int UserId { get; set; }
            public required User User { get; set; } // Assuming UserId is a string, adjust as necessary

        }
        public class UpdateSalesInvoiceRequest
        {
            public int Id { get; set; }
            public required string InvoiceNumber { get; set; }
        }

        public class GenerateSalesInvoiceRequest
        {
            public int Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime InvoiceDate { get; set; }
            public required string InvoiceNumber { get; set; }
            public required string CustomerName { get; set; }
            public required string CustomerAddress { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal SalesTax { get; set; }
            public decimal NetAmount { get; set; }
            public required int UserId { get; set; }
        }

        public class JournalEntry
        {
            public int Id { get; set; }
            public required string Description { get; set; }
            public decimal Amount { get; set; }
            public DateTime EntryDate { get; set; }
            public int? UserId { get; set; }
            public User? User { get; set; }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.PurchaseInvoiceNumber)
                .IsRequired();
            modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.BlobName)
                .IsRequired();
            modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.InvoiceNumber)
                .IsRequired();

            modelBuilder.Entity<SalesInvoice>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseInvoice>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentRecord>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LlmResponse>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountingEntry>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankAccount>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankStatement>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankPayment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankReceipt>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JournalEntry>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
