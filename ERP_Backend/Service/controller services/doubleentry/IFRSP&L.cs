using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;

namespace ERP.Service.controller_services.doubleentry
{
    public class IFRSPandLService
    {
        private readonly NominalLedgerService _ledgerService;

        public IFRSPandLService(NominalLedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        public ProfitAndLoss GenerateProfitAndLoss(DateTime financialYearEnd)
        {
            DateTime financialYearStart = new DateTime(financialYearEnd.Year, 1, 1);
            DateTime financialYearEndDate = financialYearEnd;

            var entries = _ledgerService.GetEntries(financialYearStart, financialYearEndDate);

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

            var income = new List<AccountBalance>();
            var expenditure = new List<AccountBalance>();

            foreach (var account in accountBalances)
            {
                if (IsIncomeAccount(account.AccountCode))
                {
                    income.Add(new AccountBalance { AccountCode = account.AccountCode, Balance = account.Balance });
                }
                else if (IsExpenditureAccount(account.AccountCode))
                {
                    expenditure.Add(new AccountBalance { AccountCode = account.AccountCode, Balance = account.Balance });
                }
                // else ignore or handle other accounts
            }

            return new ProfitAndLoss
            {
                FinancialYearEnd = financialYearEndDate,
                Income = income,
                Expenditure = expenditure
            };
        }

        private bool IsIncomeAccount(string accountCode)
        {
            // Simplified example: income accounts start with "4"
            return accountCode.StartsWith("4");
        }

        private bool IsExpenditureAccount(string accountCode)
        {
            // Simplified example: expenditure accounts start with "5"
            return accountCode.StartsWith("5");
        }
    }

    public class ProfitAndLoss
    {
        public DateTime FinancialYearEnd { get; set; }
        public List<AccountBalance> Income { get; set; } = new List<AccountBalance>();
        public List<AccountBalance> Expenditure { get; set; } = new List<AccountBalance>();
    }

    public class AccountBalance
    {
        public string AccountCode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
