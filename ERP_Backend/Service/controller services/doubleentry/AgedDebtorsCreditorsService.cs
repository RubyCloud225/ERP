using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.controller_services.doubleentry
{
    public class AgedDebtorsCreditorsService
    {
        private readonly ApplicationDbContext _dbContext;

        public AgedDebtorsCreditorsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AgedDebtor> GenerateAgedDebtorsReport(DateTime asOfDate)
        {
            var salesInvoices = _dbContext.SalesInvoices
                .Include(si => si.AccountingEntry)
                    .ThenInclude(ae => ae!.BankTransaction)
                .Include(si => si.User)
                .Where(si => si.InvoiceDate <= asOfDate)
                .ToList();

            var report = new List<AgedDebtor>();

            foreach (var invoice in salesInvoices)
            {
                decimal paidAmount = 0m;
                if (invoice.AccountingEntry?.BankTransaction != null)
                {
                    paidAmount = invoice.AccountingEntry.BankTransaction.Amount;
                }

                decimal outstanding = invoice.TotalAmount - paidAmount;

                if (outstanding <= 0)
                    continue;

                int daysOutstanding = (asOfDate - (invoice.DueDate ?? invoice.InvoiceDate)).Days;

                string agingBucket = GetAgingBucket(daysOutstanding);

                report.Add(new AgedDebtor
                {
                    CustomerName = invoice.CustomerName,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate,
                    OutstandingAmount = outstanding,
                    AgingBucket = agingBucket
                });
            }

            return report;
        }

        public List<AgedCreditor> GenerateAgedCreditorsReport(DateTime asOfDate)
        {
            var purchaseInvoices = _dbContext.PurchaseInvoices
                .Include(pi => pi.AccountingEntry)
                    .ThenInclude(ae => ae!.BankTransaction)
                .Include(pi => pi.User)
                .Where(pi => pi.PurchaseInvoiceDate <= asOfDate)
                .ToList();

            var report = new List<AgedCreditor>();

            foreach (var invoice in purchaseInvoices)
            {
                decimal paidAmount = 0m;
                if (invoice.AccountingEntry?.BankTransaction != null)
                {
                    paidAmount = invoice.AccountingEntry.BankTransaction.Amount;
                }

                decimal outstanding = invoice.GrossAmount - paidAmount;

                if (outstanding <= 0)
                    continue;

                int daysOutstanding = (asOfDate - (invoice.DueDate ?? invoice.PurchaseInvoiceDate)).Days;

                string agingBucket = GetAgingBucket(daysOutstanding);

                report.Add(new AgedCreditor
                {
                    SupplierName = invoice.Supplier,
                    InvoiceNumber = invoice.PurchaseInvoiceNumber,
                    InvoiceDate = invoice.PurchaseInvoiceDate,
                    DueDate = invoice.DueDate,
                    OutstandingAmount = outstanding,
                    AgingBucket = agingBucket
                });
            }

            return report;
        }

        private string GetAgingBucket(int daysOutstanding)
        {
            if (daysOutstanding <= 0)
                return "Current";
            else if (daysOutstanding <= 30)
                return "1-30 Days";
            else if (daysOutstanding <= 60)
                return "31-60 Days";
            else if (daysOutstanding <= 90)
                return "61-90 Days";
            else
                return "90+ Days";
        }
    }

    public class AgedDebtor
    {
        public string CustomerName { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string AgingBucket { get; set; } = string.Empty;
    }

    public class AgedCreditor
    {
        public string SupplierName { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string AgingBucket { get; set; } = string.Empty;
    }
}
