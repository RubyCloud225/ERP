using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;

namespace ERP.Service
{
    public interface IIFRSBalanceSheetService
    {
        IFRSBalanceSheet GenerateBalanceSheet(DateTime financialYearEnd);
    }
    public class IFRSBalanceSheetService : IIFRSBalanceSheetService
    {
        private readonly INominalLedgerService _ledgerService;

        public IFRSBalanceSheetService(INominalLedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        public IFRSBalanceSheet GenerateBalanceSheet(DateTime financialYearEnd)
        {
            // Define the financial year start and end dates
            DateTime financialYearStart = new DateTime(financialYearEnd.Year, 1, 1);
            DateTime financialYearEndDate = financialYearEnd;

            // Get ledger entries for the financial year
            var entries = _ledgerService.GetEntries(financialYearStart, financialYearEndDate);

            // Aggregate balances by account code
            var accountBalances = entries
                .GroupBy(e => e.AccountCode)
                .Select(g => new
                {
                    AccountCode = g.Key,
                    Debit = g.Sum(e => e.Debit),
                    Credit = g.Sum(e => e.Credit),
                    Balance = g.Sum(e => e.Debit) - g.Sum(e => e.Credit)
                })
                .ToList();

            // Classify accounts according to IFRS balance sheet categories
            var assets = new List<AccountBalance>();
            var liabilities = new List<AccountBalance>();
            var equity = new List<AccountBalance>();

            foreach (var account in accountBalances)
            {
                if (IsAssetAccount(account.AccountCode))
                {
                    assets.Add(new AccountBalance { AccountCode = account.AccountCode, Balance = account.Balance });
                }
                else if (IsLiabilityAccount(account.AccountCode))
                {
                    liabilities.Add(new AccountBalance { AccountCode = account.AccountCode, Balance = account.Balance });
                }
                else if (IsEquityAccount(account.AccountCode))
                {
                    equity.Add(new AccountBalance { AccountCode = account.AccountCode, Balance = account.Balance });
                }
                // else ignore or handle other accounts
            }

            return new IFRSBalanceSheet
            {
                FinancialYearEnd = financialYearEndDate,
                Assets = assets,
                Liabilities = liabilities,
                Equity = equity
            };
        }

        private bool IsAssetAccount(string accountCode)
        {
            // Simplified example: asset accounts start with "1"
            return accountCode.StartsWith("1");
        }

        private bool IsLiabilityAccount(string accountCode)
        {
            // Simplified example: liability accounts start with "2"
            return accountCode.StartsWith("2");
        }

        private bool IsEquityAccount(string accountCode)
        {
            // Simplified example: equity accounts start with "3"
            return accountCode.StartsWith("3");
        }
    }

    public class IFRSBalanceSheet
    {
        public DateTime FinancialYearEnd { get; set; }
        public List<AccountBalance> Assets { get; set; } = new List<AccountBalance>();
        public List<AccountBalance> Liabilities { get; set; } = new List<AccountBalance>();
        public List<AccountBalance> Equity { get; set; } = new List<AccountBalance>();
    }

    public class AccountBalance
    {
        public string AccountCode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
