using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.Model;

namespace ERP.Service
{
    public interface IFinancialRatioService
    {
        Task<Dictionary<string, decimal>> CalculateProfitabilityRatiosAsync(ApplicationDbContext.FinancialDataDto financialData);
        Task<Dictionary<string, decimal>> CalculateLiquidityRatiosAsync(ApplicationDbContext.FinancialDataDto financialData);
        Task<Dictionary<string, decimal>> CalculateEfficiencyRatiosAsync(ApplicationDbContext.FinancialDataDto financialData);
        Task<Dictionary<string, decimal>> CalculateValuationRatiosAsync(ApplicationDbContext.FinancialDataDto financialData);
        Task<Dictionary<string, decimal>> GetBenchmarkRatiosAsync(string industry, string geography);
    }

    public class FinancialRatioService : IFinancialRatioService
    {
        public async Task<Dictionary<string, decimal>> CalculateProfitabilityRatiosAsync(ApplicationDbContext.FinancialDataDto financialData)
        {
            var ratios = new Dictionary<string, decimal>();

            if (financialData.FinancialMetrics.TryGetValue("NetIncome", out var netIncome) &&
                financialData.FinancialMetrics.TryGetValue("Revenue", out var revenue) && revenue != 0)
            {
                ratios["Net Profit Margin"] = netIncome / revenue;
            }

            if (financialData.FinancialMetrics.TryGetValue("EBIT", out var ebit) &&
                financialData.FinancialMetrics.TryGetValue("Revenue", out var revenue2) && revenue2 != 0)
            {
                ratios["Operating Profit Margin"] = ebit / revenue2;
            }

            if (financialData.FinancialMetrics.TryGetValue("NetIncome", out var netIncome2) &&
                financialData.FinancialMetrics.TryGetValue("TotalAssets", out var totalAssets) && totalAssets != 0)
            {
                ratios["Return on Assets (ROA)"] = netIncome2 / totalAssets;
            }

            if (financialData.FinancialMetrics.TryGetValue("NetIncome", out var netIncome3) &&
                financialData.FinancialMetrics.TryGetValue("ShareholdersEquity", out var shareholdersEquity) && shareholdersEquity != 0)
            {
                ratios["Return on Equity (ROE)"] = netIncome3 / shareholdersEquity;
            }

            if (financialData.FinancialMetrics.TryGetValue("EBIT", out var ebit2) &&
                financialData.FinancialMetrics.TryGetValue("TotalAssets", out var totalAssets2) && totalAssets2 != 0)
            {
                ratios["Return on Capital Employed (ROCE)"] = ebit2 / totalAssets2;
            }

            return await Task.FromResult(ratios);
        }

        public async Task<Dictionary<string, decimal>> CalculateLiquidityRatiosAsync(ApplicationDbContext.FinancialDataDto financialData)
        {
            var ratios = new Dictionary<string, decimal>();

            if (financialData.FinancialMetrics.TryGetValue("CurrentAssets", out var currentAssets) &&
                financialData.FinancialMetrics.TryGetValue("CurrentLiabilities", out var currentLiabilities) && currentLiabilities != 0)
            {
                ratios["Current Ratio"] = currentAssets / currentLiabilities;
            }

            if (financialData.FinancialMetrics.TryGetValue("QuickAssets", out var quickAssets) &&
                financialData.FinancialMetrics.TryGetValue("CurrentLiabilities", out var currentLiabilities2) && currentLiabilities2 != 0)
            {
                ratios["Quick Ratio"] = quickAssets / currentLiabilities2;
            }

            if (financialData.FinancialMetrics.TryGetValue("CashAndCashEquivalents", out var cash) &&
                financialData.FinancialMetrics.TryGetValue("CurrentLiabilities", out var currentLiabilities3) && currentLiabilities3 != 0)
            {
                ratios["Cash Ratio"] = cash / currentLiabilities3;
            }

            return await Task.FromResult(ratios);
        }

        public async Task<Dictionary<string, decimal>> CalculateEfficiencyRatiosAsync(ApplicationDbContext.FinancialDataDto financialData)
        {
            var ratios = new Dictionary<string, decimal>();

            if (financialData.FinancialMetrics.TryGetValue("Revenue", out var revenue) &&
                financialData.FinancialMetrics.TryGetValue("TotalAssets", out var totalAssets) && totalAssets != 0)
            {
                ratios["Asset Turnover"] = revenue / totalAssets;
            }

            if (financialData.FinancialMetrics.TryGetValue("CostOfGoodsSold", out var cogs) &&
                financialData.FinancialMetrics.TryGetValue("Inventory", out var inventory) && inventory != 0)
            {
                ratios["Inventory Turnover"] = cogs / inventory;
            }

            if (financialData.FinancialMetrics.TryGetValue("Revenue", out var revenue2) &&
                financialData.FinancialMetrics.TryGetValue("AccountsReceivable", out var accountsReceivable) && accountsReceivable != 0)
            {
                ratios["Receivables Turnover"] = revenue2 / accountsReceivable;
            }

            if (financialData.FinancialMetrics.TryGetValue("CostOfGoodsSold", out var cogs2) &&
                financialData.FinancialMetrics.TryGetValue("AccountsPayable", out var accountsPayable) && accountsPayable != 0)
            {
                ratios["Payables Turnover"] = cogs2 / accountsPayable;
            }

            return await Task.FromResult(ratios);
        }

        public async Task<Dictionary<string, decimal>> CalculateValuationRatiosAsync(ApplicationDbContext.FinancialDataDto financialData)
        {
            var ratios = new Dictionary<string, decimal>();

            if (financialData.FinancialMetrics.TryGetValue("MarketPricePerShare", out var marketPrice) &&
                financialData.FinancialMetrics.TryGetValue("EarningsPerShare", out var eps) && eps != 0)
            {
                ratios["Price to Earnings (P/E)"] = marketPrice / eps;
            }

            if (financialData.FinancialMetrics.TryGetValue("MarketPricePerShare", out var marketPrice2) &&
                financialData.FinancialMetrics.TryGetValue("BookValuePerShare", out var bookValue) && bookValue != 0)
            {
                ratios["Price to Book (P/B)"] = marketPrice2 / bookValue;
            }

            if (financialData.FinancialMetrics.TryGetValue("MarketPricePerShare", out var marketPrice3) &&
                financialData.FinancialMetrics.TryGetValue("SalesPerShare", out var salesPerShare) && salesPerShare != 0)
            {
                ratios["Price to Sales (P/S)"] = marketPrice3 / salesPerShare;
            }

            if (financialData.FinancialMetrics.TryGetValue("MarketCapitalization", out var marketCap) &&
                financialData.FinancialMetrics.TryGetValue("EBITDA", out var ebitda) && ebitda != 0)
            {
                ratios["Enterprise Value to EBITDA (EV/EBITDA)"] = marketCap / ebitda;
            }

            return await Task.FromResult(ratios);
        }

        public async Task<Dictionary<string, decimal>> GetBenchmarkRatiosAsync(string industry, string geography)
        {
            var benchmarkRatios = new Dictionary<string, decimal>
            {
                { "Net Profit Margin Benchmark", 0.12M },
                { "Operating Profit Margin Benchmark", 0.15M },
                { "Return on Assets (ROA) Benchmark", 0.08M },
                { "Return on Equity (ROE) Benchmark", 0.10M },
                { "Return on Capital Employed (ROCE) Benchmark", 0.09M },
                { "Current Ratio Benchmark", 1.5M },
                { "Quick Ratio Benchmark", 1.2M },
                { "Cash Ratio Benchmark", 0.5M },
                { "Asset Turnover Benchmark", 0.8M },
                { "Inventory Turnover Benchmark", 6.0M },
                { "Receivables Turnover Benchmark", 8.0M },
                { "Payables Turnover Benchmark", 7.0M },
                { "Price to Earnings (P/E) Benchmark", 15.0M },
                { "Price to Book (P/B) Benchmark", 1.5M },
                { "Price to Sales (P/S) Benchmark", 2.0M },
                { "Enterprise Value to EBITDA (EV/EBITDA) Benchmark", 10.0M }
            };

            return await Task.FromResult(benchmarkRatios);
        }
    }
}
