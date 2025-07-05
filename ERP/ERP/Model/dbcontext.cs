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

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankStatement> BankStatements { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceLine> PurchaseInvoiceLines { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceLine> SalesInvoiceLines { get; set; }
        public DbSet<NominalAccount> NominalAccounts { get; set; }
        public DbSet<AccountingEntry> AccountingEntries { get; set; }
        public DbSet<LlmRequest> LlmRequests { get; set; }
        public DbSet<LlmResponse> LlmResponses { get; set; }
        public DbSet<DocumentRecord> DocumentRecords { get; set; }
        public DbSet<PropertyLog> PropertyLogs { get; set; }
        public DbSet<ParsedBankStatementDto> ParsedBankStatements { get; set; }
        public DbSet<ParsedBankTransactionsDto> ParsedBankTransactions { get; set; }
        public DbSet<ParsedPurchaseInvoiceDto> ParsedPurchaseInvoices { get; set; }
        public DbSet<parsedSalesInvoiceDto> ParsedSalesInvoices { get; set; }
        public DbSet<parsedSalesInvoiceLineDto>  ParsedSalesInvoiceLine { get; set; }
        public DbSet<NominalAccountSuggestionDto> NominalAccountSuggestions { get; set; }
        public DbSet<AccountingEntryLine> AccountingEntryLines { get; set; }

        //-------------------- User Schema ---------------------//
        public class User
        {
            public Guid Id { get; set; }
            public required string Name { get; set; }
            public required string Username { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
            public string? CompanyName { get; set; }
            public string? CountryOfOrigin { get; set; }
            public string? Address { get; set; }
            public int? NumberOfRoles { get; set; }
            public string? CompanyNumber { get; set; }
            public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
            public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
            public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
            public ICollection<NominalAccount> NominalAccounts { get; set; } = new List<NominalAccount>();
            public ICollection<AccountingEntry> AccountingEntries { get; set; } = new List<AccountingEntry>();
            public ICollection<DocumentRecord> DocumentRecords { get; set; } = new List<DocumentRecord>();
        }
        public class loginDto
        {
            public Guid Id { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class UserDto
        {
            public Guid Id { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Password { get; set; }
        }

        public class UserSignUpDto
        {
            public string? CompanyName { get; set; }
            public string? CountryOfOrigin { get; set; }
            public string? Address { get; set; }
            public int? NumberOfRoles { get; set; }
            public string? CompanyNumber { get; set; }
            public string? Name { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        //-------------------- Bank Schema ---------------------//

        public class BankAccount
        {
            public Guid Id { get; set; }
            public required string AccountNumber { get; set; }
            public required string AccountName { get; set; }
            public required string BankName { get; set; }
            public decimal Balance { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
        }
        public class BankStatement
        {
            public Guid Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime StatementStartDate { get; set; }
            public DateTime StatementEndDate { get; set; }
            public decimal StatementNumber { get; set; } // Ensure this property is included
            public required decimal OpeningBalance { get; set; }
            public required decimal ClosingBalance { get; set; }
            public Guid? BankAccountId { get; set; }
            public BankAccount? BankAccount { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool Reconciled { get; set; } = false;
            public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();

            // Removed invalid implicit operator definition
        }
        public enum TransactionType
        {
            Debit,
            Credit
        }
        public class BankTransaction
        {
            public Guid Id { get; set; }
            public required string TransactionNumber { get; set; }
            public required DateTime TransactionDate { get; set; }
            public required decimal Amount { get; set; }
            public required TransactionType TransactionType { get; set; }
            public required string Description { get; set; }
            public Guid? BankStatementId { get; set; }
            public BankStatement? bankStatement { get; set; }// Renamed property
            public AccountingEntry? AccountingEntry { get; set; } // Added property
            public Guid? NominalAccountId { get; set; } // Added nominal account id
            public NominalAccount? NominalAccount { get; set; } // Added navigation property
        }
        /// <summary>
        /// DTO representing a parsed bank statement.
        /// </summary>
        public class ParsedBankStatementDto
        {
            public Guid Id { get; set; }
            public required string BlobName { get; set; }
            public DateTime StatementStartDate { get; set; }
            public DateTime StatementEndDate { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal ClosingBalance { get; set; }
            /// <summary>
            /// List of parsed bank transactions with optional manual nominal entry.
            /// </summary>
            public List<ParsedBankTransactionsDto> Transactions { get; set; } = new();
            public decimal StatementNumber { get; set; }
        }
        /// <summary>
        /// DTO representing a parsed bank transaction.
        /// Nominal accounts follow the public enum NominalAccountType:
        /// Asset, Liability, Equity, Revenue, Expense.
        /// </summary>
        public class ParsedBankTransactionsDto
        {
            public required string Description { get; set; }
            public required DateTime TransactionDate { get; set; }
            public required decimal Amount { get; set; }
            public required TransactionType TransactionType { get; set; }
            /// <summary>
            /// Recommended nominal account name, can be set manually or by LLM.
            /// </summary>
            public string? RecommendedNominalAccount { get; set; }
            /// <summary>
            /// Optional nominal account type for manual entry.
            /// </summary>
            public NominalAccountType? RecommendedNominalAccountType { get; set; }
        }

        //-------------------- Purchase Invoice Schema ---------------------//
        public class PurchaseInvoice
        {
            public Guid Id { get; set; }
            public required string BlobName { get; set; }
            public required string PurchaseInvoiceNumber { get; set; }
            public required string Supplier { get; set; }
            public decimal NetAmount { get; set; }
            public decimal GrossAmount { get; set; }
            public decimal TaxAmount { get; set; } // Ensure this property is included
            public required string Description { get; set; }
            public required string SupplierAddress { get; set; }
            public DateTime PurchaseInvoiceDate { get; set; }
            public required string DocumentType { get; set; } // e.g., "Purchase Invoice", "Credit Note"
                                                              // public required string Response { get; set; } // e.g., "Approved", "Rejected"
            public required string Response { get; set; } // e.g., "Approved", "Rejected"
                                                          // due date payment
            public DateTime? DueDate { get; set; } // Nullable to allow for no due date
            public bool IsPaid { get; set; } = false; // Default to false, can be changed later
            // user account
            public Guid? UserId { get; set; }
            public User? User { get; set; }
            // Generated Accounting Entry
            public Guid? AccountingEntryId { get; set; }
            public AccountingEntry? AccountingEntry { get; set; }
            public ICollection<PurchaseInvoiceLine> Lines { get; set; } = new List<PurchaseInvoiceLine>();
        }
        public class PurchaseInvoiceLine
        {
            public Guid Id { get; set; }
            public required string Description { get; set; }
            public required decimal Quantity { get; set; }
            public required decimal UnitPrice { get; set; }
            public required decimal TotalPrice { get; set; } // Calculate total price based on quantity and unit price
            public Guid? NominalAccountId { get; set; } // Optional, if you want to link to a nominal account
            public decimal TaxAmount { get; set; } // Ensure this property is included
            public Guid? PurchaseInvoiceId { get; set; }
            public PurchaseInvoice? PurchaseInvoice { get; set; }
        }

        public class PurchaseInvoiceLineDto
        {
            public required string Description { get; set; }
            public required decimal Quantity { get; set; }
            public required decimal UnitPrice { get; set; }
            public required decimal TotalPrice { get; set; } // Calculate total price based on quantity and unit price
            public decimal TaxAmount { get; set; } // Ensure this property is included
            public string? RecommendedNominalAccount { get; set; } // Optional, if you want to recommend a nominal account
            public NominalAccountType? RecommendedNominalAccountType { get; set; } // Optional, if you want to recommend a nominal account type
        }

        public class ParsedPurchaseInvoiceDto
        {
            public required string BlobName { get; set; }
            public required string InvoiceNumber { get; set; }
            public DateTime InvoiceDate { get; set; }
            public required string SupplierName { get; set; }
            public required string Address { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal NetAmount { get; set; }
            public DateTime? DueDate { get; set; }
            public List<ParsedInvoiceLineDto> LineItems { get; set; } = new List<ParsedInvoiceLineDto>();

            // If your LLM can categorize line items, you might add:
            // public List<ParsedInvoiceLineDto> LineItems { get; set; } = new List<ParsedInvoiceLineDto>();
            public string? RecommendedPayableNominal { get; set; } // For purchase invoices
            public NominalAccountType? RecommendedPayableNominalType { get; set; } // e.g., "Purchase Expense"
            public string? RecommendedExpenseNominal { get; set; } // For credit notes
            public NominalAccountType? RecommendedExpenseNominalType { get; set; } // e.g., "Purchase Expense"
            public string? RecommendedTaxNominal { get; set; } // e.g., "VAT Input" or "GST Input"
            public NominalAccountType? RecommendedTaxNominalType { get; set; } //
        }
        public class ParsedInvoiceLineDto
        {
            public required string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalAmount => Quantity * UnitPrice; // Calculate total price based on quantity and unit price
            public decimal TaxAmount { get; set; }
            public string? RecommendedNominalAccount { get; set; } // Optional, if you want
            public NominalAccountType? RecommendedNominalAccountType { get; set; } // Optional

        }
        public class CreatePurchaseInvoiceDto
        {
            public ParsedPurchaseInvoiceDto? ParsedInvoice { get; set; } = default;
            public Guid? UserId { get; set; } // Optional user ID for the creator
        }
        //-------------------- Accounting Schema ---------------------//
        public enum NominalAccountType
        {
            Asset,
            Liability,
            Equity,
            Revenue,
            Expense
        }
        public class NominalAccount
        {
            public NominalAccount() { }
            public Guid Id { get; set; }
            public required string Name { get; set; }
            public required string Code { get; set; }
            public Guid? AccountTypeid { get; set; } // e.g., Asset, Liability, Equity, Revenue, Expense
            public NominalAccountType type { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
            public string Description { get; set; } = string.Empty;
            public bool IsActive { get; set; } = true; // Default to true, can be changed later
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime? UpdatedAt { get; set; } // Nullable to allow for no updates
            public Guid? AccountingEntryId { get; set; }
            public AccountingEntry? AccountingEntry { get; set; }
        }
        public class NominalAccountSuggestionDto
        {
            public required string Name { get; set; }
            public required string Code { get; set; }
            public NominalAccountType Type { get; set; }
            public string? Description { get; set; } // Optional description
        }
        public class AccountingEntry
        {
            public AccountingEntry() { }
            public Guid Id { get; set; }
            public required string Description { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal TotalCredit { get; set; }
            public DateTime EntryDate { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
            public ICollection<AccountingEntryLine> Lines { get; set; } = new List<AccountingEntryLine>();
            public Guid? NominalAccountId { get; set; }
            public NominalAccount? NominalAccount { get; set; }
            public Guid? BankTransactionId { get; set; } // Added property
            public BankTransaction? BankTransaction { get; set; } // Added navigation property
            public Guid? SalesInvoiceId { get; set; } // Added property
            public Guid? PurchaseInvoiceId { get; set; } // Added property for purchase invoice
            public PurchaseInvoice? PurchaseInvoice { get; set; } // Navigation property for purchase invoice
        }
        public class AccountingEntryLine
        {
            public Guid Id { get; set; }
            public required string Description { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
            public Guid? AccountingEntryId { get; set; }
            public AccountingEntry? AccountingEntry { get; set; }
            public Guid? NominalAccountId { get; set; }
            public NominalAccount? NominalAccount { get; set; }
        }

        //-------------------- Sales Invoice Schema ---------------------//
            public class SalesInvoice
            {
                public Guid Id { get; set; }
                public required string BlobName { get; set; }
                public DateTime InvoiceDate { get; set; }
                public required string InvoiceNumber { get; set; }
                public required string CustomerName { get; set; }
                public required string CustomerAddress { get; set; }
                public decimal NetAmount { get; set; }
                public decimal SalesTax { get; set; }
                public decimal TotalAmount { get; set; }
                public DateTime? DueDate { get; set; } // Nullable to allow for no due date
                public bool IsPaid { get; set; } = false; // Default to false, can be changed later
                public required Guid UserId { get; set; }
                public required User User { get; set; } // Assuming UserId is a string, adjust as necessary
                public ICollection<SalesInvoiceLine> Lines { get; set; } = new List<SalesInvoiceLine>();
                public Guid? AccountingEntryId { get; set; }
                public AccountingEntry? AccountingEntry { get; set; }
            }
        public class SalesInvoiceLine
        {
            public Guid Id { get; set; }
            public required string Description { get; set; }
            public SalesInvoice? SalesInvoice { get; set; }
            public Guid? SalesInvoiceId { get; set; }
            public decimal quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; } // Calculate total price based on quantity and unit price
            public Guid? NominalAccountId { get; set; } // Optional, if you want to link to a nominal account
            public NominalAccount? NominalAccount { get; set; } // Optional, if you want to link to a nominal account
        }
        public class SalesInvoiceLineDto
        {
            public required string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice => Quantity * UnitPrice; // Calculate total price based on quantity and unit price
            public string? RecommendedNominal { get; set; } // Optional, if you want to recommend a nominal account
            public NominalAccountType? RecommendedNominalType { get; set; } // Optional, if you want to recommend a nominal account type
        }
        public class GenerateSalesInvoiceDto
        {
            public required string CustomerName { get; set; }
            public DateTime InvoiceDate { get; set; } = DateTime.Now; // Default to current date
            public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30); // Default to 30 days from now
            public required List<SalesInvoiceLineDto> LineItems { get; set; } = new List<SalesInvoiceLineDto>();
            public string? Notes { get; set; }
            public string? CustomerAddress { get; set; } // Optional customer address

            public string? RecommendedRecieivableNominal { get; set; } // For sales invoices
            public NominalAccountType? RecommendedRecieivableNominalType { get; set; } // e.g., "Sales Revenue"
            public string? RecommendedRevenueNominal { get; set; } // e.g., "Sales Revenue"
            public NominalAccountType? RecommendedRevenueNominalType { get; set; } // e.g., "Sales Revenue"
            public string? RecommendedTaxNominal { get; set; } // e.g., "VAT Payable" or "GST Payable"
            public NominalAccountType? RecommendedTaxNominalType { get; set; } // e.g., "Sales Tax"
        }

        public class parsedSalesInvoiceDto
        {
            public required string BlobName { get; set; }
            public required string InvoiceNumber { get; set; }
            public DateTime InvoiceDate { get; set; }
            public required string CustomerName { get; set; }
            public required string CustomerAddress { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTax { get; set; }
            public decimal TotalAmount { get; set; }

            public DateTime? DueDate { get; set; }

            // If your LLM can categorize line items, you might add:
            // public List<ParsedInvoiceLineDto> LineItems { get; set; } = new List<ParsedInvoiceLineDto>();
            public string? RecommendedRecieivableNominal { get; set; } // For sales invoices
            public NominalAccountType? RecommendedRecieivableNominalType { get; set; } // e.g., "Sales Revenue"
            public string? RecommendedRevenueNominal { get; set; } // e.g., "Sales Revenue"
            public NominalAccountType? RecommendedRevenueNominalType { get; set; } // e.g., "Sales Revenue"
            public string? RecommendedTaxNominal { get; set; } // e.g., "VAT Payable" or "GST Payable"
            public NominalAccountType? RecommendedTaxNominalType { get; set; } // e.g., "Sales Tax"
            public List<parsedSalesInvoiceLineDto> ParsedSalesInvoiceLines { get; set; } = new List<parsedSalesInvoiceLineDto>();
        }
        public class parsedSalesInvoiceLineDto
        {
            public required string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice => Quantity * UnitPrice; // Calculate total price based on quantity and unit price
            public string? RecommendedNominal { get; set; } // Optional, if you want to recommend a nominal account
            public NominalAccountType? RecommendedNominalType { get; set; } // Optional, if you want to recommend a nominal account type
        }

        //-------------------- LLM Schema ---------------------//
        public class LlmRequest
        {
            public Guid Id { get; set; }
            public required string Request { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
        }
        public class LlmResponse
        {
            public Guid Id { get; set; }
            public required string Response { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
        }
        //-------------------- Document Schema ---------------------//

        public class DocumentRecord
            {
                public Guid Id { get; set; }
                public required string BlobName { get; set; }
                public DateTime CreatedAt { get; set; }
                public required string DocumentContent { get; set; }
                public required string DocumentType { get; set; }
                public Guid? UserId { get; set; }
                public User? User { get; set; }
            }
        //-------------------- Property Log Schema ---------------------//
        public class PropertyLog
        {
            public Guid Id { get; set; }
            public required string PropertyName { get; set; }
            public required string PropertyValue { get; set; }
            public required string PropertyType { get; set; }
            public DateTime LoggedAt { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
        }

        //-------------------- Journal Entry Schema ---------------------//
        public class JournalEntry
        {
            public Guid Id { get; set; }
            public required string Description { get; set; }
            public DateTime EntryDate { get; set; }
            public Guid? UserId { get; set; }
            public User? User { get; set; }
        }

        

        //-------------------- DbContext Configuration ---------------------//

        public class Result<T>
        {
            public bool Success { get; }
            public string? Error { get; }
            public T? Value { get; }

            private Result(bool success, T? value = default, string? error = null)
            {
                this.Success = success;
                Value = value;
                Error = error;
            }
            public static Result<T> SuccessResult(T value) => new(true, value);
            public static Result<T> Fail(string error) => new(false, error: error);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.PurchaseInvoiceNumber)
                .IsRequired();
            
            modelBuilder.Entity<PurchaseInvoice>()
                .HasIndex(pi => pi.PurchaseInvoiceNumber)
                .IsUnique();
            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.BlobName)
                .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
            .Property(p => p.Supplier)
            .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.SupplierAddress)
                .IsRequired();
                modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.DocumentType)
                .IsRequired();
                modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.Response)
                .IsRequired();
                modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.NetAmount)
                .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
            .Property(p => p.GrossAmount)
            .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
            .Property(p => p.TaxAmount)
            .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.PurchaseInvoiceDate)
                .IsRequired();
                modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.DueDate)
                .IsRequired();
                modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.IsPaid)
                .IsRequired();
            modelBuilder.Entity<PurchaseInvoice>()
            .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PurchaseInvoice>()
            .HasOne(p => p.AccountingEntry)
                .WithMany()
                .HasForeignKey(p => p.AccountingEntryId)
                .OnDelete(DeleteBehavior.Restrict);
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
                
                modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.CustomerName)
                .IsRequired();
                modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.CustomerAddress)
                .IsRequired();
                modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.NetAmount)
                .IsRequired();
                modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.SalesTax)
                .IsRequired();
                modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.TotalAmount)
                .IsRequired();

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

            modelBuilder.Entity<NominalAccount>()
                .HasKey(n => n.Id)
                .HasName("PK_NominalAccounts");

            modelBuilder.Entity<NominalAccount>()
                .HasIndex(n => n.Name)
                .IsUnique();

            modelBuilder.Entity<NominalAccount>()
                .HasIndex(n => n.Code)
                .IsUnique();

            modelBuilder.Entity<NominalAccount>()
                .Property(n => n.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<NominalAccount>()
                .Property(n => n.IsActive)
                .IsRequired();

            modelBuilder.Entity<NominalAccount>()
                .Property(n => n.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<NominalAccount>()
                .Property(n => n.UpdatedAt)
                .IsRequired(false); // Nullable to allow for no updates

            modelBuilder.Entity<NominalAccount>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NominalAccount>()
                .HasOne<NominalAccount>()
                .WithMany()
                .HasForeignKey(n => n.AccountingEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NominalAccount>()
                .HasMany<AccountingEntry>()
                .WithOne()
                .HasForeignKey(ae => ae.NominalAccountId)
                .IsRequired(false); // Nullable to allow for no accounting entry
            

            modelBuilder.Entity<AccountingEntry>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<AccountingEntry>()
                .HasOne<NominalAccount>()
                .WithMany()
                .HasForeignKey(a => a.NominalAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<AccountingEntry>()
                .Property(a => a.TotalDebit)
                .IsRequired();
            modelBuilder.Entity<AccountingEntry>()
                .Property(a => a.TotalCredit)
                .IsRequired();
            modelBuilder.Entity<AccountingEntry>()
                .Property(a => a.EntryDate)
                .IsRequired();
            modelBuilder.Entity<AccountingEntry>()
                .HasOne(a => a.BankTransaction)
                .WithOne(b => b.AccountingEntry)
                .HasForeignKey<AccountingEntry>(a => a.BankTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccountingEntry>()
                .HasIndex(a => a.SalesInvoiceId)
                .IsUnique()
                .HasFilter("SalesInvoiceId IS NOT NULL");

            modelBuilder.Entity<AccountingEntry>()
                .HasOne<SalesInvoice>()
                .WithMany()
                .HasForeignKey(a => a.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccountingEntry>()
                .HasIndex(a => a.PurchaseInvoiceId)
                .IsUnique()
                .HasFilter("PurchaseInvoiceId IS NOT NULL");

            modelBuilder.Entity<AccountingEntry>()
                .HasOne<PurchaseInvoice>()
                .WithMany()
                .HasForeignKey(a => a.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<PurchaseInvoice>()
                .HasMany(p => p.Lines)
                .WithOne(l => l.PurchaseInvoice)
                .HasForeignKey(l => l.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankTransaction>()
            .HasOne(a => a.AccountingEntry)
                .WithOne(b => b.BankTransaction)
                .HasForeignKey<BankTransaction>(b => b.Id)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BankTransaction>()
            .HasIndex(b => b.TransactionNumber)
                .IsUnique();

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
        }
    }
}
