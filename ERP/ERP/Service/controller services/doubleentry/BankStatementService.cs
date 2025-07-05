using ERP.Model;
using static ERP.Model.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Service
{
    public interface IBankStatementService
    {
        Task<BankStatement> ProcessBankStatementAsync(ParsedBankStatementDto parsedBankStatement, Guid? userId);

        Task<BankStatement?> GetBankStatementByIdAsync(Guid id);

        Task<IEnumerable<BankStatement>> GetBankStatementsByUserAsync(Guid userId);

        Task<bool> DeleteBankStatementAsync(Guid id);

        Task<BankStatement> AmendBankStatementAsync(BankStatement amendedBankStatement);

        Task<bool> ReconcileBankStatementAsync(Guid bankStatementId, decimal userInputBalance);
    }

    public class BankStatementService : IBankStatementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILlmService _llmService;
        private readonly IAccountingService _accountingService;
        private readonly INominalAccountResolutionService _nominalAccountResolutionService;

        public BankStatementService(ApplicationDbContext dbContext, ILlmService llmService, IAccountingService accountingService, INominalAccountResolutionService nominalAccountResolutionService)
        {
            _dbContext = dbContext;
            _llmService = llmService;
            _accountingService = accountingService;
            _nominalAccountResolutionService = nominalAccountResolutionService;
        }

        public async Task<BankStatement> ProcessBankStatementAsync(ParsedBankStatementDto parsedBankStatement, Guid? userId)
        {
            if (parsedBankStatement == null)
            {
                throw new ArgumentNullException(nameof(parsedBankStatement), "Parsed bank statement cannot be null");
            }

            var bankStatement = new BankStatement
            {
                Id = Guid.NewGuid(),
                BlobName = parsedBankStatement.BlobName,
                StatementStartDate = parsedBankStatement.StatementStartDate,
                StatementEndDate = parsedBankStatement.StatementEndDate,
                OpeningBalance = parsedBankStatement.OpeningBalance,
                ClosingBalance = parsedBankStatement.ClosingBalance,
                StatementNumber = parsedBankStatement.StatementNumber,
                UserId = userId,
                Transactions = new List<BankTransaction>()
            };

            _dbContext.BankStatements.Add(bankStatement);
            await _dbContext.SaveChangesAsync();

            foreach (var transactionDto in parsedBankStatement.Transactions)
            {
                string nominalName;
                ApplicationDbContext.NominalAccountType? nominalType;

                if (!string.IsNullOrWhiteSpace(transactionDto.RecommendedNominalAccount) && transactionDto.RecommendedNominalAccountType.HasValue)
                {
                    // Use manual nominal entry if provided
                    nominalName = transactionDto.RecommendedNominalAccount;
                    nominalType = transactionDto.RecommendedNominalAccountType;
                }
                else
                {
                    // Call LLM service to get nominal recommendation for each transaction
                    string prompt = $"You are an expert accounting AI. Please recommend the appropriate nominal account for the following bank transaction:\n" +
                                    $"Description: {transactionDto.Description}\n" +
                                    $"Date: {transactionDto.TransactionDate:yyyy-MM-dd}\n" +
                                    $"Amount: {transactionDto.Amount:C}\n" +
                                    $"Transaction Type: {transactionDto.TransactionType}\n";

                    string llmResponse = await _llmService.GenerateResponseAsync(prompt);

                    // For simplicity, assume the LLM response is the nominal account name
                    nominalName = llmResponse.Trim();
                    nominalType = null;
                }

                // Resolve or create nominal account
                Guid nominalAccountId = await _nominalAccountResolutionService.ResolveOrCreateNominalAccountAsync(nominalName, nominalType);

                var bankTransaction = new BankTransaction
                {
                    Id = Guid.NewGuid(),
                    TransactionNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    TransactionDate = transactionDto.TransactionDate,
                    Amount = transactionDto.Amount,
                    TransactionType = transactionDto.TransactionType,
                    Description = transactionDto.Description,
                    BankStatementId = bankStatement.Id,
                    NominalAccountId = nominalAccountId
                };

                _dbContext.BankTransactions.Add(bankTransaction);
                await _dbContext.SaveChangesAsync();

                // Create accounting entry for the bank transaction
                await _accountingService.CreateAccountingForBankTransactionAsync(bankTransaction, nominalAccountId, userId);
            }

            return bankStatement;
        }

        public async Task<BankStatement?> GetBankStatementByIdAsync(Guid id)
        {
            return await _dbContext.BankStatements
                .Include(bs => bs.Transactions)
                .FirstOrDefaultAsync(bs => bs.Id == id);
        }

        public async Task<IEnumerable<BankStatement>> GetBankStatementsByUserAsync(Guid userId)
        {
            return await _dbContext.BankStatements
                .Where(bs => bs.UserId == userId)
                .Include(bs => bs.Transactions)
                .ToListAsync();
        }

        public async Task<bool> DeleteBankStatementAsync(Guid id)
        {
            var bankStatement = await _dbContext.BankStatements
                .Include(bs => bs.Transactions)
                .FirstOrDefaultAsync(bs => bs.Id == id);

            if (bankStatement == null)
            {
                return false;
            }

            _dbContext.BankTransactions.RemoveRange(bankStatement.Transactions);
            _dbContext.BankStatements.Remove(bankStatement);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<BankStatement> AmendBankStatementAsync(BankStatement amendedBankStatement)
        {
            var existingBankStatement = await _dbContext.BankStatements
                .Include(bs => bs.Transactions)
                .FirstOrDefaultAsync(bs => bs.Id == amendedBankStatement.Id);

            if (existingBankStatement == null)
            {
                throw new ArgumentException("Bank statement not found", nameof(amendedBankStatement));
            }

            // Update properties
            existingBankStatement.BlobName = amendedBankStatement.BlobName;
            existingBankStatement.StatementStartDate = amendedBankStatement.StatementStartDate;
            existingBankStatement.StatementEndDate = amendedBankStatement.StatementEndDate;
            existingBankStatement.OpeningBalance = amendedBankStatement.OpeningBalance;
            existingBankStatement.ClosingBalance = amendedBankStatement.ClosingBalance;
            existingBankStatement.StatementNumber = amendedBankStatement.StatementNumber;
            existingBankStatement.UserId = amendedBankStatement.UserId;

            // Optionally update transactions if needed (not implemented here)

            await _dbContext.SaveChangesAsync();

            return existingBankStatement;
        }

        public async Task<bool> ReconcileBankStatementAsync(Guid bankStatementId, decimal userInputBalance)
        {
            var bankStatement = await _dbContext.BankStatements
                .FirstOrDefaultAsync(bs => bs.Id == bankStatementId);

            if (bankStatement == null)
            {
                return false;
            }

            // Simple reconciliation logic: compare user input balance with closing balance
            if (bankStatement.ClosingBalance == userInputBalance)
            {
                // Mark as reconciled (assuming a Reconciled flag exists, else add one)
                // For now, let's assume we add a Reconciled boolean property to BankStatement
                bankStatement.Reconciled = true;
            }
            else
            {
                bankStatement.Reconciled = false;
            }

            await _dbContext.SaveChangesAsync();

            return bankStatement.Reconciled;
        }
    }
}
